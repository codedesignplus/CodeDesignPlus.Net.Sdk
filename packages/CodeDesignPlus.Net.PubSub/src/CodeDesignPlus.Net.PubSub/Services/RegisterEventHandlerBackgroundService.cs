namespace CodeDesignPlus.Net.PubSub.Services;

/// <summary>
/// Provides a background service for handling events with a specified event handler.
/// </summary>
/// <typeparam name="TEventHandler">The type of the event handler.</typeparam>
/// <typeparam name="TEvent">The type of the event.</typeparam>
public class RegisterEventHandlerBackgroundService<TEventHandler, TEvent>(
    IMessage message,
    ISubscriptionTracker subscriptionTracker,
    ILogger<RegisterEventHandlerBackgroundService<TEventHandler, TEvent>> logger
) : BackgroundService
    where TEventHandler : IEventHandler<TEvent>
    where TEvent : IDomainEvent
{
    private const int MaxRetriesDefault = 10;
    private const int BaseDelayMs = 2000;
    private const int MaxDelayMs = 60_000;

    /// <summary>
    /// Cuantos intentos se hacen antes de dar la suscripcion por perdida.
    /// </summary>
    protected virtual int MaxRetries => MaxRetriesDefault;

    /// <summary>
    /// La espera antes del siguiente intento: exponencial desde 2 s, con techo de 60 s.
    /// </summary>
    /// <remarks>
    /// Es <c>virtual</c> para que se pueda probar el comportamiento sin esperar el reloj real. Agotar los
    /// diez intentos con la espera de produccion lleva mas de cinco minutos, asi que la prueba que
    /// comprobaba que se marca el fallo estaba escrita contra una ventana de 25 segundos donde solo caben
    /// <b>cuatro</b> intentos, y fallaba siempre.
    /// <para>
    /// El calculo va en <c>double</c> a proposito: <c>2^(n-1)</c> en <c>int</c> se desborda a partir del
    /// intento 32, que hoy no ocurre porque el maximo son diez, pero si alguien sube el tope.
    /// </para>
    /// </remarks>
    /// <param name="retryCount">El numero de intento fallido, empezando en 1.</param>
    protected virtual TimeSpan GetDelay(int retryCount)
    {
        var delay = Math.Min(BaseDelayMs * Math.Pow(2, retryCount - 1), MaxDelayMs);

        return TimeSpan.FromMilliseconds(delay);
    }

    /// <summary>
    /// Executes the background service task with retry logic.
    /// </summary>
    /// <param name="stoppingToken">Triggered when the host is performing a graceful shutdown.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var handlerName = typeof(TEventHandler).Name;
        var eventName = typeof(TEvent).Name;

        logger.LogInformation("Starting subscription of {TEventHandler} for event type {TEvent}.", handlerName, eventName);

        var retryCount = 0;

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await message.SubscribeAsync<TEvent, TEventHandler>(stoppingToken);

                subscriptionTracker.MarkSubscribed(handlerName);

                logger.LogInformation("Successfully subscribed {TEventHandler} for event type {TEvent}.", handlerName, eventName);

                return;
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                logger.LogInformation("Subscription cancelled for {TEventHandler}/{TEvent} due to shutdown.", handlerName, eventName);
                return;
            }
            catch (Exception ex)
            {
                retryCount++;

                var delay = this.GetDelay(retryCount);

                logger.LogError(ex, "Failed to subscribe {TEventHandler} for {TEvent}. Attempt {Attempt}/{Max}. Retrying in {Delay}ms.",
                    handlerName, eventName, retryCount, this.MaxRetries, delay.TotalMilliseconds);

                if (retryCount >= this.MaxRetries)
                {
                    subscriptionTracker.MarkFailed(handlerName);

                    logger.LogCritical("Max retries ({Max}) exceeded for {TEventHandler}/{TEvent}. Handler will NOT consume events until pod restart.",
                        this.MaxRetries, handlerName, eventName);

                    return;
                }

                try
                {
                    await Task.Delay(delay, stoppingToken);
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    return;
                }
            }
        }
    }
}
