namespace CodeDesignPlus.Net.PubSub.Services;

/// <summary>
/// Envía al bus lo que espera en la bandeja de salida, lo reintenta con espera creciente si falla y deja como fallido, a
/// la vista, lo que agota sus intentos (pendings/245).
/// </summary>
/// <remarks>
/// Corre en cada réplica de cada entrypoint del micro. Cada mensaje se reclama antes de enviarlo, así que dos réplicas no
/// lo envían a la vez; si una muere a mitad, otra lo envía cuando vence el reclamo. El envío es al menos una vez: si el
/// proceso muere después de enviar y antes de anotarlo, el mensaje sale dos veces con el mismo identificador, igual que
/// cuando el bus reintenta una entrega.
/// </remarks>
internal sealed class OutboxDispatcherBackgroundService(IOutboxSignal signal, IOptions<PubSubOptions> options, ILogger<OutboxDispatcherBackgroundService> logger, IOutboxStore store = null, IOutboxTransport transport = null) : BackgroundService
{
    /// <summary>
    /// El comienzo fijo del log de un mensaje que agotó sus intentos. La alerta de SigNoz lo busca: no se cambia.
    /// </summary>
    internal const string ExhaustedLogPrefix = "Outbox message exhausted";

    private readonly string owner = $"{Environment.MachineName}/{Guid.NewGuid():N}";

    /// <inheritdoc/>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (store is null || transport is null)
        {
            logger.LogWarning("The outbox dispatcher is not running: there is no {Store} or no {Transport}.", nameof(IOutboxStore), nameof(IOutboxTransport));

            return;
        }

        var outbox = options.Value.Outbox;

        await this.EnsureIndexesAsync(outbox, stoppingToken).ConfigureAwait(false);

        logger.LogInformation("Outbox dispatcher {Owner} started.", this.owner);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await this.DispatchPendingAsync(stoppingToken).ConfigureAwait(false);

                await signal.WaitAsync(TimeSpan.FromSeconds(outbox.PollingIntervalSeconds), stoppingToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "The outbox dispatcher could not read the outbox.");

                await Task.Delay(TimeSpan.FromSeconds(outbox.PollingIntervalSeconds), stoppingToken).ConfigureAwait(false);
            }
        }

        logger.LogInformation("Outbox dispatcher {Owner} stopped.", this.owner);
    }

    /// <summary>
    /// Envía uno a uno los pendientes que ya toca enviar, hasta que no quede ninguno.
    /// </summary>
    /// <param name="stoppingToken">El token de parada del proceso.</param>
    /// <returns>Cuántos mensajes reclamó.</returns>
    internal async Task<int> DispatchPendingAsync(CancellationToken stoppingToken)
    {
        var outbox = options.Value.Outbox;
        var lease = Duration.FromSeconds(outbox.LeaseSeconds);
        var count = 0;

        while (!stoppingToken.IsCancellationRequested)
        {
            var message = await store.LeaseNextAsync(this.owner, lease, stoppingToken).ConfigureAwait(false);

            if (message is null)
            {
                break;
            }

            count++;

            await this.DispatchAsync(message, stoppingToken).ConfigureAwait(false);
        }

        return count;
    }

    /// <summary>
    /// Envía un mensaje y anota el resultado.
    /// </summary>
    private async Task DispatchAsync(OutboxMessage message, CancellationToken stoppingToken)
    {
        try
        {
            using var timeout = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);

            timeout.CancelAfter(TimeSpan.FromSeconds(options.Value.PublishTimeoutSeconds));

            await transport.SendAsync(message, timeout.Token).ConfigureAwait(false);
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
            // El proceso se apaga: el mensaje queda reclamado y otra réplica lo envía cuando vence el reclamo.
            throw;
        }
        catch (Exception exception)
        {
            await this.MarkFailedAsync(message, exception, stoppingToken).ConfigureAwait(false);

            return;
        }

        // Si anotarlo falla, el reclamo vence y se vuelve a enviar: un duplicado con el mismo identificador, nunca una
        // pérdida. Por eso no cuenta como intento fallido.
        await store.MarkSentAsync(message.Id, stoppingToken).ConfigureAwait(false);

        logger.LogDebug("Outbox message {EventName} {EventId} sent to {Topic}.", message.EventName, message.Id, message.Topic);
    }

    /// <summary>
    /// Anota un envío fallido: lo programa para más tarde con espera creciente o, si agotó los intentos, lo deja como
    /// fallido y lo registra con el mensaje fijo de la alerta.
    /// </summary>
    private async Task MarkFailedAsync(OutboxMessage message, Exception exception, CancellationToken stoppingToken)
    {
        var outbox = options.Value.Outbox;
        var attempts = message.Attempts + 1;

        if (attempts >= outbox.MaxAttempts)
        {
            await store.MarkFailedAsync(message.Id, exception.Message, null, stoppingToken).ConfigureAwait(false);

            logger.LogError(exception, ExhaustedLogPrefix + ": {EventName} {EventId} of aggregate {AggregateId} to {Topic} failed {Attempts} attempts and will not be retried.", message.EventName, message.Id, message.AggregateId, message.Topic, attempts);

            return;
        }

        var delay = RetryDelay(attempts, outbox);

        await store.MarkFailedAsync(message.Id, exception.Message, SystemClock.Instance.GetCurrentInstant() + delay, stoppingToken).ConfigureAwait(false);

        logger.LogWarning(exception, "Outbox message {EventName} {EventId} to {Topic} failed attempt {Attempts}/{MaxAttempts}; retrying in {Delay}.", message.EventName, message.Id, message.Topic, attempts, outbox.MaxAttempts, delay);
    }

    /// <summary>
    /// La espera antes del reintento: la inicial, duplicada en cada fallo, hasta la máxima.
    /// </summary>
    /// <param name="attempts">Cuántos envíos fallidos lleva, contando el último.</param>
    /// <param name="outbox">Las opciones de la bandeja.</param>
    /// <returns>La espera.</returns>
    internal static Duration RetryDelay(int attempts, OutboxOptions outbox)
    {
        var seconds = outbox.InitialRetryDelaySeconds * Math.Pow(2, Math.Min(attempts - 1, 30));

        return Duration.FromSeconds(Math.Min(seconds, outbox.MaxRetryDelaySeconds));
    }

    /// <summary>
    /// Crea los índices de la bandeja. Si falla, el despachador sigue: sin índices se envía igual, solo más despacio y sin
    /// borrar lo enviado.
    /// </summary>
    private async Task EnsureIndexesAsync(OutboxOptions outbox, CancellationToken stoppingToken)
    {
        try
        {
            await store.EnsureIndexesAsync(Duration.FromDays(outbox.SentRetentionDays), stoppingToken).ConfigureAwait(false);
        }
        catch (Exception exception) when (!stoppingToken.IsCancellationRequested)
        {
            logger.LogError(exception, "Could not create the outbox indexes.");
        }
    }
}
