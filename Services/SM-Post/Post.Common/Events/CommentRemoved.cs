using CQRS.Core.Events;

namespace Post.Common.Events;

public sealed class CommentRemoved : BaseEvent
{
    public Guid CommentId { get; set; }

    public CommentRemoved() : base(nameof(CommentRemoved)) {
    }
}