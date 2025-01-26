namespace CodeDesignPlus.Net.Core.Services;

/// <summary>
/// Represents a context for events, containing metadata and tenant information.
/// </summary>
public class EventContext: IEventContext, IDisposable
{
    private bool disposed;

    private Dictionary<string, object> Metadata { get; set; } = []; 

    /// <summary>
    /// Gets the current domain event.
    /// </summary>
    public IDomainEvent CurrentDomainEvent { get; private set; }
    
    /// <summary>
    /// Gets or sets the tenant identifier.
    /// </summary>
    public Guid Tenant { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="EventContext"/> class.
    /// </summary>
    /// <typeparam name="TEvent">The type of the domain event.</typeparam>
    /// <param name="domainEvent">The current domain event.</param>
    public void SetCurrentDomainEvent<TEvent>(TEvent domainEvent) where TEvent : IDomainEvent
    {
        if(this.CurrentDomainEvent != null)
            throw new CoreException("The current domain event is already set.");

        this.CurrentDomainEvent = domainEvent;

        if(domainEvent is ITenant tenant)
            this.Tenant = tenant.Tenant;
    }

    /// <summary>
    /// Adds a metadata entry with the specified key and value.
    /// </summary>
    /// <param name="key">The key of the metadata entry.</param>
    /// <param name="value">The value of the metadata entry.</param>
    public void AddMetadata(string key, object value)
    {
        this.Metadata.Add(key, value);
    }

    /// <summary>
    /// Gets the metadata value associated with the specified key.
    /// </summary>
    /// <param name="key">The key of the metadata entry.</param>
    /// <returns>The value of the metadata entry.</returns>
    public object GetMetadata(string key)
    {
        return this.Metadata[key];
    }

    /// <summary>
    /// Tries to get the metadata value associated with the specified key.
    /// </summary>
    /// <param name="key">The key of the metadata entry.</param>
    /// <param name="value">When this method returns, contains the value associated with the specified key, if the key is found; otherwise, the default value for the type of the value parameter. This parameter is passed uninitialized.</param>
    /// <returns>true if the metadata contains an element with the specified key; otherwise, false.</returns>
    public bool TryGetMetadata(string key, out object value)
    {
        return this.Metadata.TryGetValue(key, out value);
    }

    /// <summary>
    /// Removes the metadata entry with the specified key.
    /// </summary>
    /// <param name="key">The key of the metadata entry to remove.</param>
    public void RemoveMetadata(string key)
    {
        this.Metadata.Remove(key);
    }

    /// <summary>
    /// Clears all metadata entries.
    /// </summary>
    public void ClearMetadata()
    {
        this.Metadata.Clear();
    }

    /// <summary>
    /// Releases all resources used by the current instance of the <see cref="EventContext"/> class.
    /// </summary>
    protected virtual void Dispose(bool disposing)
    {
        if (!disposed)
        {
            if (disposing)
            {
                this.Metadata.Clear();
                this.Metadata = null;
            }

            disposed = true;
        }
    }

    /// <summary>
    /// Releases all resources used by the current instance of the <see cref="EventContext"/> class.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}