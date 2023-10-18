using CQRS.Core.Events;

namespace Post.Common.Events;

public sealed class CommentRemovedEvent : BaseEvent
{
    public Guid CommentId { get; set; }

    public CommentRemovedEvent() : base(nameof(CommentRemovedEvent)) {
    }
}