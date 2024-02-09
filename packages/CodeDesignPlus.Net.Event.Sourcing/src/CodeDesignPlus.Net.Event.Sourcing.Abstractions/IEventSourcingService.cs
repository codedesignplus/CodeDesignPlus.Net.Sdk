namespace CodeDesignPlus.Net.Event.Sourcing.Abstractions;

using System;
using System.Collections.Generic;
using CodeDesignPlus.Net.Core.Abstractions;

/// <summary>
/// Provides operations for interacting with an event store in an Event Sourcing system.
/// </summary>
public interface IEventSourcingService
{
    /// <summary>
    /// Counts the number of events for a specific category and aggregate ID.
    /// </summary>
    /// <param name="category">The category of the events.</param>
    /// <param name="aggregateId">The aggregate ID of the events.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the number of events.</returns>
    Task<long> CountEventsAsync(string category, Guid aggregateId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Appends an event to the event store.
    /// </summary>
    /// <typeparam name="TDomainEvent">The type of the event.</typeparam>
    /// <param name="category">The category of the events.</param>
    /// <param name="event">The event to append.</param>
    /// <param name="metadata">The metadata associated with the event.</param>
    /// <param name="version">The version of the event store.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task AppendEventAsync<TDomainEvent>(string category, TDomainEvent @event, long? version = null, CancellationToken cancellationToken = default) 
        where TDomainEvent : IDomainEvent;

    /// <summary>
    /// Gets the version of the event store.
    /// </summary>
    /// <param name="category">The category of the events.</param>
    /// <param name="aggregateId">The aggregate ID of the events.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the version of the event store.</returns>
    Task<long> GetVersionAsync(string category, Guid aggregateId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Loads the events for a specific category and aggregate ID.
    /// </summary>
    /// <param name="category">The category of the events.</param>
    /// <param name="aggregateId">The aggregate ID of the events.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the loaded events.</returns>
    Task<IEnumerable<IDomainEvent>> LoadEventsAsync(string category, Guid aggregateId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Loads the snapshot for a specific category and aggregate ID.
    /// </summary>
    /// <param name="category">The category of the snapshot.</param>
    /// <param name="aggregateId">The aggregate ID of the snapshot.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the loaded snapshot.</returns>
    Task<TAggregate> LoadSnapshotAsync<TAggregate>(string category, Guid aggregateId, CancellationToken cancellationToken = default)
        where TAggregate : IAggregateRoot;

    /// <summary>
    /// Saves the snapshot for a specific category and aggregate ID.
    /// </summary>
    /// <param name="category">The category of the snapshot.</param>
    /// <param name="aggregateId">The aggregate ID of the snapshot.</param>
    /// <param name="snapshot">The snapshot to save.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task SaveSnapshotAsync<TAggregate>(TAggregate aggregate, CancellationToken cancellationToken = default)
        where TAggregate : IAggregateRoot;

    /// <summary>
    /// Searches for events that match the specified criteria.
    /// </summary>
    /// <param name="streamName">The name of the stream.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the events that match the criteria.</returns>
    Task<IEnumerable<IDomainEvent>> SearchEventsAsync(string streamName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Searches for events that match the specified criteria.
    /// </summary>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the events that match the criteria.</returns>
    Task<IEnumerable<TDomainEvent>> SearchEventsAsync<TDomainEvent>(CancellationToken cancellationToken = default)
        where TDomainEvent : IDomainEvent;

    /// <summary>
    /// Searches for events that match the specified criteria.
    /// </summary>
    /// <param name="category">The category of the events.</param>
    /// <param name="criteria">The criteria to match.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the events that match the criteria.</returns>
    Task<IEnumerable<TDomainEvent>> SearchEventsAsync<TDomainEvent>(string category, CancellationToken cancellationToken = default)
       where TDomainEvent : IDomainEvent;
}
