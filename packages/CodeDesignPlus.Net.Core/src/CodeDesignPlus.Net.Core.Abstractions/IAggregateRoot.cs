namespace CodeDesignPlus.Net.Core.Abstractions;

/// <summary>
/// Represents the contract to be implemented by the aggregate root.
/// </summary>
public interface IAggregateRoot: IEntity
{
    /// <summary>
    /// The date and time when the aggregate root was created.
    /// </summary>
    /// <param name="event">The event that will be added to the list of events.</param>
    void AddEvent(IDomainEvent @event);
    /// <summary>
    /// Gets the list of events that have occurred in the aggregate root and clears the list.
    /// </summary>
    /// <returns>The list of events that have occurred in the aggregate root.</returns>
    IReadOnlyList<IDomainEvent> GetAndClearEvents();
}