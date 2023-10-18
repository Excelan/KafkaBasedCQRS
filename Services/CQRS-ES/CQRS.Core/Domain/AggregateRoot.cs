namespace CQRS.Core.Domain;

using CQRS.Core.Events;

public abstract class AggregateRoot
{
    private static string _applyMethodName= "Apply";
    private readonly List<BaseEvent> _changes = new();

    protected Guid _id;

    public Guid Id { get => _id; }
    public int Version { get; set; } = -1;

    private void RegisterUncommitedChange(BaseEvent @event) => _changes.Add(@event);
    private void MarkChangesAsCommited() => _changes.Clear();

    private void ApplyChange(BaseEvent @event) {
        var aggregateType = GetType();
        var eventType = @event.GetType();
        var method = aggregateType.GetMethod(_applyMethodName, new[] { eventType });
        if (method is null) {
            throw new ArgumentException($"The {aggregateType.Name} " +
                $"contains no ${_applyMethodName} " +
                $"that takes {eventType.Name} arguments");
        }
        method.Invoke(this, new object[] { @event });
    }

    protected void RaiseEvent(BaseEvent @event) {
        ApplyChange(@event);
        RegisterUncommitedChange(@event);
    }

    public IEnumerable<BaseEvent> GetUncommitedChanges => _changes;

    public void ReplyEvents(IEnumerable<BaseEvent> events) {
        foreach (var @event in events) {
            ApplyChange(@event);
        }
    }
}
