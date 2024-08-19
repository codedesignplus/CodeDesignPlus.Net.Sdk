namespace CodeDesignPlus.Net.Core.Abstractions;

/// <summary>
/// Represents the base class for aggregate roots in the domain.
/// </summary>
public abstract class AggregateRoot : IAggregateRoot
{
    /// <summary>
    /// Gets or sets the unique identifier of the aggregate root.
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Gets or sets a value indicating whether the aggregate root is active.
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when the aggregate root was created.
    /// </summary>
    public long CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the user who created the aggregate root.
    /// </summary>
    public Guid CreatedBy { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when the aggregate root was last updated.
    /// </summary>
    public long? UpdatedAt { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the user who last updated the aggregate root.
    /// </summary>
    public Guid? UpdatedBy { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the tenant associated with the aggregate root.
    /// </summary>
    public Guid Tenant { get; set; }

    private ConcurrentQueue<IDomainEvent> domainEvents = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="AggregateRoot"/> class.
    /// </summary>
    protected AggregateRoot() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="AggregateRoot"/> class with the specified identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the aggregate root.</param>
    protected AggregateRoot(Guid id)
    {
        this.Id = id;
    }

    /// <summary>
    /// Adds a domain event to the aggregate root.
    /// </summary>
    /// <param name="event">The domain event to be added.</param>
    protected virtual void AddEvent(IDomainEvent @event)
    {
        this.domainEvents ??= new ConcurrentQueue<IDomainEvent>();

        this.domainEvents.Enqueue(@event);
    }

    /// <summary>
    /// Gets and clears the list of domain events associated with the aggregate root.
    /// </summary>
    /// <returns>The list of domain events.</returns>
    public virtual IReadOnlyList<IDomainEvent> GetAndClearEvents()
    {
        if (this.domainEvents == null)
            return [];

        var events = this.domainEvents.ToList();

        this.domainEvents.Clear();

        return events;
    }
}
