namespace CodeDesignPlus.Net.Redis.PubSub.Services;

/// <summary>
/// Provides Redis Pub/Sub services for publishing and subscribing to domain events.
/// </summary>
public class RedisPubSubService : IRedisPubSub
{
    private readonly ILogger<RedisPubSubService> logger;
    private readonly Redis.Abstractions.IRedis redisService;
    private readonly IDomainEventResolver domainEventResolverService;
    private readonly IServiceProvider serviceProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="RedisPubSubService"/> class.
    /// </summary>
    /// <param name="redisServiceFactory">The factory to create Redis services.</param>
    /// <param name="serviceProvider">The service provider.</param>
    /// <param name="logger">The logger instance.</param>
    /// <param name="domainEventResolverService">The domain event resolver service.</param>
    /// <exception cref="ArgumentNullException">Thrown when any of the parameters are null.</exception>
    public RedisPubSubService(
        IRedisFactory redisServiceFactory,
        IServiceProvider serviceProvider,
        ILogger<RedisPubSubService> logger,
        IDomainEventResolver domainEventResolverService)
    {
        ArgumentNullException.ThrowIfNull(redisServiceFactory);
        ArgumentNullException.ThrowIfNull(serviceProvider);
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(domainEventResolverService);

        this.redisService = redisServiceFactory.Create(FactoryConst.RedisPubSub);

        this.domainEventResolverService = domainEventResolverService;
        this.serviceProvider = serviceProvider;
        this.logger = logger;

        this.logger.LogInformation("RedisPubSubService initialized.");
    }

    /// <summary>
    /// Publishes a domain event asynchronously.
    /// </summary>
    /// <param name="event">The domain event to publish.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the event is null.</exception>
    public Task PublishAsync(IDomainEvent @event, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(@event);

        this.logger.LogInformation("Publishing event: {TEvent}.", @event.GetType().Name);

        return this.PrivatePublishAsync<long>(@event);
    }

    /// <summary>
    /// Publishes a list of domain events asynchronously.
    /// </summary>
    /// <param name="event">The list of domain events to publish.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task PublishAsync(IReadOnlyList<IDomainEvent> @event, CancellationToken cancellationToken)
    {
        var tasks = @event.Select(x => this.PublishAsync(x, cancellationToken));

        return Task.WhenAll(tasks);
    }

    /// <summary>
    /// Publishes a domain event to the Redis channel.
    /// </summary>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <param name="event">The domain event to publish.</param>
    /// <returns>A task that represents the asynchronous operation, containing the result.</returns>
    private async Task<TResult> PrivatePublishAsync<TResult>(object @event)
    {
        var channel = this.domainEventResolverService.GetKeyDomainEvent(@event.GetType());

        var message = JsonSerializer.Serialize(@event);

        var notified = await this.redisService.Subscriber.PublishAsync(RedisChannel.Literal(channel), message);

        this.logger.LogInformation("Event {TEvent} published with {Notified} notifications.", @event.GetType().Name, notified);

        return (TResult)Convert.ChangeType(notified, typeof(TResult));
    }

    /// <summary>
    /// Subscribes to a domain event asynchronously.
    /// </summary>
    /// <typeparam name="TEvent">The type of the domain event.</typeparam>
    /// <typeparam name="TEventHandler">The type of the event handler.</typeparam>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task SubscribeAsync<TEvent, TEventHandler>(CancellationToken cancellationToken)
        where TEvent : IDomainEvent
        where TEventHandler : IEventHandler<TEvent>
    {
        var channel = this.domainEventResolverService.GetKeyDomainEvent(typeof(TEvent));

        this.logger.LogInformation("Subscribed to event: {TEvent}.", typeof(TEvent).Name);

        return this.redisService.Subscriber.SubscribeAsync(RedisChannel.Literal(channel), (_, v) => this.ListenerEvent<TEvent, TEventHandler>(v, cancellationToken));
    }

    /// <summary>
    /// Handles the received domain event.
    /// </summary>
    /// <typeparam name="TEvent">The type of the domain event.</typeparam>
    /// <typeparam name="TEventHandler">The type of the event handler.</typeparam>
    /// <param name="value">The received event value.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public void ListenerEvent<TEvent, TEventHandler>(RedisValue value, CancellationToken cancellationToken)
        where TEvent : IDomainEvent
        where TEventHandler : IEventHandler<TEvent>
    {
        var @event = JsonSerializer.Deserialize<TEvent>(value);

        var eventHandler = this.serviceProvider.GetRequiredService<TEventHandler>();

        eventHandler.HandleAsync(@event, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Unsubscribes from a domain event asynchronously.
    /// </summary>
    /// <typeparam name="TEvent">The type of the domain event.</typeparam>
    /// <typeparam name="TEventHandler">The type of the event handler.</typeparam>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task UnsubscribeAsync<TEvent, TEventHandler>(CancellationToken cancellationToken)
        where TEvent : IDomainEvent
        where TEventHandler : IEventHandler<TEvent>
    {
        var channel = this.domainEventResolverService.GetKeyDomainEvent(typeof(TEvent));

        this.redisService.Subscriber.Unsubscribe(RedisChannel.Literal(channel));

        this.logger.LogInformation("Unsubscribed from event: {TEvent}.", typeof(TEvent).Name);

        return Task.CompletedTask;
    }
}