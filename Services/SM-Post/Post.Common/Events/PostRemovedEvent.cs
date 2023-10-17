using CQRS.Core.Events;

namespace Post.Common.Events;

public sealed class PostRemovedEvent : BaseEvent
{
    public PostRemovedEvent() : base(nameof(PostRemovedEvent)) {
    }
}