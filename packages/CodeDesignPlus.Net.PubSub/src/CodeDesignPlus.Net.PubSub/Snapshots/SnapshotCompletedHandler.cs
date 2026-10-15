using CodeDesignPlus.Net.Core.Abstractions.Options;
using CodeDesignPlus.Net.Core.Abstractions.Snapshots;
using CodeDesignPlus.Net.PubSub.Abstractions.Attributes;

namespace CodeDesignPlus.Net.PubSub.Snapshots;

/// <summary>
/// Lado de la copia: cuando el dueño termina la foto completa que pidió esta aplicación, marca la fuente como
/// sincronizada para no volver a pedirla al siguiente arranque.
/// </summary>
/// <remarks>
/// Una foto de una sola copropiedad (una reparación a mano) no cambia la marca: no prueba que estén todas.
/// </remarks>
[QueueName("ProjectionSnapshot", "SnapshotCompletedHandler")]
internal sealed class SnapshotCompletedHandler(IOptions<CoreOptions> coreOptions, ILogger<SnapshotCompletedHandler> logger, IProjectionSyncStore store = null) : IEventHandler<SnapshotCompletedDomainEvent>
{
    /// <inheritdoc/>
    public async Task HandleAsync(SnapshotCompletedDomainEvent data, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(data);

        if (!string.Equals(data.RequestedBy, coreOptions.Value.AppName, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        if (data.Tenant.HasValue)
        {
            logger.LogInformation("Snapshot {RequestId} of {Aggregate} from {Source} for tenant {Tenant} completed: {Count} aggregates.", data.RequestId, data.Aggregate, data.Source, data.Tenant, data.Count);

            return;
        }

        if (store is null)
        {
            logger.LogWarning("Snapshot {RequestId} of {Aggregate} from {Source} completed, but there is no {Store} to remember it.", data.RequestId, data.Aggregate, data.Source, nameof(IProjectionSyncStore));

            return;
        }

        await store.MarkSyncedAsync(data.Source, data.Aggregate, data.RequestId, data.Count, token).ConfigureAwait(false);

        logger.LogInformation("Projection of {Aggregate} from {Source} synced with snapshot {RequestId}: {Count} aggregates.", data.Aggregate, data.Source, data.RequestId, data.Count);
    }
}
