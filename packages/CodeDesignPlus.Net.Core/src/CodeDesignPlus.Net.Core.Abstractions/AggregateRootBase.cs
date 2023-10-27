
namespace CodeDesignPlus.Net.Core.Abstractions;

/// <summary>
/// Represents a base implementation of an aggregate root in a domain-driven design context.
/// Aggregate roots are the primary entry points to aggregates, a cluster of domain objects 
/// that are treated as a single unit for data changes.
/// </summary>
public abstract class AggregateRootBase : IAggregateRoot
{
    private readonly List<IDomainEvent> uncommittedEvents = new();

    /// <summary>
    /// Gets or sets the version of the aggregate root. Useful for optimistic concurrency control.
    /// </summary>
    public int Version { get; set; }

    /// <summary>
    /// Adds the specified domain event to the list of uncommitted events.
    /// </summary>
    /// <param name="event">The domain event to be added.</param>
    public void ApplyEvent(IDomainEvent @event)
    {
        this.uncommittedEvents.Add(@event);
    }

    /// <summary>
    /// Retrieves a read-only list of uncommitted events.
    /// </summary>
    /// <returns>A sequence of uncommitted domain events.</returns>
    public IEnumerable<IDomainEvent> GetUncommittedEvents() => this.uncommittedEvents.AsReadOnly();

    /// <summary>
    /// Clears all uncommitted events, typically called after these events have been persisted.
    /// </summary>
    public void ClearUncommittedEvents() => this.uncommittedEvents.Clear();
}

/// <summary>
/// Represents a base implementation of an aggregate root in a domain-driven design context.
/// Aggregate roots are the primary entry points to aggregates, a cluster of domain objects 
/// that are treated as a single unit for data changes.
/// </summary>
/// <typeparam name="TKey">The type of the primary key for this aggregate root.</typeparam>
/// <typeparam name="TUserKey">The type of the user key for this aggregate root.</typeparam>
public abstract class AggregateRootBase<TKey, TUserKey> : AggregateRootBase, IAggregateRoot<TKey, TUserKey>
{
    /// <summary>
    /// Gets or sets the unique identifier for the aggregate root.
    /// </summary>
    public TKey Id { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the aggregate root is active.
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the user who created the aggregate root.
    /// </summary>
    public TUserKey IdUserCreator { get; set; }

    /// <summary>
    /// Gets or sets the creation date of the aggregate root.
    /// </summary>
    public DateTime DateCreated { get; set; }
}