using CQRS.Core.Events;

namespace Post.Common.Events;

public sealed class CommentUpdatedEvent : BaseEvent
{
    public Guid CommentId { get; set; }
    public required string Comment { get; set; }
    public required string Username { get; set; }
    public DateTime EditDate { get; set; }

    public CommentUpdatedEvent() : base(nameof(CommentUpdatedEvent)) {
    }
}