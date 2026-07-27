using Microsoft.Extensions.DependencyInjection;

namespace CodeDesignPlus.Net.PubSub.Abstractions;

/// <summary>
/// Helpers used by the broker implementations to run the registered
/// <see cref="IEventScopeInitializer"/> for a message.
/// </summary>
public static class EventScopeInitializerExtensions
{
    /// <summary>
    /// Runs every registered <see cref="IEventScopeInitializer"/> against the scope of the message.
    /// Does nothing when none is registered.
    /// </summary>
    /// <param name="serviceProvider">The service provider of the scope of the message.</param>
    /// <param name="domainEvent">The event that is about to be handled.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public static async Task InitializeEventScopeAsync(this IServiceProvider serviceProvider, IDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        foreach (var initializer in serviceProvider.GetServices<IEventScopeInitializer>())
        {
            await initializer.InitializeAsync(domainEvent, cancellationToken).ConfigureAwait(false);
        }
    }
}
