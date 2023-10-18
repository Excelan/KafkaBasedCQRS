using CQRS.Core.Domain;
using CQRS.Core.Handlers;
using CQRS.Core.Infrastructure;
using Post.Cmd.Domain.Aggregates;

namespace Post.Cmd.Infrastructure.Handlers
{
    public class EventSourcingHandler : IEventSourcingHandler<PostAggregate>
    {
        private readonly IEventStore _eventStore;

        public EventSourcingHandler(IEventStore eventStore)
        {
            _eventStore = eventStore;
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
    }
}
