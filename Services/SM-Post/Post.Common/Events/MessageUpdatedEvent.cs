using CQRS.Core.Events;

namespace Post.Common.Events;

public sealed class MessageUpdatedEvent : BaseEvent
{
    public string? Message { get; set; }

    public MessageUpdatedEvent() : base(nameof(MessageUpdatedEvent)) {
    }
}