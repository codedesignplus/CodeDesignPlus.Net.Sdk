using CodeDesignPlus.Net.Core.Abstractions.Options;
using CodeDesignPlus.Net.Core.Abstractions.Snapshots;
using CodeDesignPlus.Net.PubSub.Abstractions.Attributes;

namespace CodeDesignPlus.Net.PubSub.Snapshots;

/// <summary>
/// Lado de la copia: cuando el dueño termina la foto completa que pidió esta aplicación, anota cuántas fotos anunció; la
/// fuente queda sincronizada solo si la copia ya aplicó todas, o cuando aplique la última (pendings/485).
/// </summary>
/// <remarks>
/// Una foto de una sola copropiedad (una reparación a mano) no cambia la marca: no prueba que estén todas.
/// </remarks>
[QueueName("ProjectionSnapshot", "SnapshotCompletedHandler")]
internal sealed class SnapshotCompletedHandler(IOptions<CoreOptions> coreOptions, ISnapshotProgressTracker tracker, ILogger<SnapshotCompletedHandler> logger) : IEventHandler<SnapshotCompletedDomainEvent>
{
    /// <inheritdoc/>
    public Task HandleAsync(SnapshotCompletedDomainEvent data, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(data);

        if (!string.Equals(data.RequestedBy, coreOptions.Value.AppName, StringComparison.OrdinalIgnoreCase))
        {
            return Task.CompletedTask;
        }

        if (data.Tenant.HasValue)
        {
            logger.LogInformation("Snapshot {RequestId} of {Aggregate} from {Source} for tenant {Tenant} completed: {Count} aggregates.", data.RequestId, data.Aggregate, data.Source, data.Tenant, data.Count);

            return Task.CompletedTask;
        }

        return tracker.RecordCompletedAsync(data, token);
    }
}
