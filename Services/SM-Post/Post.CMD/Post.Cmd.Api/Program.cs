using Confluent.Kafka;
using CQRS.Core.Domain;
using CQRS.Core.Events;
using CQRS.Core.Handlers;
using CQRS.Core.Infrastructure;
using CQRS.Core.Producesrs;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization;
using Post.Cmd.Api.CommandHandler;
using Post.Cmd.Api.Commands;
using Post.Cmd.Domain.Aggregates;
using Post.Cmd.Infrastructure.Config;
using Post.Cmd.Infrastructure.Dispatchers;
using Post.Cmd.Infrastructure.Handlers;
using Post.Cmd.Infrastructure.Producers;
using Post.Cmd.Infrastructure.Repositories;
using Post.Cmd.Infrastructure.Stores;
using Post.Common.Events;

var builder = WebApplication.CreateBuilder(args);

BsonClassMap.RegisterClassMap<BaseEvent>();
BsonClassMap.RegisterClassMap<PostCreatedEvent>();
BsonClassMap.RegisterClassMap<MessageUpdatedEvent>();
BsonClassMap.RegisterClassMap<PostLikeEvent>();
BsonClassMap.RegisterClassMap<PostRemovedEvent>();
BsonClassMap.RegisterClassMap<CommentAddedEvent>();
BsonClassMap.RegisterClassMap<CommentUpdatedEvent>();
BsonClassMap.RegisterClassMap<CommentRemovedEvent>();

// Add services to the container.
builder.Services.Configure<MongoDbConfig>(builder.Configuration.GetSection(nameof(MongoDbConfig)));
builder.Services.Configure<ProducerConfig>(builder.Configuration.GetSection(nameof(ProducerConfig)));
builder.Services.AddSingleton<IEventStoreRepository, EventStoreRepository>();
builder.Services.AddSingleton<IEventProducer, EventProducer>();
builder.Services.AddSingleton<IEventStore, EventStore>();
builder.Services.AddSingleton<IEventSourcingHandler<PostAggregate>, EventSourcingHandler>();
builder.Services.AddSingleton<ICommandHandler, CommandHandler>();
builder.Services.AddSingleton(CreateCommandDispatcher);

static ICommandDispatcher CreateCommandDispatcher(IServiceProvider sp) {
    var commandHandler = sp.GetRequiredService<ICommandHandler>();
    var dispatcher = new CommandDispatcher();
    dispatcher.RegisterHandler<NewPostCommand>(commandHandler.HandleAsync);
    dispatcher.RegisterHandler<LikePostCommand>(commandHandler.HandleAsync);
    dispatcher.RegisterHandler<EditMessageCommand>(commandHandler.HandleAsync);
    dispatcher.RegisterHandler<DeletePostCommand>(commandHandler.HandleAsync);
    dispatcher.RegisterHandler<AddCommentCommand>(commandHandler.HandleAsync);
    dispatcher.RegisterHandler<EditCommentCommand>(commandHandler.HandleAsync);
    dispatcher.RegisterHandler<RemoveCommentCommand>(commandHandler.HandleAsync);
    dispatcher.RegisterHandler<RestoreReadDbCommand>(commandHandler.HandleAsync);
    return dispatcher;
};

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
