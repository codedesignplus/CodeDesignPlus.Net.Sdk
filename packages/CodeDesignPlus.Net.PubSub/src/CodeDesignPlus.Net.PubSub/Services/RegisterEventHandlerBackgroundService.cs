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
    private const int MaxRetries = 10;
    private const int BaseDelayMs = 2000;
    private const int MaxDelayMs = 60_000;

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

                var delay = Math.Min(BaseDelayMs * (int)Math.Pow(2, retryCount - 1), MaxDelayMs);

                logger.LogError(ex, "Failed to subscribe {TEventHandler} for {TEvent}. Attempt {Attempt}/{Max}. Retrying in {Delay}ms.",
                    handlerName, eventName, retryCount, MaxRetries, delay);

                if (retryCount >= MaxRetries)
                {
                    subscriptionTracker.MarkFailed(handlerName);

                    logger.LogCritical("Max retries ({Max}) exceeded for {TEventHandler}/{TEvent}. Handler will NOT consume events until pod restart.",
                        MaxRetries, handlerName, eventName);

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
