using CodeDesignPlus.Net.Core.Abstractions.Snapshots;
using CodeDesignPlus.Net.Exceptions;
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
/// <remarks>
/// La foto de una copropiedad que ya no existe no lanza: la descarta el consumidor de fotos y cuenta como aplicada, para
/// que la petición de foto completa cuadre en vez de llenar la cola de descartados (pendings/467).
/// </remarks>
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

            // El equivalente al middleware para el camino asincrono: sin esto, todo lo que hace un
            // worker al consumir un evento saldria en las trazas sin tenant.
            TenantTelemetry.Seed(eventContext.Tenant, tenant.Name);
        }
        catch (CodeDesignPlusException exception) when (domainEvent is SnapshotDomainEvent && exception.Code == Errors.TenantNotFound.GetCode())
        {
            logger.LogInformation("The tenant {TenantId} of the snapshot {EventType} does not exist: the snapshot handler discards it", eventContext.Tenant, domainEvent.GetType().Name);
        }
        catch (Exception exception) when (exception is SecurityException or CodeDesignPlusException)
        {
            logger.LogError(exception, "The tenant {TenantId} could not be loaded while consuming {EventType}", eventContext.Tenant, domainEvent?.GetType().Name);

            throw;
        }
    }
}
