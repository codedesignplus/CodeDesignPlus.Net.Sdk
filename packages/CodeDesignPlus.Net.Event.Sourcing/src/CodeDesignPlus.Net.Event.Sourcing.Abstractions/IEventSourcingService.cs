namespace CodeDesignPlus.Net.Event.Sourcing.Abstractions;

using System;
using System.Collections.Generic;

/// <summary>
/// Provides operations for interacting with an event store in an Event Sourcing system.
/// </summary>
public interface IEventSourcingService<TUserKey>
{
    Task<TAggregate> LoadSnapshotAsync<TAggregate>(string category, Guid aggregateId) where TAggregate : IAggregateRoot<TUserKey>;
    Task SaveSnapshotAsync<TAggregate>(TAggregate aggregate) where TAggregate : IAggregateRoot<TUserKey>;
    Task<long> GetVersionAsync(string category, Guid aggregateId);
    Task AppendEventAsync<TDomainEvent>(TDomainEvent @event, Metadata<TUserKey> metadata) where TDomainEvent : IDomainEvent;
    Task<IEnumerable<(IDomainEvent, Metadata<TUserKey>)>> LoadEventsAsync(string category, Guid aggregateId);
    Task<IEnumerable<(IDomainEvent, Metadata<TUserKey>)>> SearchEventsAsync(string streamName);
    Task<IEnumerable<(TDomainEvent, Metadata<TUserKey>)>> SearchEventsAsync<TDomainEvent>() where TDomainEvent : IDomainEvent;
    Task<IEnumerable<(TDomainEvent, Metadata<TUserKey>)>> SearchEventsAsync<TDomainEvent>(string category) where TDomainEvent : IDomainEvent;
}
