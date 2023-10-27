namespace CodeDesignPlus.Net.Core.Abstractions;


/// <summary>
/// Defines a contract for domain events within the system. Domain events capture state 
/// changes and are used as part of the Event Sourcing pattern.
/// </summary>
public interface IDomainEvent
{
    /// <summary>
    /// Gets the unique identifier for the event.
    /// </summary>
    Guid EventId { get; }

    /// <summary>
    /// Gets the identifier for the Aggregate associated with this event.
    /// </summary>
    Guid AggregateId { get; }

    /// <summary>
    /// Gets the specific version for the event.
    /// This is useful for handling changes in the event structure over time.
    /// </summary>
    int Version { get; }

    /// <summary>
    /// Gets the name of the event type.
    /// </summary>
    string EventType { get; }

    /// <summary>
    /// Gets the date and time when the event occurred.
    /// </summary>
    DateTime OccurredOn { get; }
}

/// <summary>
/// Defines a contract for domain events within the system. Domain events capture state 
/// changes and are used as part of the Event Sourcing pattern.
/// </summary>
/// <typeparam name="TMetadata">The type of metadata associated with the event.</typeparam>
public interface IDomainEvent<TMetadata> : IDomainEvent
    where TMetadata : class
{
    /// <summary>
    /// Gets the metadata associated with the event. Metadata provides 
    /// additional context or details about the event.
    /// </summary>
    TMetadata Metadata { get; }
}

/// <summary>
/// Represents a lightweight domain event. Lightweight events, often referred to as thin events, 
/// typically carry the minimal information necessary to describe a change in the system. 
/// They often contain only the identifiers or values that have changed, without the full context.
/// </summary>
/// <typeparam name="TMetadata">The type of metadata associated with the event.</typeparam>
public interface IThinDomainEvent : IDomainEvent
{

}

/// <summary>
/// Represents a lightweight domain event. Lightweight events, often referred to as thin events, 
/// typically carry the minimal information necessary to describe a change in the system. 
/// They often contain only the identifiers or values that have changed, without the full context.
/// </summary>
/// <typeparam name="TMetadata">The type of metadata associated with the event.</typeparam>
public interface IThinDomainEvent<TMetadata> : IDomainEvent<TMetadata>, IThinDomainEvent
    where TMetadata : class
{

}

/// <summary>
/// Represents a rich domain event. Rich events carry more information than thin events, 
/// providing a greater context about a change. This includes the state of the aggregate 
/// before and after the event occurred.
/// </summary>
/// <typeparam name="TAggregate">The type of the aggregate root associated with this event.</typeparam>
/// <typeparam name="TMetadata">The type of metadata associated with the event.</typeparam>
public interface IRichDomainEvent<TAggregate> : IDomainEvent
    where TAggregate : IAggregateRoot
{
    /// <summary>
    /// Gets the state of the aggregate before the event occurred.
    /// </summary>
    TAggregate PreviousState { get; }

    /// <summary>
    /// Gets the state of the aggregate after the event occurred.
    /// </summary>
    TAggregate CurrentState { get; }
}

/// <summary>
/// Represents a rich domain event. Rich events carry more information than thin events, 
/// providing a greater context about a change. This includes the state of the aggregate 
/// before and after the event occurred.
/// </summary>
/// <typeparam name="TAggregate">The type of the aggregate root associated with this event.</typeparam>
/// <typeparam name="TMetadata">The type of metadata associated with the event.</typeparam>
public interface IRichDomainEvent<TAggregate, TMetadata> : IDomainEvent<TMetadata>, IRichDomainEvent<TAggregate>
    where TAggregate : IAggregateRoot
    where TMetadata : class
{
    
}