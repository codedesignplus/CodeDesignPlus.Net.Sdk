namespace CodeDesignPlus.Net.PubSub.Services;

public class EventQueueService : IEventQueueService
{
    private readonly ConcurrentQueue<IDomainEvent> queue = new();
    private readonly ILogger<EventQueueService> logger;
    private readonly IMessage message;
    private readonly PubSubOptions options;
    private readonly IActivityService activityService;

    public EventQueueService(ILogger<EventQueueService> logger, IOptions<PubSubOptions> options, IMessage message, IActivityService activityService)
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

            this.logger.LogDebug("Event of type {name} enqueued.", @event.GetType().Name);

            activity?.SetStatus(ActivityStatusCode.Ok);
            activity?.Stop();
        }
        else
        {
            this.logger.LogWarning("Event of type {name} was already in the queue. Skipping.", @event.GetType().Name);
        }

        return Task.CompletedTask;
    }

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
