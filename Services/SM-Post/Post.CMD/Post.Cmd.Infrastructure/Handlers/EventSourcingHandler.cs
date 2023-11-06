namespace Post.Cmd.Infrastructure.Handlers;

using CQRS.Core.Domain;
using CQRS.Core.Handlers;
using CQRS.Core.Infrastructure;
using CQRS.Core.Producesrs;
using Post.Cmd.Domain.Aggregates;

public class EventSourcingHandler : IEventSourcingHandler<PostAggregate>
{
    private readonly IEventStore _eventStore;
    private readonly IEventProducer _eventProducer;

    public EventSourcingHandler(IEventStore eventStore, IEventProducer eventProducer) {
        _eventStore = eventStore;
        _eventProducer = eventProducer;
    }

    public async Task<PostAggregate> GetByIdAsync(Guid agregateId) {
        var aggregate = new PostAggregate();
        var events = await _eventStore.GetEventsAsync(agregateId);
        if (events==null || !events.Any()) {
            return aggregate;
        }
        aggregate.ReplyEvents(events);
        aggregate.Version = events.Max(x => x.Version);

        return aggregate;
    }

    public async Task SaveAsync(AggregateRoot aggregate) {
        await _eventStore.SaveEventsAsync(aggregate.Id, aggregate.GetUncommitedChanges(), aggregate.Version);
        aggregate.MarkChangesAsCommited();

    }

    public async Task RepublishEventsAsync() {
        var aggregateIds = await _eventStore.GetAggregateIdsAsync();
        if (aggregateIds is null || !aggregateIds.Any()) {
            return;
        }
        foreach (var aggregateId in aggregateIds) {
            var aggregate = await GetByIdAsync(aggregateId);
            if (aggregate is null || !aggregate.Active) {
                continue;
            }
            var events = await _eventStore.GetEventsAsync(aggregateId);
            foreach (var @event in events) {
                var topic = Environment.GetEnvironmentVariable("KAFKA_TOPIC");
                await _eventProducer.ProduceAsync(topic, @event);
            }
        }
    }
}
