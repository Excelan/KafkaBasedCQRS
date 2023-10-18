namespace Post.Cmd.Infrastructure.Repositories;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CQRS.Core.Domain;
using CQRS.Core.Events;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Post.Cmd.Infrastructure.Config;

internal class EventStoreRepository : IEventStoreRepository
{
    private readonly IMongoCollection<EventModel> _eventStoreCollection;
    public EventStoreRepository(IOptions<MongoDbConfig> config)
    {
        var mongoClient = new MongoClient(config.Value.ConnectionString);
        var mongoDatabase = mongoClient.GetDatabase(config.Value.Database);
        _eventStoreCollection = mongoDatabase.GetCollection<EventModel>(config.Value.Collection);
    }

    public Task<List<EventModel>> FindAggregateById(Guid aggregateId) {
        throw new NotImplementedException();
    }

    public Task SaveAsync(EventModel @event) {
        throw new NotImplementedException();
    }
}
