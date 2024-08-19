namespace CodeDesignPlus.Net.PubSub.Services;

/// <summary>
/// Service responsible for publishing domain events.
/// </summary>
public class PubSubService : IPubSub
{
    private readonly IMessage message;
    private readonly IOptions<PubSubOptions> options;
    private readonly IServiceProvider serviceProvider;
    private readonly ILogger<PubSubService> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="PubSubService"/> class.
    /// </summary>
    /// <param name="message">The message service used for publishing events.</param>
    /// <param name="options">The options for configuring the PubSub service.</param>
    /// <param name="serviceProvider">The service provider for resolving dependencies.</param>
    /// <param name="logger">The logger for logging information.</param>
    /// <exception cref="ArgumentNullException">Thrown when any of the parameters are null.</exception>
    public PubSubService(IMessage message, IOptions<PubSubOptions> options, IServiceProvider serviceProvider, ILogger<PubSubService> logger)
    {
        ArgumentNullException.ThrowIfNull(message);
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(serviceProvider);
        ArgumentNullException.ThrowIfNull(logger);

        this.message = message;
        this.options = options;
        this.serviceProvider = serviceProvider;
        this.logger = logger;

        this.logger.LogDebug("PubSubService initialized.");
    }

    /// <summary>
    /// Publishes a single domain event asynchronously.
    /// </summary>
    /// <param name="event">The domain event to publish.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous publish operation.</returns>
    public Task PublishAsync(IDomainEvent @event, CancellationToken cancellationToken)
    {
        if (this.options.Value.UseQueue)
        {
            this.logger.LogDebug("UseQueue is true, enqueuing event of type {name}.", @event.GetType().Name);

            var eventQueueService = this.serviceProvider.GetRequiredService<IEventQueueService>();

            return eventQueueService.EnqueueAsync(@event, cancellationToken);
        }

        this.logger.LogDebug("UseQueue is false, publishing event of type {name}.", @event.GetType().Name);

        return this.message.PublishAsync(@event, cancellationToken);
    }

    /// <summary>
    /// Publishes a list of domain events asynchronously.
    /// </summary>
    /// <param name="events">The list of domain events to publish.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous publish operation.</returns>
    public Task PublishAsync(IReadOnlyList<IDomainEvent> events, CancellationToken cancellationToken)
    {
        var tasks = events.Select(@event => this.PublishAsync(@event, cancellationToken));

        return Task.WhenAll(tasks);
    }
}