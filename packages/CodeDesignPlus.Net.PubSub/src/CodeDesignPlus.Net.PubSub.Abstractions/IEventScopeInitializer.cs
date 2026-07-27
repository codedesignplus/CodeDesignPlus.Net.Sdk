namespace CodeDesignPlus.Net.PubSub.Abstractions;

/// <summary>
/// Runs once per consumed message, after the event context has been set and before the event
/// handler is resolved. It is the extension point to prepare ambient state that handlers rely on,
/// such as the tenant context, without every handler having to do it.
/// </summary>
/// <remarks>
/// Registration is optional and several implementations may coexist: brokers resolve all of them
/// from the scope of the message and invoke them in registration order.
/// </remarks>
public interface IEventScopeInitializer
{
    /// <summary>
    /// Prepares the scope of the message about to be handled.
    /// </summary>
    /// <param name="domainEvent">The event that is about to be handled.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task InitializeAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default);
}
