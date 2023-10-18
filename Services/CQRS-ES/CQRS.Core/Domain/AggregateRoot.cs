namespace CQRS.Core.Domain;

using CQRS.Core.Events;

public abstract class AggregateRoot
{
    private static readonly string _applyMethodName= "Apply";
    private readonly List<BaseEvent> _changes = new();

    protected Guid _id;

    public Guid Id { get => _id; }
    public int Version { get; set; } = -1;

    private void RegisterUncommitedChange(BaseEvent @event) => _changes.Add(@event);

    private void ApplyChange(BaseEvent @event) {
        var eventType = @event.GetType();
        var method = GetType().GetMethod(_applyMethodName, new[] { eventType }) 
            ?? throw new ArgumentException($"Can not apply changes for the event of type {eventType.Name}");
        method.Invoke(this, new object[] { @event });
    }

    protected void RaiseEvent(BaseEvent @event) {
        ApplyChange(@event);
        RegisterUncommitedChange(@event);
    }

    public void ReplyEvents(IEnumerable<BaseEvent> events) {
        foreach (var @event in events) {
            ApplyChange(@event);
        }
    }

    public IEnumerable<BaseEvent> GetUncommitedChanges() => _changes;

    public void MarkChangesAsCommited() => _changes.Clear();
}
