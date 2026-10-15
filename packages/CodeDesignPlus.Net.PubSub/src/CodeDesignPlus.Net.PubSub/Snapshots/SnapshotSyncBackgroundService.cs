using CodeDesignPlus.Net.Core.Abstractions.Snapshots;
using CodeDesignPlus.Net.PubSub.Abstractions.Snapshots;

namespace CodeDesignPlus.Net.PubSub.Snapshots;

/// <summary>
/// Las fotos que consume esta aplicación: un gemelo por cada <see cref="SnapshotHandler{TSnapshot}"/> descubierto.
/// </summary>
internal sealed class SnapshotSyncOptions
{
    /// <summary>
    /// Los eventos de foto con su consumidor.
    /// </summary>
    public List<(Type Snapshot, Type Handler)> Snapshots { get; } = [];
}

/// <summary>
/// Lado de la copia: al arrancar, pide la foto completa de cada fuente que todavía no figura como sincronizada.
/// </summary>
/// <remarks>
/// Espera a que todos los consumidores estén suscritos: el bus reparte cada foto solo a las colas que ya existen, y una
/// foto publicada antes de que exista la cola de su consumidor se pierde. Si la suscripción de un consumidor de fotos
/// falló, no pide esa fuente. Con varias réplicas, cada una pide: el dueño contesta dos veces y la copia aplica con
/// upsert, así que no duplica.
/// </remarks>
internal sealed class SnapshotSyncBackgroundService(ISubscriptionTracker tracker, ISnapshotRequester requester, IOptions<SnapshotSyncOptions> options, ILogger<SnapshotSyncBackgroundService> logger, IProjectionSyncStore store = null) : BackgroundService
{
    /// <summary>
    /// Cada cuánto se mira si ya están suscritos los consumidores.
    /// </summary>
    internal TimeSpan PollInterval { get; init; } = TimeSpan.FromSeconds(1);

    /// <inheritdoc/>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (store is null)
        {
            logger.LogWarning("There is no {Store}: the projections fed by snapshots will not request one on startup.", nameof(IProjectionSyncStore));

            return;
        }

        try
        {
            await this.WaitForSubscriptionsAsync(stoppingToken).ConfigureAwait(false);

            foreach (var (snapshot, handler) in options.Value.Snapshots)
            {
                await this.RequestIfPendingAsync(snapshot, handler, stoppingToken).ConfigureAwait(false);
            }
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
            logger.LogInformation("Snapshot synchronization cancelled due to shutdown.");
        }
    }

    private async Task WaitForSubscriptionsAsync(CancellationToken stoppingToken)
    {
        while (!IsSettled(tracker))
        {
            await Task.Delay(this.PollInterval, stoppingToken).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Todos los consumidores terminaron de intentar suscribirse, bien o mal.
    /// </summary>
    private static bool IsSettled(ISubscriptionTracker tracker) => tracker.ExpectedCount > 0 && tracker.ReadyCount + tracker.FailedHandlers.Count >= tracker.ExpectedCount;

    private async Task RequestIfPendingAsync(Type snapshot, Type handler, CancellationToken stoppingToken)
    {
        if (tracker.FailedHandlers.Contains(handler.Name))
        {
            logger.LogError("{Handler} is not subscribed: the snapshot of {Snapshot} is not requested.", handler.Name, snapshot.Name);

            return;
        }

        try
        {
            var key = SnapshotSourceKey.Of(snapshot);

            var state = await store.FindAsync(key.Source, key.Aggregate, stoppingToken).ConfigureAwait(false);

            if (state?.IsSynced == true)
            {
                return;
            }

            var requestId = await requester.RequestAsync(snapshot, null, stoppingToken).ConfigureAwait(false);

            await store.MarkRequestedAsync(key.Source, key.Aggregate, requestId, stoppingToken).ConfigureAwait(false);

            logger.LogInformation("Snapshot {RequestId} of {Aggregate} requested from {Source}.", requestId, key.Aggregate, key.Source);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            // Una fuente que no se pudo pedir no impide pedir las demás; se vuelve a intentar en el siguiente arranque.
            logger.LogError(ex, "The snapshot of {Snapshot} could not be requested.", snapshot.Name);
        }
    }
}
