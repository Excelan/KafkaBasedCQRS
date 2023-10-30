﻿using CQRS.Core.Events;

namespace Post.Common.Events;

public class MessageUpdatedEvent : BaseEvent
{
    public required string Message { get; set; }

    public MessageUpdatedEvent() : base(nameof(MessageUpdatedEvent)) {
    }
}