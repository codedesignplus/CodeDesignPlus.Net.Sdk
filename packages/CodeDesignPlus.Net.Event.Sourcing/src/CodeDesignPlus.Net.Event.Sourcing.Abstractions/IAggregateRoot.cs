namespace CodeDesignPlus.Net.Event.Sourcing.Abstractions;

/// <summary>
/// Represents the contract to be implemented by the aggregate root.
/// </summary>
public interface IAggregateRoot : Core.Abstractions.IAggregateRoot
{
    /// <summary>
    /// Gets the category of the aggregate root.
    /// </summary>
    string Category { get; }

    /// <summary>
    /// Gets the version of the aggregate root.
    /// </summary>
    long Version { get; }
}