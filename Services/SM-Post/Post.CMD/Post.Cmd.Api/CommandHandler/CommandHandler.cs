using Post.Cmd.Api.Commands;

namespace Post.Cmd.Api.CommandHandler;

using CQRS.Core.Handlers;
using Post.Cmd.Domain.Aggregates;

// CommandHandler is a Concrete Coleague in Mediator pattern.
// ICommandHandler is a IColeague in Mediator pattern.
// A Coleague (this Command handler) should be registered in a Mediator ()
public class CommandHandler : ICommandHandler
{
    private readonly IEventSourcingHandler<PostAggregate> _eventSourcingHandler;

    public CommandHandler(IEventSourcingHandler<PostAggregate> eventSourcingHandler)
    {
        _eventSourcingHandler = eventSourcingHandler;
    }
    public async Task HandleAsync(NewPostCommand command)
    {
        var postAggregate = new PostAggregate(command.Id, command.Author, command.Message);
        await _eventSourcingHandler.SaveAsync(postAggregate);
    }

    public async Task HandleAsync(EditMessageCommand command)
    {
        var aggragate = await _eventSourcingHandler.GetByIdAsync(command.Id);
        aggragate.EditMessage(command.Message);
        await _eventSourcingHandler.SaveAsync(aggragate);
    }

    public async Task HandleAsync(LikePostCommand command)
    {
        var aggragate = await _eventSourcingHandler.GetByIdAsync(command.Id);
        aggragate.LikePost();
        await _eventSourcingHandler.SaveAsync(aggragate);
    }

    public async Task HandleAsync(DeletePostCommand command)
    {
        var aggragate = await _eventSourcingHandler.GetByIdAsync(command.Id);
        aggragate.DeletePost(command.Username);
        await _eventSourcingHandler.SaveAsync(aggragate);
    }

    public async Task HandleAsync(AddCommentCommand command)
    {
        var aggragate = await _eventSourcingHandler.GetByIdAsync(command.Id);
        aggragate.AddComment(command.Comment, command.Username);
        await _eventSourcingHandler.SaveAsync(aggragate);
    }

    public async Task HandleAsync(EditCommentCommand command)
    {
        var aggragate = await _eventSourcingHandler.GetByIdAsync(command.Id);
        aggragate.EditComment(command.CommentId, command.Comment, command.Username);
        await _eventSourcingHandler.SaveAsync(aggragate);
    }

    public async Task HandleAsync(RemoveCommentCommand command)
    {
        var aggragate = await _eventSourcingHandler.GetByIdAsync(command.Id);
        aggragate.RemoveComment(command.CommentId, command.Username);
        await _eventSourcingHandler.SaveAsync(aggragate);
    }

    public async Task HandleAsync(RestoreReadDbCommand command) {
        await _eventSourcingHandler.RepublishEventsAsync();
    }
}
