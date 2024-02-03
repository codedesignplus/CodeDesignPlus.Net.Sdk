using CodeDesignPlus.Net.Core.Abstractions;

namespace CodeDesignPlus.Net.Event.Sourcing.Abstractions;

/// <summary>
/// Represents the contract to be implemented by the aggregate root.
/// </summary>
public interface IAggregateRoot : Core.Abstractions.IAggregateRoot
{
    /// <summary>
    /// The category of the aggregate root.
    /// </summary>
    string Category { get; }
    /// <summary>
    /// The version of the aggregate root.
    /// </summary>
    long Version { get; }
    /// <summary>
    /// Apply the changes that occur in the aggregate root.
    /// </summary>
    /// <param name="event">The event that will be applied to the aggregate root.</param>
    void ApplyEvent(IDomainEvent @event);
}