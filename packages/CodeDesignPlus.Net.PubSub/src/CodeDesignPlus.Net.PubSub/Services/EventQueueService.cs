namespace CodeDesignPlus.Net.PubSub.Services;

/// <summary>
/// Provides a service to manage the event queue.
/// </summary>
public class EventQueueService : IEventQueue
{
    private readonly ConcurrentQueue<IDomainEvent> queue = new();
    private readonly ILogger<EventQueueService> logger;
    private readonly IMessage message;
    private readonly PubSubOptions options;
    private readonly IActivityService activityService;

    /// <summary>
    /// Initializes a new instance of the <see cref="EventQueueService"/> class.
    /// </summary>
    /// <param name="logger">The logger to manage the logs.</param>
    /// <param name="options">The options for the PubSub service.</param>
    /// <param name="message">The message service to publish events.</param>
    /// <param name="activityService">The activity service to manage activities (optional).</param>
    public EventQueueService(ILogger<EventQueueService> logger, IOptions<PubSubOptions> options, IMessage message, IActivityService activityService = null)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(message);

        this.logger = logger;
        this.message = message;
        this.options = options.Value;
        this.activityService = activityService;

        this.logger.LogDebug("EventQueueService initialized.");
    }

    /// <summary>
    /// Enqueues an event to the queue.
    /// </summary>
    /// <param name="event">The event to enqueue.</param>
    /// <param name="cancellationToken">A cancellation token used to propagate notifications that operations should be canceled.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task EnqueueAsync(IDomainEvent @event, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(@event);

        var exist = this.queue.Any(x => x.Equals(@event));

        if (!exist)
        {
            var activity = this.activityService?.StartActivity("EventQueueService.EnqueueAsync", ActivityKind.Internal);

            this.activityService?.Inject(activity, @event);

            activity?.AddTag("event.type", @event.GetType().Name);
            activity?.AddTag("event.id", @event.EventId.ToString());
            activity?.AddTag("event.aggregate_id", @event.AggregateId.ToString());

            this.queue.Enqueue(@event);

            this.logger.LogDebug("Event of type {Name} enqueued.", @event.GetType().Name);

            activity?.SetStatus(ActivityStatusCode.Ok);
            activity?.Stop();
        }
        else
        {
            this.logger.LogWarning("Event of type {Name} was already in the queue. Skipping.", @event.GetType().Name);
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Dequeues events from the queue and processes them.
    /// </summary>
    /// <param name="token">A cancellation token used to propagate notifications that operations should be canceled.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task DequeueAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            try
            {
                if (this.queue.TryDequeue(out IDomainEvent @event))
                {
                    this.logger.LogDebug("Dequeueing event of type {TEvent}.", @event.GetType().Name);

                    var parentContext = this.activityService?.Extract(@event);

                    var activity = this.activityService?.StartActivity("EventQueueService.DequeueAsync", ActivityKind.Internal, parentContext);

                    activity?.AddTag("event.type", @event.GetType().Name);
                    activity?.AddTag("event.id", @event.EventId.ToString());
                    activity?.AddTag("event.aggregate_id", @event.AggregateId.ToString());

                    await this.message.PublishAsync(@event, token).ConfigureAwait(false);

                    activity?.SetStatus(ActivityStatusCode.Ok);

                    activity?.Stop();
                }
                else
                {
                    this.logger.LogDebug("No events in the queue. Waiting...");

                    await Task.Delay(TimeSpan.FromSeconds(this.options.SecondsWaitQueue), CancellationToken.None);
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error processing event.");
            }
        }

        this.logger.LogWarning("DequeueAsync stopped due to cancellation token.");
    }
}