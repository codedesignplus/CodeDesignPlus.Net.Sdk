namespace CodeDesignPlus.Net.Core.Abstractions;

public abstract class DomainEventBase : IDomainEvent
{
    /// <summary>
    /// Provides a unique identifier for the domain event.
    /// </summary>
    public Guid EventId { get; private set; } = Guid.NewGuid();

    /// <summary>
    /// Provides the unique identifier of the aggregate root associated with this domain event.
    /// </summary>
    public Guid AggregateId { get; protected set; }

    /// <summary>
    /// Represents the version of the domain event. Useful for versioning events in scenarios where the event structure might change over time.
    /// </summary>
    public long Version { get; set; }

    /// <summary>
    /// Represents the name of the event type, derived from the actual runtime type of the event instance.
    /// </summary>
    public string EventType => GetType().Name;

    /// <summary>
    /// Indicates the date and time when the domain event was created or occurred.
    /// </summary>
    public DateTime OccurredOn { get; private set; } = DateTime.UtcNow;

    /// <summary>
    /// Initializes a new instance of the <see cref="DomainEventBase{TMetadata}"/> class with the specified aggregate ID and metadata.
    /// </summary>
    /// <param name="aggregateId">The unique identifier of the aggregate root associated with this domain event.</param>
    protected DomainEventBase(Guid aggregateId)
    {
        this.AggregateId = aggregateId;
    }
}

/// <summary>
/// Provides a base implementation for domain events. 
/// </summary>
/// <typeparam name="TMetadata">The type of metadata associated with the event.</typeparam>
public abstract class DomainEventBase<TMetadata> : DomainEventBase, IDomainEvent<TMetadata>
    where TMetadata : class
{
    /// <summary>
    /// Contains metadata associated with the domain event, providing additional context or details.
    /// </summary>
    public TMetadata Metadata { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DomainEventBase{TMetadata}"/> class with the specified aggregate ID and metadata.
    /// </summary>
    /// <param name="aggregateId">The unique identifier of the aggregate root associated with this domain event.</param>
    /// <param name="metadata">The metadata associated with the domain event.</param>
    protected DomainEventBase(Guid aggregateId, TMetadata metadata)
        : base(aggregateId)
    {
        this.AggregateId = aggregateId;
        this.Metadata = metadata;
    }

    /// <summary>
    /// Sets or updates the metadata associated with a domain event.
    /// </summary>
    /// <param name="metadata">The metadata to be set or updated.</param>
    /// <exception cref="ArgumentNullException">Thrown when the <paramref name="metadata"/> is null.</exception>
    public void SetMetadata(TMetadata metadata)
    {
        this.Metadata = metadata ?? throw new ArgumentNullException(nameof(metadata));
    }
}

/// <summary>
/// Provides a base implementation for lightweight domain events.
/// </summary>
/// <typeparam name="TMetadata">The type of metadata associated with the event.</typeparam>
public abstract class ThinDomainEventBase<TMetadata> : DomainEventBase<TMetadata>, IThinDomainEvent<TMetadata>
    where TMetadata : class
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ThinDomainEventBase{TMetadata}"/> class with the specified aggregate ID and metadata.
    /// </summary>
    /// <param name="aggregateId">The unique identifier of the aggregate root associated with this lightweight domain event.</param>
    /// <param name="metadata">The metadata associated with the lightweight domain event.</param>
    protected ThinDomainEventBase(Guid aggregateId, TMetadata metadata)
        : base(aggregateId, metadata)
    {
    }
}

/// <summary>
/// Provides a base implementation for rich domain events.
/// </summary>
/// <typeparam name="TAggregate">The type of the aggregate root associated with this event.</typeparam>
/// <typeparam name="TMetadata">The type of metadata associated with the event.</typeparam>
public abstract class RichDomainEventBase<TKey, TAggregate, TMetadata> : DomainEventBase<TMetadata>, IRichDomainEvent<TKey, TAggregate, TMetadata>
    where TAggregate : IAggregateRoot<TKey>
    where TMetadata : class
{
    /// <summary>
    /// Represents the state of the aggregate before the event occurred. 
    /// This provides context about the changes that led to this event.
    /// </summary>
    public TAggregate PreviousState { get; protected set; }

    /// <summary>
    /// Represents the state of the aggregate after the event occurred.
    /// This reflects the changes that were made due to this event.
    /// </summary>
    public TAggregate CurrentState { get; protected set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="RichDomainEventBase{TAggregate, TMetadata}"/> class 
    /// with the specified aggregate ID, metadata, previous state, and current state.
    /// </summary>
    /// <param name="aggregateId">The unique identifier of the aggregate root associated with this rich domain event.</param>
    /// <param name="metadata">The metadata associated with the rich domain event.</param>
    /// <param name="previousState">The state of the aggregate before the event occurred.</param>
    /// <param name="currentState">The state of the aggregate after the event occurred.</param>
    protected RichDomainEventBase(Guid aggregateId, TMetadata metadata, TAggregate previousState, TAggregate currentState)
        : base(aggregateId, metadata)
    {
        this.PreviousState = previousState;
        this.CurrentState = currentState;
    }
}