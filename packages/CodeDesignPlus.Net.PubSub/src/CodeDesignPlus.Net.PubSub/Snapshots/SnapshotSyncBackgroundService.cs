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
/// Lado de la copia: al arrancar, pide la foto completa de cada fuente que todavía no figura como sincronizada, y después
/// la vuelve a pedir mientras no lo esté: primero tras las esperas de <see cref="PubSubOptions.SnapshotRetryDelays"/>, que
/// crecen, y luego cada <see cref="PubSubOptions.SnapshotRetryInterval"/>.
/// </summary>
/// <remarks>
/// Espera a que todos los consumidores estén suscritos: el bus reparte cada foto solo a las colas que ya existen, y una
/// foto publicada antes de que exista la cola de su consumidor se pierde. Si la suscripción de un consumidor de fotos
/// falló, no pide esa fuente. Con varias réplicas, cada una pide: el dueño contesta dos veces y la copia aplica con
/// upsert, así que no duplica.
/// <para>
/// Una fuente queda sin sincronizar si la copia no aplicó todo lo que anunció el dueño, o si el dueño no contestó
/// (pendings/485). Al repetir la petición lo avisa con una advertencia, con cuántas fotos aplicó de cuántas.
/// </para>
/// <para>
/// El dueño no contesta si arrancó después que la copia: la petición llegó antes de que existiera su suscripción y se
/// perdió. Las primeras repeticiones van pronto para que esa copia no espere el intervalo entero (pendings/466).
/// </para>
/// </remarks>
internal sealed class SnapshotSyncBackgroundService(ISubscriptionTracker tracker, ISnapshotRequester requester, IOptions<SnapshotSyncOptions> options, IOptions<PubSubOptions> pubSubOptions, ILogger<SnapshotSyncBackgroundService> logger, IProjectionSyncStore store = null) : BackgroundService
{
    /// <summary>
    /// Cada cuánto se mira si ya están suscritos los consumidores.
    /// </summary>
    internal TimeSpan PollInterval { get; init; } = TimeSpan.FromSeconds(1);

    /// <summary>
    /// El reloj con que se mide cuánto lleva una petición sin completarse.
    /// </summary>
    internal IClock Clock { get; init; } = SystemClock.Instance;

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

            await this.RequestPendingAsync(null, stoppingToken).ConfigureAwait(false);

            for (var attempt = 0; !stoppingToken.IsCancellationRequested; attempt++)
            {
                var delay = pubSubOptions.Value.SnapshotRetryDelayOf(attempt);

                if (delay <= TimeSpan.Zero)
                {
                    return;
                }

                await Task.Delay(delay, stoppingToken).ConfigureAwait(false);

                await this.RequestPendingAsync(delay, stoppingToken).ConfigureAwait(false);
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

    /// <summary>
    /// Pide cada fuente que no está sincronizada.
    /// </summary>
    /// <param name="retryAfter">Nulo al arrancar; al repetir, la espera que se acaba de cumplir.</param>
    /// <param name="stoppingToken">El token de parada.</param>
    private async Task RequestPendingAsync(TimeSpan? retryAfter, CancellationToken stoppingToken)
    {
        foreach (var (snapshot, handler) in options.Value.Snapshots)
        {
            await this.RequestIfPendingAsync(snapshot, handler, retryAfter, stoppingToken).ConfigureAwait(false);
        }
    }

    private async Task RequestIfPendingAsync(Type snapshot, Type handler, TimeSpan? retryAfter, CancellationToken stoppingToken)
    {
        if (tracker.FailedHandlers.Contains(handler.Name))
        {
            // Al repetir ya se avisó al arrancar: la suscripción no se reintenta sola.
            if (retryAfter is null)
            {
                logger.LogError("{Handler} is not subscribed: the snapshot of {Snapshot} is not requested.", handler.Name, snapshot.Name);
            }

            return;
        }

        try
        {
            var key = SnapshotSync.KeyOf(snapshot);

            var state = await store.FindAsync(key, stoppingToken).ConfigureAwait(false);

            if (state?.IsSynced == true)
            {
                return;
            }

            // Al repetir, una petición reciente (de otra réplica) todavía puede estar llegando y se le deja correr. La mitad de
            // la espera y no la espera entera: la petición propia se hizo justo antes de empezar a esperar, y por un instante
            // de reloj no puede saltarse su repetición.
            if (retryAfter.HasValue && state?.RequestedAt is not null && this.Clock.GetCurrentInstant() - state.RequestedAt.Value < Duration.FromTimeSpan(retryAfter.Value / 2))
            {
                return;
            }

            await this.WarnWhyAsync(key, state, retryAfter.HasValue, stoppingToken).ConfigureAwait(false);

            var requestId = await requester.RequestAsync(snapshot, null, stoppingToken).ConfigureAwait(false);

            await store.MarkRequestedAsync(key, requestId, stoppingToken).ConfigureAwait(false);

            logger.LogInformation("Snapshot {RequestId} ({Snapshot}) requested from {Source}.", requestId, key.Snapshot, key.Source);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            // Una fuente que no se pudo pedir no impide pedir las demás; se vuelve a intentar más tarde.
            logger.LogError(ex, "The snapshot of {Snapshot} could not be requested.", snapshot.Name);
        }
    }

    /// <summary>
    /// Deja en el log por qué se vuelve a pedir una foto que ya se había pedido.
    /// </summary>
    private async Task WarnWhyAsync(SnapshotKey key, ProjectionSyncState state, bool retrying, CancellationToken stoppingToken)
    {
        if (state is null)
        {
            return;
        }

        if (state.IsIncomplete)
        {
            var progress = await store.FindProgressAsync(state.IncompleteRequestId!.Value, stoppingToken).ConfigureAwait(false);

            logger.LogWarning("Snapshot {RequestId} ({Snapshot}) from {Source} is incomplete: {Applied} of {Expected} snapshots applied. Requesting it again.", state.IncompleteRequestId, key.Snapshot, key.Source, progress?.Applied ?? 0, progress?.Expected);

            return;
        }

        if (state.SyncedAt.HasValue)
        {
            logger.LogInformation("Snapshot {Snapshot} from {Source} was marked synced with {Count} snapshots before the applied ones were counted. Requesting it again to verify it.", key.Snapshot, key.Source, state.Count);

            return;
        }

        if (retrying)
        {
            logger.LogWarning("Snapshot {RequestId} ({Snapshot}) from {Source}, requested at {RequestedAt}, got no complete answer. Requesting it again.", state.LastRequestId, key.Snapshot, key.Source, state.RequestedAt);
        }
    }
}
