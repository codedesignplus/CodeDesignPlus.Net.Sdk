namespace CodeDesignPlus.Net.RabbitMQ.Services;

/// <summary>
/// Service to publish and subscribe to domain events using RabbitMQ.
/// </summary>
public class RabbitPubSubService : IRabbitPubSub
{
    private readonly ILogger<RabbitPubSubService> logger;
    private readonly IServiceProvider serviceProvider;
    private readonly IDomainEventResolver domainEventResolverService;
    private readonly CoreOptions coreOptions;
    private readonly IChannelProvider channelProvider;
    private readonly Dictionary<string, object> argumentsQueue;

    /// <summary>
    /// Initializes a new instance of the <see cref="RabbitPubSubService"/> class.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="serviceProvider">The service provider.</param>
    /// <param name="domainEventResolverService">The domain event resolver service.</param>
    /// <param name="channelProvider">The channel provider.</param>
    /// <param name="coreOptions">The core options.</param>
    /// <param name="rabbitMQOptions">The RabbitMQ options.</param>
    public RabbitPubSubService(ILogger<RabbitPubSubService> logger, IServiceProvider serviceProvider, IDomainEventResolver domainEventResolverService, IChannelProvider channelProvider, IOptions<CoreOptions> coreOptions, IOptions<RabbitMQOptions> rabbitMQOptions)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(serviceProvider);
        ArgumentNullException.ThrowIfNull(domainEventResolverService);
        ArgumentNullException.ThrowIfNull(coreOptions);
        ArgumentNullException.ThrowIfNull(rabbitMQOptions);
        ArgumentNullException.ThrowIfNull(channelProvider);

        this.logger = logger;
        this.serviceProvider = serviceProvider;
        this.domainEventResolverService = domainEventResolverService;
        this.coreOptions = coreOptions.Value;
        this.channelProvider = channelProvider;

        this.argumentsQueue = rabbitMQOptions.Value.QueueArguments.GetArguments();

        this.logger.LogInformation("RabbitPubSubService initialized.");
    }

    /// <summary>
    /// Publishes a domain event asynchronously.
    /// </summary>
    /// <param name="event">The domain event to publish.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous publish operation.</returns>
    public Task PublishAsync(IDomainEvent @event, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(@event);

        this.logger.LogInformation("Publishing event: {TEvent}.", @event.GetType().Name);

        return this.PrivatePublishAsync(@event);
    }

    /// <summary>
    /// Publishes a list of domain events asynchronously.
    /// </summary>
    /// <param name="event">The list of domain events to publish.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous publish operation.</returns>
    public Task PublishAsync(IReadOnlyList<IDomainEvent> @event, CancellationToken cancellationToken)
    {
        var tasks = @event.Select(x => PublishAsync(x, cancellationToken));

        return Task.WhenAll(tasks);
    }

    /// <summary>
    /// Publishes a domain event to RabbitMQ.
    /// </summary>
    /// <param name="event">The domain event to publish.</param>
    /// <returns>A task that represents the asynchronous publish operation.</returns>
    private async Task PrivatePublishAsync(IDomainEvent @event)
    {
        var channel = await this.channelProvider.GetChannelPublishAsync(@event.GetType());

        var exchangeName = await this.channelProvider.ExchangeDeclareAsync(@event.GetType());

        var message = JsonSerializer.Serialize(@event);

        var body = Encoding.UTF8.GetBytes(message);

        var properties = new BasicProperties
        {
            Persistent = true,
            AppId = coreOptions.AppName,
            Type = @event.GetType().Name,
            Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds()),
            MessageId = @event.EventId.ToString(),
            CorrelationId = Guid.NewGuid().ToString(),
            ContentEncoding = "utf-8",
            ContentType = "application/json"
        };

        await channel.BasicPublishAsync(
            exchange: exchangeName,
            routingKey: string.Empty,
            mandatory: true,
            basicProperties: properties,
            body: body
        );

        this.logger.LogInformation("Event {TEvent} published ", @event.GetType().Name);
    }

    /// <summary>
    /// Subscribes to a domain event asynchronously.
    /// </summary>
    /// <typeparam name="TEvent">The type of the domain event.</typeparam>
    /// <typeparam name="TEventHandler">The type of the event handler.</typeparam>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous subscribe operation.</returns>
    public async Task SubscribeAsync<TEvent, TEventHandler>(CancellationToken cancellationToken)
        where TEvent : IDomainEvent
        where TEventHandler : IEventHandler<TEvent>
    {
        var channel = await this.channelProvider.GetChannelConsumerAsync<TEvent, TEventHandler>();

        var queueNameAttribute = typeof(TEventHandler).GetCustomAttribute<QueueNameAttribute>();
        var queueName = queueNameAttribute.GetQueueName(coreOptions.AppName, coreOptions.Business, coreOptions.Version);

        var exchangeName = this.domainEventResolverService.GetKeyDomainEvent<TEvent>();

        await ConfigQueueAsync(channel, queueName, exchangeName);
        await ConfigQueueDlxAsync(channel, queueName, exchangeName);

        this.logger.LogInformation("Subscribed to event: {TEvent}.", typeof(TEvent).Name);

        var eventConsumer = new AsyncEventingBasicConsumer(channel);

        eventConsumer.ReceivedAsync += async (_, ea) => await RecivedEvent<TEvent, TEventHandler>(channel, ea, cancellationToken);

        var consumerTag = await channel.BasicConsumeAsync(queue: queueName, autoAck: false, consumer: eventConsumer, cancellationToken: cancellationToken);

        this.channelProvider.SetConsumerTag<TEvent, TEventHandler>(consumerTag);
    }

    /// <summary>
    /// Processes the received event.
    /// </summary>
    /// <typeparam name="TEvent">The type of the domain event.</typeparam>
    /// <typeparam name="TEventHandler">The type of the event handler.</typeparam>
    /// <param name="channel">The RabbitMQ channel.</param>
    /// <param name="eventArguments">The event arguments.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task RecivedEvent<TEvent, TEventHandler>(IChannel channel, BasicDeliverEventArgs eventArguments, CancellationToken cancellationToken)
        where TEvent : IDomainEvent
        where TEventHandler : IEventHandler<TEvent>
    {
        try
        {
            this.logger.LogDebug("Processing event: {TEvent}.", typeof(TEvent).Name);

            var body = eventArguments.Body.ToArray();

            var message = Encoding.UTF8.GetString(body);

            var @event = JsonSerializer.Deserialize<TEvent>(message);

            var eventHandler = this.serviceProvider.GetRequiredService<TEventHandler>();

            await eventHandler.HandleAsync(@event, cancellationToken).ConfigureAwait(false);

            await channel.BasicAckAsync(deliveryTag: eventArguments.DeliveryTag, multiple: false, cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Error processing event: {TEvent}.", typeof(TEvent).Name);

            await channel.BasicNackAsync(deliveryTag: eventArguments.DeliveryTag, multiple: false, requeue: false, cancellationToken: cancellationToken);
        }
    }

    /// <summary>
    /// Configures the RabbitMQ queue.
    /// </summary>
    /// <param name="channel">The RabbitMQ channel.</param>
    /// <param name="queue">The queue name.</param>
    /// <param name="exchangeName">The exchange name.</param>
    private async Task ConfigQueueAsync(IChannel channel, string queue, string exchangeName)
    {
        argumentsQueue["x-dead-letter-exchange"] = GetExchangeNameDlx(exchangeName);
        await channel.ExchangeDeclareAsync(exchange: exchangeName, type: ExchangeType.Fanout, durable: true);
        await channel.QueueDeclareAsync(queue: queue, durable: true, exclusive: false, autoDelete: false, arguments: argumentsQueue);
        await channel.QueueBindAsync(queue: queue, exchange: exchangeName, routingKey: string.Empty);
    }

    /// <summary>
    /// Configures the dead-letter exchange (DLX) queue.
    /// </summary>
    /// <param name="channel">The RabbitMQ channel.</param>
    /// <param name="queue">The queue name.</param>
    /// <param name="exchangeName">The exchange name.</param>
    private static async Task ConfigQueueDlxAsync(IChannel channel, string queue, string exchangeName)
    {
        exchangeName = GetExchangeNameDlx(exchangeName);
        queue = GetQueueNameDlx(queue);

        await channel.ExchangeDeclareAsync(exchange: exchangeName, type: ExchangeType.Fanout, durable: true);
        await channel.QueueDeclareAsync(queue: queue, durable: true, exclusive: false, autoDelete: false, arguments: null);
        await channel.QueueBindAsync(queue: queue, exchange: exchangeName, routingKey: string.Empty);
    }

    /// <summary>
    /// Gets the dead-letter exchange (DLX) name.
    /// </summary>
    /// <param name="exchangeName">The original exchange name.</param>
    /// <returns>The DLX name.</returns>
    public static string GetExchangeNameDlx(string exchangeName) => $"{exchangeName}.dlx";

    /// <summary>
    /// Gets the dead-letter queue (DLQ) name.
    /// </summary>
    /// <param name="queueName">The original queue name.</param>
    /// <returns>The DLQ name.</returns>
    public static string GetQueueNameDlx(string queueName) => $"{queueName}.dlx";

    /// <summary>
    /// Unsubscribes from a domain event asynchronously.
    /// </summary>
    /// <typeparam name="TEvent">The type of the domain event.</typeparam>
    /// <typeparam name="TEventHandler">The type of the event handler.</typeparam>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous unsubscribe operation.</returns>
    public async Task UnsubscribeAsync<TEvent, TEventHandler>(CancellationToken cancellationToken)
        where TEvent : IDomainEvent
        where TEventHandler : IEventHandler<TEvent>
    {
        var consumerTag = this.channelProvider.GetConsumerTag<TEvent, TEventHandler>();

        if (!string.IsNullOrEmpty(consumerTag))
        {
            var channel = await this.channelProvider.GetChannelConsumerAsync<TEvent, TEventHandler>();
            await channel.BasicCancelAsync(consumerTag, cancellationToken: cancellationToken);
            logger.LogInformation("Unsubscribed from event: {TEvent}.", typeof(TEvent).Name);
        }
    }
}
