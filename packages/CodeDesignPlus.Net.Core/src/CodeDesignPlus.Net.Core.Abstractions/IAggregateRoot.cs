namespace CodeDesignPlus.Net.Core.Abstractions;

/// <summary>
/// Represents the contract to be implemented by the aggregate root.
/// </summary>
public interface IAggregateRoot : IEntity
{
    /// <summary>
    /// Gets the list of events that have occurred in the aggregate root and clears the list.
    /// </summary>
    /// <returns>The list of events that have occurred in the aggregate root.</returns>
    IReadOnlyList<IDomainEvent> GetAndClearEvents();
}