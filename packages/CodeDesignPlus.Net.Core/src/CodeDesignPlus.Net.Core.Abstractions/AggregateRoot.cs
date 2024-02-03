
namespace CodeDesignPlus.Net.Core.Abstractions;

/// <summary>
/// Represents an aggregate root.
/// </summary>
public abstract class AggregateRoot : IAggregateRoot
{
    /// <summary>
    /// Initializes a new instance of <see cref="AggregateRoot"/>.
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// The list of events that have occurred in the aggregate root but have not yet been committed to the event store.
    /// </summary>
    protected List<IDomainEvent> DomainEvents = [];

    /// <summary>
    /// Default constructor to rehydrate the aggregate root.
    /// </summary>
    protected AggregateRoot() { }

    /// <summary>
    /// Initializes a new instance of <see cref="AggregateRoot"/>.
    /// </summary>
    protected AggregateRoot(Guid id)
    {
        this.Id = id;
    }

    /// <summary>
    /// Apply the changes that occur in the aggregate root.
    /// </summary>
    /// <param name="event">The domain event to apply the changes.</param>
    public virtual void AddEvent(IDomainEvent @event)
    {
        this.DomainEvents.Add(@event);
    }

    /// <summary>
    /// Get and clear the events that have occurred in the aggregate root.
    /// </summary>
    /// <returns>The list of events that have occurred in the aggregate root.</returns>
    public virtual IReadOnlyList<IDomainEvent> GetAndClearEvents()
    {
        var domainEvents = this.DomainEvents;

        this.DomainEvents = [];

        return domainEvents.AsReadOnly();
    }
}
