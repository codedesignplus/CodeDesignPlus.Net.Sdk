using CodeDesignPlus.Net.Core.Abstractions.Snapshots;

namespace CodeDesignPlus.Net.PubSub.Snapshots;

/// <summary>
/// Lado de la copia: cuenta las fotos aplicadas de cada petición y marca la fuente como sincronizada solo cuando aplicó
/// todas las que anunció el dueño al cerrar (pendings/485).
/// </summary>
/// <remarks>
/// Las fotos y el cierre llegan por colas distintas y en cualquier orden. Cada lado anota lo suyo en el progreso de la
/// petición con una operación atómica que devuelve también lo del otro, así que el que escribe último ve las dos cosas y
/// marca: la última foto si el cierre ya estaba, o el cierre si ya estaban todas las fotos.
/// </remarks>
internal sealed class SnapshotProgressTracker(ILogger<SnapshotProgressTracker> logger, IProjectionSyncStore store = null) : ISnapshotProgressTracker
{
    /// <inheritdoc/>
    public async Task RecordAppliedAsync(SnapshotDomainEvent snapshot, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(snapshot);

        // Sin petición no hay cuenta que llevar: es una foto que no pidió nadie con el SDK.
        if (store is null || snapshot.RequestId == Guid.Empty)
        {
            return;
        }

        var key = SnapshotSync.KeyOf(snapshot.GetType());

        var progress = await store.RecordAppliedAsync(key, snapshot.RequestId, snapshot.EventId, cancellationToken).ConfigureAwait(false);

        if (progress is null || !progress.Expected.HasValue)
        {
            return;
        }

        if (progress.IsComplete)
        {
            await store.MarkSyncedAsync(key, progress.RequestId, progress.Applied, cancellationToken).ConfigureAwait(false);

            logger.LogInformation("Projection of {Aggregate} from {Source} synced with snapshot {RequestId}: {Applied} of {Expected} applied.", key.Aggregate, key.Source, progress.RequestId, progress.Applied, progress.Expected);
        }
        else if (progress.Applied > progress.Expected.Value)
        {
            logger.LogWarning("Snapshot {RequestId} of {Aggregate} from {Source} applied {Applied} snapshots but the owner announced {Expected}: the count proves nothing and the source stays unsynced.", progress.RequestId, key.Aggregate, key.Source, progress.Applied, progress.Expected);
        }
    }

    /// <inheritdoc/>
    public async Task RecordCompletedAsync(SnapshotCompletedDomainEvent completed, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(completed);

        if (store is null)
        {
            logger.LogWarning("Snapshot {RequestId} of {Aggregate} from {Source} completed, but there is no {Store} to remember it.", completed.RequestId, completed.Aggregate, completed.Source, nameof(IProjectionSyncStore));

            return;
        }

        var key = completed.Key();

        var progress = await store.RecordCompletedAsync(key, completed.RequestId, completed.Count, cancellationToken).ConfigureAwait(false);

        if (progress.IsComplete)
        {
            await store.MarkSyncedAsync(key, completed.RequestId, completed.Count, cancellationToken).ConfigureAwait(false);

            logger.LogInformation("Projection of {Aggregate} from {Source} synced with snapshot {RequestId}: {Applied} of {Expected} applied.", completed.Aggregate, completed.Source, completed.RequestId, progress.Applied, completed.Count);

            return;
        }

        await store.MarkIncompleteAsync(key, completed.RequestId, cancellationToken).ConfigureAwait(false);

        // Información y no advertencia: el cierre suele adelantarse a las últimas fotos, que siguen en su cola. La
        // advertencia sale cuando la petición se vuelve a pedir porque no se completó.
        logger.LogInformation("Snapshot {RequestId} of {Aggregate} from {Source} completed by the owner with {Expected} snapshots, {Applied} applied so far: incomplete until the rest are applied.", completed.RequestId, completed.Aggregate, completed.Source, completed.Count, progress.Applied);
    }
}
