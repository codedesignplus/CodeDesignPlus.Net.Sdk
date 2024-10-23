namespace CodeDesignPlus.Net.Core.Abstractions;

/// <summary>
/// Represents the base class for aggregate roots in the domain.
/// </summary>
public abstract class AggregateRoot : AggregateRootBase, IAggregateRoot
{

    /// <summary>
    /// Gets or sets the unique identifier of the tenant associated with the aggregate root.
    /// </summary>
    public Guid Tenant { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AggregateRoot"/> class.
    /// </summary>
    protected AggregateRoot() : base() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="AggregateRoot"/> class with the specified identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the aggregate root.</param>
    protected AggregateRoot(Guid id) : base(id) { }
}
