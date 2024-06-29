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
    /// Gets or sets the identifier of the user who created the record.
    /// </summary>
    public bool IsActive { get; set; }
    /// <summary>
    /// Get or sets the creatae at
    /// </summary>
    public long CreatedAt { get; set; }
    /// <summary>
    /// Get or sets the create by
    /// </summary>
    public Guid CreatedBy { get; set; }
    /// <summary>
    /// Get or sets the update at
    /// </summary>
    public long? UpdatedAt { get; set; }
    /// <summary>
    /// Get or sets the update by
    /// </summary>
    public Guid? UpdatedBy { get; set; }
    /// <summary>
    /// Get or set the tenant identifier.
    /// </summary>
    public Guid Tenant { get; set; }

    /// <summary>
    /// The list of events that have occurred in the aggregate root but have not yet been committed to the event store.
    /// </summary>
    private ConcurrentQueue<IDomainEvent> domainEvents = [];

    /// <summary>
    /// Default constructor to rehydrate the aggregate root.
    /// </summary>
    protected AggregateRoot() { }

    /// <summary>
    /// Initializes a new instance of <see cref="AggregateRoot"/>.
    /// </summary>
    /// <param name="id">The identifier of the aggregate root.</param>
    protected AggregateRoot(Guid id)
    {
        this.Id = id;
    }

    /// <summary>
    /// Apply the changes that occur in the aggregate root.
    /// </summary>
    /// <param name="event">The domain event to apply the changes.</param>
    protected virtual void AddEvent(IDomainEvent @event)
    {
        this.domainEvents ??= new ConcurrentQueue<IDomainEvent>();

        this.domainEvents.Enqueue(@event);
    }

    /// <summary>
    /// Get and clear the events that have occurred in the aggregate root.
    /// </summary>
    /// <returns>The list of events that have occurred in the aggregate root.</returns>
    public virtual IReadOnlyList<IDomainEvent> GetAndClearEvents()
    {
        if (this.domainEvents == null)
            return [];

        var events = this.domainEvents.ToList();

        this.domainEvents.Clear();

        return events;
    }
}
