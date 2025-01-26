namespace CodeDesignPlus.Net.Core.Abstractions;

/// <summary>
/// Represents a context for events, containing metadata and tenant information.
/// </summary>
public interface IEventContext
{
    /// <summary>
    /// Gets the current domain event.
    /// </summary>
    IDomainEvent CurrentDomainEvent { get; }

    /// <summary>
    /// Gets or sets the tenant identifier.
    /// </summary>
    Guid Tenant { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="EventContext"/> class.
    /// </summary>
    /// <typeparam name="TEvent">The type of the domain event.</typeparam>
    /// <param name="domainEvent">The current domain event.</param>
    void SetCurrentDomainEvent<TEvent>(TEvent domainEvent) where TEvent : IDomainEvent;
    
    /// <summary>
    /// Adds a metadata entry with the specified key and value.
    /// </summary>
    /// <param name="key">The key of the metadata entry.</param>
    /// <param name="value">The value of the metadata entry.</param>
    void AddMetadata(string key, object value);

    /// <summary>
    /// Gets the metadata value associated with the specified key.
    /// </summary>
    /// <param name="key">The key of the metadata entry.</param>
    /// <returns>The value of the metadata entry.</returns>
    object GetMetadata(string key);

    /// <summary>
    /// Tries to get the metadata value associated with the specified key.
    /// </summary>
    /// <param name="key">The key of the metadata entry.</param>
    /// <param name="value">When this method returns, contains the value associated with the specified key, if the key is found; otherwise, the default value for the type of the value parameter. This parameter is passed uninitialized.</param>
    /// <returns>true if the metadata contains an element with the specified key; otherwise, false.</returns>
    bool TryGetMetadata(string key, out object value);

    /// <summary>
    /// Removes the metadata entry with the specified key.
    /// </summary>
    /// <param name="key">The key of the metadata entry to remove.</param>
    void RemoveMetadata(string key);

    /// <summary>
    /// Clears all metadata entries.
    /// </summary>
    void ClearMetadata();
}
