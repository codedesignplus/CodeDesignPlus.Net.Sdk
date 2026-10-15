using CodeDesignPlus.Net.Core.Abstractions.Options;
using CodeDesignPlus.Net.Core.Abstractions.Snapshots;
using CodeDesignPlus.Net.PubSub.Abstractions.Attributes;

namespace CodeDesignPlus.Net.PubSub.Snapshots;

/// <summary>
/// Lado del dueño: contesta una petición de foto que lo nombra, con un evento de foto por agregado y el cierre.
/// </summary>
/// <remarks>
/// Solo se registra en una aplicación que implementa algún <see cref="ISnapshotSource"/>. Lee página a página y publica
/// cada página antes de leer la siguiente, así una fuente grande no se carga entera en memoria. Si algo falla a mitad, el
/// bus reentrega la petición y la foto se repite entera: la copia aplica con upsert, así que repetir no duplica.
/// </remarks>
[QueueName("ProjectionSnapshot", "SnapshotRequestedHandler")]
internal sealed class SnapshotRequestedHandler(IEnumerable<ISnapshotSource> sources, IMessage message, IOptions<CoreOptions> coreOptions, IOptions<PubSubOptions> pubSubOptions, ILogger<SnapshotRequestedHandler> logger) : IEventHandler<SnapshotRequestedDomainEvent>
{
    /// <inheritdoc/>
    public async Task HandleAsync(SnapshotRequestedDomainEvent data, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(data);

        if (!string.Equals(data.Source, coreOptions.Value.AppName, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        var source = sources.FirstOrDefault(x => string.Equals(x.Aggregate, data.Aggregate, StringComparison.OrdinalIgnoreCase) && string.Equals(x.Snapshot, data.Snapshot, StringComparison.OrdinalIgnoreCase));

        if (source is null)
        {
            logger.LogWarning("{RequestedBy} requested the snapshot {Snapshot} of {Aggregate}, but {Source} has no snapshot source for it.", data.RequestedBy, data.Snapshot, data.Aggregate, data.Source);

            return;
        }

        var pageSize = (int)pubSubOptions.Value.SnapshotPageSize;
        var page = 1;
        long count = 0;

        while (true)
        {
            var snapshots = await source.ReadPageAsync(data.Tenant, page, pageSize, token).ConfigureAwait(false);

            foreach (var snapshot in snapshots)
            {
                snapshot.RequestId = data.RequestId;
                snapshot.RequestedBy = data.RequestedBy;
            }

            if (snapshots.Count > 0)
            {
                await message.PublishAsync(snapshots, token).ConfigureAwait(false);
            }

            count += snapshots.Count;

            if (snapshots.Count < pageSize)
            {
                break;
            }

            page++;
        }

        await message.PublishAsync(SnapshotCompletedDomainEvent.Create(data, count), token).ConfigureAwait(false);

        logger.LogInformation("Snapshot {RequestId} ({Snapshot}) for {RequestedBy} published: {Count} aggregates.", data.RequestId, data.Snapshot, data.RequestedBy, count);
    }
}
