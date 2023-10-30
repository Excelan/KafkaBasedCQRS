using CQRS.Core.Events;

namespace Post.Common.Events;

public sealed class PostCreatedEvent : BaseEvent
{
    public required string Author { get; set; }
    public required string Message { get; set; }
    public DateTime DatePosted { get; set; }

    public PostCreatedEvent() : base(nameof(PostCreatedEvent)) {
    }
}