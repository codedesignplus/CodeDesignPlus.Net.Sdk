namespace CodeDesignPlus.Net.Event.Sourcing.Abstractions;

using CodeDesignPlus.Net.Core.Abstractions;
using System;
using System.Collections.Generic;

/// <summary>
/// Provides operations for interacting with an event store in an Event Sourcing system.
/// </summary>
public interface IEventSourcingService
{
    /// <summary>
    /// Appends a new event to the store.
    /// </summary>
    /// <param name="event">The domain event to append.</param>
    void AppendEvent(IDomainEvent @event);
    /// <summary>
    /// Appends a new event to the store.
    /// </summary>
    /// <param name="event">The domain event to append.</param>
    void AppendEvent<TMetadata>(IDomainEvent<TMetadata> @event) where TMetadata: class;

    /// <summary>
    /// Loads all events for a specific aggregate.
    /// </summary>
    /// <param name="aggregateId">The unique identifier of the aggregate.</param>
    /// <returns>A sequence of domain events for the aggregate.</returns>
    IEnumerable<IDomainEvent> LoadEventsForAggregate(Guid aggregateId);

    /// <summary>
    /// Loads all events for a specific aggregate.
    /// </summary>
    /// <param name="aggregateId">The unique identifier of the aggregate.</param>
    /// <returns>A sequence of domain events for the aggregate.</returns>
    IEnumerable<IDomainEvent<TMetadata>> LoadEventsForAggregate<TMetadata>(Guid aggregateId) where TMetadata: class;

    /// <summary>
    /// Saves a snapshot of the current state of an aggregate.
    /// </summary>
    /// <param name="aggregate">The aggregate to create a snapshot for.</param>
    void SaveSnapshot<TAggregate>(TAggregate aggregate) where TAggregate : IAggregateRoot;

    /// <summary>
    /// Saves a snapshot of the current state of an aggregate.
    /// </summary>
    /// <param name="aggregate">The aggregate to create a snapshot for.</param>
    void SaveSnapshot<TAggregate, TKey, TUserKey>(TAggregate aggregate) where TAggregate : IAggregateRoot<TKey, TUserKey>;

    /// <summary>
    /// Loads the most recent snapshot for a specific aggregate.
    /// </summary>
    /// <param name="aggregateId">The unique identifier of the aggregate.</param>
    /// <returns>The most recent snapshot of the aggregate.</returns>
    TAggregate LoadSnapshotForAggregate<TAggregate>(Guid aggregateId) where TAggregate : IAggregateRoot;
    
    /// <summary>
    /// Loads the most recent snapshot for a specific aggregate.
    /// </summary>
    /// <param name="aggregateId">The unique identifier of the aggregate.</param>
    /// <returns>The most recent snapshot of the aggregate.</returns>
    TAggregate LoadSnapshotForAggregate<TAggregate, TKey, TUserKey>(Guid aggregateId) where TAggregate : IAggregateRoot<TKey, TUserKey>;

    /// <summary>
    /// Gets the current position in the event stream.
    /// </summary>
    /// <returns>The current position in the event stream.</returns>
    long GetEventStreamPosition();

    /// <summary>
    /// Loads events starting from a specific position in the event stream.
    /// </summary>
    /// <param name="position">The position in the event stream to start loading from.</param>
    /// <returns>A sequence of domain events starting from the specified position.</returns>
    IEnumerable<IDomainEvent> LoadEventsFromPosition(long position);
    
    /// <summary>
    /// Loads events starting from a specific position in the event stream.
    /// </summary>
    /// <param name="position">The position in the event stream to start loading from.</param>
    /// <returns>A sequence of domain events starting from the specified position.</returns>
    IEnumerable<IDomainEvent<TMetadata>> LoadEventsFromPosition<TMetadata>(long position) where TMetadata : class;

    /// <summary>
    /// Searches for events based on specific criteria.
    /// </summary>
    /// <param name="criteria">The criteria to search for.</param>
    /// <returns>A sequence of domain events that match the search criteria.</returns>
    // Note: The type of 'criteria' can be modified based on your specific needs
    IEnumerable<IDomainEvent> SearchEvents(object criteria);
    
    /// <summary>
    /// Searches for events based on specific criteria.
    /// </summary>
    /// <param name="criteria">The criteria to search for.</param>
    /// <returns>A sequence of domain events that match the search criteria.</returns>
    // Note: The type of 'criteria' can be modified based on your specific needs
    IEnumerable<IDomainEvent<TMetadata>> SearchEvents<TMetadata>(object criteria) where TMetadata : class;

    /// <summary>
    /// Gets the current version of a specific aggregate.
    /// </summary>
    /// <param name="aggregateId">The unique identifier of the aggregate.</param>
    /// <returns>The current version of the aggregate.</returns>
    int GetAggregateVersion(Guid aggregateId);
}
