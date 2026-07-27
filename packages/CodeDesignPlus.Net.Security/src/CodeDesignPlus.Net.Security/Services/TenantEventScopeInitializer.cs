using CodeDesignPlus.Net.PubSub.Abstractions;
using Microsoft.Extensions.Logging;
using IDomainEvent = CodeDesignPlus.Net.Core.Abstractions.IDomainEvent;
using IEventContext = CodeDesignPlus.Net.Core.Abstractions.IEventContext;

namespace CodeDesignPlus.Net.Security.Services;

/// <summary>
/// Loads the tenant carried by a consumed domain event into <see cref="ITenant"/>, so that handlers
/// running in a background worker can read its location, currency and licensed modules the same way
/// an HTTP handler does.
/// </summary>
/// <param name="eventContext">The context of the event being consumed.</param>
/// <param name="tenant">The tenant of the current scope.</param>
/// <param name="logger">The logger service.</param>
public class TenantEventScopeInitializer(IEventContext eventContext, ITenant tenant, ILogger<TenantEventScopeInitializer> logger) : IEventScopeInitializer
{
    /// <inheritdoc/>
    public async Task InitializeAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        // EventContext solo trae el tenant si el evento implementa Core.Abstractions.ITenant. Si no,
        // no hay a quien cargar y el handler corre igual: el contexto es opcional.
        if (eventContext.Tenant == Guid.Empty)
            return;

        try
        {
            await tenant.SetAsync(eventContext.Tenant, cancellationToken);
        }
        catch (SecurityException exception)
        {
            logger.LogError(exception, "The tenant {TenantId} could not be loaded while consuming {EventType}", eventContext.Tenant, domainEvent?.GetType().Name);

            throw;
        }
    }
}
