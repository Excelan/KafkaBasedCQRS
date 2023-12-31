﻿namespace Post.Cmd.Infrastructure.Stores;

using CQRS.Core.Domain;
using CQRS.Core.Events;
using CQRS.Core.Exceptions;
using CQRS.Core.Infrastructure;
using CQRS.Core.Producesrs;
using Post.Cmd.Domain.Aggregates;

public class EventStore : IEventStore
{
    private readonly IEventStoreRepository _eventStoreRepository;
    private readonly IEventProducer _eventProducer;

    public EventStore(IEventStoreRepository eventStoreRepository, IEventProducer eventProducer) {
        _eventStoreRepository = eventStoreRepository;
        _eventProducer = eventProducer;
    }

    public async Task<List<BaseEvent>> GetEventsAsync(Guid aggregateId) {
        var eventStream = await _eventStoreRepository.FindAggregateById(aggregateId);
        if (eventStream == null || !eventStream.Any()) {
            throw new AggregateNotFoundException("Incorrect post ID provided");
        }
        return eventStream.OrderBy(x => x.Version).Select(x => x.EventData).ToList();
    }

    public async Task SaveEventsAsync(Guid aggregateId, IEnumerable<BaseEvent> events, int expectedVersion) {
        var eventStream = await _eventStoreRepository.FindAggregateById(aggregateId);
        if (expectedVersion != -1 && eventStream[^1].Version != expectedVersion) {
            throw new ConcurrencyException();
        }

        var version = expectedVersion;
        foreach (var @event in events) {
            version++;
            @event.Version = version;
            var eventType = @event.GetType().Name;
            var eventModel = new EventModel {
                Timestamp = DateTime.UtcNow,
                AggregateIdentifier = aggregateId,
                AggregateType = nameof(PostAggregate),
                Version = version,
                EventType = eventType,
                EventData = @event
            };

            // This region could be placed into a transaction.
            // However MongoDB supports transactions ONLY if its is run as a replica set.
            #region
            await _eventStoreRepository.SaveAsync(eventModel);
            var topic = Environment.GetEnvironmentVariable("KAFKA_TOPIC") ?? throw new ApplicationException("Environment variable [KAFKA_TOPIC] is not found.");
            await _eventProducer.ProduceAsync(topic, @event);
            #endregion

        }
    }

    public async Task<List<Guid>> GetAggregateIdsAsync() {
        var eventStream = await _eventStoreRepository.FindAllAsync();
        if (eventStream is null || !eventStream.Any()) {
            throw new ArgumentNullException(nameof(eventStream), "Could not retrieve events from the event store.");
        }
        return eventStream.Select(x => x.AggregateIdentifier).Distinct().ToList();
    }
}
