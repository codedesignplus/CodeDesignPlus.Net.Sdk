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
    /// Appends a domain event to the event store.
    /// </summary>
    /// <typeparam name="TDomainEvent">The type of the domain event.</typeparam>
    /// <param name="event">The domain event to append.</param>
    /// <exception cref="ArgumentNullException">Thrown when the <paramref name="event"/> is null.</exception>
    Task AppendEventAsync<TDomainEvent>(TDomainEvent @event)
        where TDomainEvent : IDomainEvent;

    /// <summary>
    /// Appends a domain event, with associated metadata, to the event store.
    /// </summary>
    /// <typeparam name="TDomainEvent">The type of the domain event.</typeparam>
    /// <typeparam name="TMetadata">The type of metadata associated with the domain event.</typeparam>
    /// <param name="event">The domain event to append.</param>
    /// <exception cref="ArgumentNullException">Thrown when the <paramref name="event"/> is null.</exception>
    Task AppendEventAsync<TDomainEvent, TMetadata>(TDomainEvent @event)
        where TDomainEvent : IDomainEvent<TMetadata>
        where TMetadata : class;

    /// <summary>
    /// Gets the version number of an aggregate from the event store.
    /// </summary>
    /// <param name="aggregateId">The unique identifier of the aggregate.</param>
    /// <returns>The aggregate version number.</returns>
    /// <exception cref="ArgumentException">Thrown when the provided <paramref name="aggregateId"/> is an empty GUID.</exception>
    Task<int> GetAggregateVersionAsync(Guid aggregateId);

    /// <summary>
    /// Retrieves the position of the latest event in the global stream.
    /// </summary>
    /// <returns>The position of the last event in the global stream.</returns>
    Task<long> GetEventStreamPositionAsync();

    /// <summary>
    /// Loads events for a specific aggregate from the event store.
    /// </summary>
    /// <param name="aggregateId">The unique identifier of the aggregate.</param>
    /// <returns>A collection of domain events for the aggregate.</returns>
    /// <exception cref="ArgumentException">Thrown when the provided <paramref name="aggregateId"/> is an empty GUID.</exception>
    Task<IEnumerable<IDomainEvent>> LoadEventsForAggregateAsync(Guid aggregateId);

    /// <summary>
    /// Loads events for a specific aggregate, with associated metadata, from the event store.
    /// </summary>
    /// <typeparam name="TDomainEvent">The type of the domain event.</typeparam>
    /// <typeparam name="TMetadata">The type of metadata associated with the domain event.</typeparam>
    /// <param name="aggregateId">The unique identifier of the aggregate.</param>
    /// <returns>A collection of domain events for the aggregate.</returns>
    /// <exception cref="ArgumentException">Thrown when the provided <paramref name="aggregateId"/> is an empty GUID.</exception>
    Task<IEnumerable<TDomainEvent>> LoadEventsForAggregateAsync<TDomainEvent, TMetadata>(Guid aggregateId)
        where TDomainEvent : IDomainEvent<TMetadata>
        where TMetadata : class;
    /// <summary>
    /// Loads events starting from a specified position in the global stream.
    /// </summary>
    /// <typeparam name="TDomainEvent">The type of the domain event.</typeparam>
    /// <param name="position">The position to start reading from.</param>
    /// <returns>A collection of domain events.</returns>
    Task<IEnumerable<TDomainEvent>> LoadEventsFromPositionAsync<TDomainEvent>(long position)
        where TDomainEvent : IDomainEvent;

    /// <summary>
    /// Loads events, with associated metadata, starting from a specified position in the global stream.
    /// </summary>
    /// <typeparam name="TDomainEvent">The type of the domain event.</typeparam>
    /// <typeparam name="TMetadata">The type of metadata associated with the domain event.</typeparam>
    /// <param name="position">The position to start reading from.</param>
    /// <returns>A collection of domain events.</returns>
    Task<IEnumerable<TDomainEvent>> LoadEventsFromPositionAsync<TDomainEvent, TMetadata>(long position)
        where TDomainEvent : IDomainEvent<TMetadata>
        where TMetadata : class;

    /// <summary>
    /// Loads a snapshot for a specific aggregate from the event store.
    /// </summary>
    /// <typeparam name="TAggregate">The type of the aggregate.</typeparam>
    /// <typeparam name="TKey">The type of the aggregate's identifier.</typeparam>
    /// <param name="aggregateId">The unique identifier of the aggregate.</param>
    /// <returns>The latest snapshot of the aggregate, or default value if not found.</returns>
    /// <exception cref="ArgumentException">Thrown when the provided <paramref name="aggregateId"/> is an empty GUID.</exception>
    Task<TAggregate> LoadSnapshotForAggregateAsync<TAggregate, TKey>(Guid aggregateId)
        where TAggregate : IAggregateRoot<TKey>;

    /// <summary>
    /// Loads a snapshot for a specific aggregate, with associated metadata, from the event store.
    /// </summary>
    /// <typeparam name="TAggregate">The type of the aggregate.</typeparam>
    /// <typeparam name="TKey">The type of the aggregate's identifier.</typeparam>
    /// <typeparam name="TUserKey">The type of the associated user's identifier.</typeparam>
    /// <param name="aggregateId">The unique identifier of the aggregate.</param>
    /// <returns>The latest snapshot of the aggregate, or default value if not found.</returns>
    /// <exception cref="ArgumentException">Thrown when the provided <paramref name="aggregateId"/> is an empty GUID.</exception>
    Task<TAggregate> LoadSnapshotForAggregateAsync<TAggregate, TKey, TUserKey>(Guid aggregateId)
        where TAggregate : IAggregateRoot<TKey, TUserKey>;

    /// <summary>
    /// Saves a snapshot of a specific aggregate to the event store.
    /// </summary>
    /// <typeparam name="TAggregate">The type of the aggregate.</typeparam>
    /// <typeparam name="TKey">The type of the aggregate's identifier.</typeparam>
    /// <param name="aggregate">The current state of the aggregate to be saved as a snapshot.</param>
    /// <exception cref="ArgumentNullException">Thrown when the <paramref name="aggregate"/> is null.</exception>
    Task SaveSnapshotAsync<TAggregate, TKey>(TAggregate aggregate)
        where TAggregate : IAggregateRoot<TKey>;

    /// <summary>
    /// Saves a snapshot of a specific aggregate, with associated metadata, to the event store.
    /// </summary>
    /// <typeparam name="TAggregate">The type of the aggregate.</typeparam>
    /// <typeparam name="TKey">The type of the aggregate's identifier.</typeparam>
    /// <typeparam name="TUserKey">The type of the associated user's identifier.</typeparam>
    /// <param name="aggregate">The current state of the aggregate to be saved as a snapshot.</param>
    /// <exception cref="ArgumentNullException">Thrown when the <paramref name="aggregate"/> is null.</exception>
    Task SaveSnapshotAsync<TAggregate, TKey, TUserKey>(TAggregate aggregate)
        where TAggregate : IAggregateRoot<TKey, TUserKey>;

    /// <summary>
    /// Searches for events in the event store by the specified stream name.
    /// </summary>
    /// <param name="streamName">The name of the stream to search in.</param>
    /// <returns>A collection of domain events that match the search criteria.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the <paramref name="streamName"/> is null or empty.</exception>
    Task<IEnumerable<IDomainEvent>> SearchEventsByStreamAsync(string streamName);

    /// <summary>
    /// Searches for events in the event store by the type of the domain event.
    /// </summary>
    /// <typeparam name="TDomainEvent">The type of the domain event to search for.</typeparam>
    /// <returns>A collection of domain events that match the specified type.</returns>
    Task<IEnumerable<TDomainEvent>> SearchEventsByEventTypeAsync<TDomainEvent>()
        where TDomainEvent : IDomainEvent;

    /// <summary>
    /// Searches for events in the event store by the specified category.
    /// </summary>
    /// <typeparam name="TDomainEvent">The type of the domain event to search for.</typeparam>
    /// <param name="category">The category to search in.</param>
    /// <returns>A collection of domain events that match the search criteria.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the <paramref name="category"/> is null or empty.</exception>
    Task<IEnumerable<TDomainEvent>> SearchEventsByCategoryAsync<TDomainEvent>(string category)
        where TDomainEvent : IDomainEvent;
}
