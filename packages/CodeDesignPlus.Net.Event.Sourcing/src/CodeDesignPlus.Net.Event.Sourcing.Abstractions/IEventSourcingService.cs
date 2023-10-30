namespace CodeDesignPlus.Net.Event.Sourcing.Abstractions;

using CodeDesignPlus.Net.Core.Abstractions;
using System;
using System.Collections.Generic;

/// <summary>
/// Provides operations for interacting with an event store in an Event Sourcing system.
/// </summary>
public interface IEventSourcingService<TUserKey>
{
    Task AppendEventAsync<TDomainEvent>(TDomainEvent @event, Metadata<TUserKey> metadata)
        where TDomainEvent : IDomainEvent;

    Task<long> GetAggregateVersionAsync(Guid aggregateId);

    Task<long> GetEventStreamPositionAsync();

    Task<IEnumerable<(IDomainEvent, Metadata<TUserKey>)>> LoadEventsForAggregateAsync(Guid aggregateId);

    Task<IEnumerable<(IDomainEvent, Metadata<TUserKey>)>> LoadEventsFromPositionAsync(long position);

    Task<TAggregate> LoadSnapshotForAggregateAsync<TAggregate>(Guid aggregateId)
        where TAggregate : IAggregateRoot<TUserKey>;

    Task SaveSnapshotAsync<TAggregate>(TAggregate aggregate)
        where TAggregate : IAggregateRoot<TUserKey>;

    Task<IEnumerable<(IDomainEvent, Metadata<TUserKey>)>> SearchEventsByStreamAsync(string streamName);

    Task<IEnumerable<(TDomainEvent, Metadata<TUserKey>)>> SearchEventsByEventTypeAsync<TDomainEvent>()
        where TDomainEvent : IDomainEvent;

    Task<IEnumerable<(TDomainEvent, Metadata<TUserKey>)>> SearchEventsByCategoryAsync<TDomainEvent>(string category)
        where TDomainEvent : IDomainEvent;
}
