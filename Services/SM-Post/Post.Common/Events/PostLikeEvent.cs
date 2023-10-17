using CQRS.Core.Events;

namespace Post.Common.Events;

public sealed class PostLikeEvent : BaseEvent
{
    public PostLikeEvent() : base(nameof(PostLikeEvent)) {
    }
}