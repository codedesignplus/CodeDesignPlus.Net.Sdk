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
    /// Get the number of events that have been stored for the specified aggregate.
    /// </summary>
    /// <param name="category">The category of the aggregate root.</param>
    /// <param name="aggregateId">The identifier of the aggregate root.</param>
    /// <returns>The number of events that have been stored for the specified aggregate.</returns>
    Task<long> CountEventsAsync(string category, Guid aggregateId);
    /// <summary>
    /// Load the aggregate root from the event store.
    /// </summary>
    /// <typeparam name="TAggregate">The type of the aggregate root.</typeparam>
    /// <param name="category">The category of the aggregate root.</param>
    /// <param name="aggregateId">The identifier of the aggregate root.</param>
    /// <returns>The aggregate root loaded from the event store.</returns>
    Task<TAggregate> LoadSnapshotAsync<TAggregate>(string category, Guid aggregateId) where TAggregate : IAggregateRoot;
    /// <summary>
    /// Save a snapshot of the aggregate root to the event store.
    /// </summary>
    /// <typeparam name="TAggregate">The type of the aggregate root.</typeparam>
    /// <param name="aggregate">The aggregate root to save a snapshot of.</param>
    /// <returns>A task that represents the asynchronous save operation.</returns>
    Task SaveSnapshotAsync<TAggregate>(TAggregate aggregate) where TAggregate : IAggregateRoot;
    /// <summary>
    /// Get the version of the aggregate root.
    /// </summary>
    /// <param name="category">The category of the aggregate root.</param>
    /// <param name="aggregateId">The identifier of the aggregate root.</param>
    /// <returns>The version of the aggregate root.</returns>
    Task<long> GetVersionAsync(string category, Guid aggregateId);
    /// <summary>
    /// Append a domain event to the event store.
    /// </summary>
    /// <typeparam name="TDomainEvent">The type of the domain event.</typeparam>
    /// <param name="event">The domain event to append to the event store.</param>
    /// <param name="metadata">The metadata of the domain event.</param>
    /// <returns>A task that represents the asynchronous append operation.</returns>
    Task AppendEventAsync<TDomainEvent>(TDomainEvent @event, Metadata metadata) where TDomainEvent : IDomainEvent;
    /// <summary>
    /// Load the events for the specified aggregate from the event store.
    /// </summary>
    /// <param name="category">The category of the aggregate root.</param>
    /// <param name="aggregateId">The identifier of the aggregate root.</param>
    /// <returns>The events for the specified aggregate loaded from the event store.</returns>
    Task<IEnumerable<(IDomainEvent, Metadata)>> LoadEventsAsync(string category, Guid aggregateId);
    /// <summary>
    /// Search the events for the specified stream from the event store.
    /// </summary>
    /// <param name="streamName">The name of the stream.</param>
    /// <returns>The events for the specified stream loaded from the event store.</returns>
    Task<IEnumerable<(IDomainEvent, Metadata)>> SearchEventsAsync(string streamName);
    /// <summary>
    /// Search the events for the specified stream from the event store.
    /// </summary>
    /// <typeparam name="TDomainEvent">The type of the domain event.</typeparam>
    /// <returns>The events for the specified stream loaded from the event store.</returns>
    Task<IEnumerable<(TDomainEvent, Metadata)>> SearchEventsAsync<TDomainEvent>() where TDomainEvent : IDomainEvent;
    /// <summary>
    /// Search the events for the specified category from the event store.
    /// </summary>
    /// <typeparam name="TDomainEvent">The type of the domain event.</typeparam>
    /// <param name="category">The category of the aggregate root.</param>
    /// <returns>The events for the specified category loaded from the event store.</returns>
    Task<IEnumerable<(TDomainEvent, Metadata)>> SearchEventsAsync<TDomainEvent>(string category) where TDomainEvent : IDomainEvent;
}
