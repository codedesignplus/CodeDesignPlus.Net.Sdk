namespace CodeDesignPlus.Net.Core.Abstractions;

/// <summary>
/// Represents an aggregate root entity in the domain.
/// </summary>
/// <remarks>
/// An aggregate root is an entity that serves as the root of an aggregate, which is a cluster of associated objects that are treated as a single unit.
/// </remarks>
/// <seealso cref="IEntityBase"/>
public interface IAggregateRoot : IEntityBase
{
    /// <summary>
    /// Gets and clears the domain events associated with the aggregate root.
    /// </summary>
    /// <returns>A read-only list of domain events.</returns>
    IReadOnlyList<IDomainEvent> GetAndClearEvents();
}