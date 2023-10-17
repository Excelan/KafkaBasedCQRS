namespace CQRS.Core.Events;

using CQRS.Core.Messages;

public abstract class BaseEvent : Message
{
    public string Type { get; }
    public int Version { get; set; }

    protected BaseEvent(string Type) {
        this.Type = Type;
    }
}
