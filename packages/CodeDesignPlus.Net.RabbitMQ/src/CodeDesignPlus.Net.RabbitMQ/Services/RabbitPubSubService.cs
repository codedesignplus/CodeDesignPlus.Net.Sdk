namespace CodeDesignPlus.Net.RabbitMQ.Services;

public class RabbitPubSubService : IRabbitPubSubService
{
    private readonly ILogger<RabbitPubSubService> logger;
    private readonly IServiceProvider serviceProvider;
    private readonly IDomainEventResolverService domainEventResolverService;
    private readonly CoreOptions coreOptions;
    private readonly IChannelProvider channelProvider;
    private readonly Dictionary<string, object> argumentsQueue;


    public RabbitPubSubService(ILogger<RabbitPubSubService> logger, IServiceProvider serviceProvider, IDomainEventResolverService domainEventResolverService, IChannelProvider channelProvider, IOptions<CoreOptions> coreOptions, IOptions<RabbitMQOptions> rabbitMQOptions)
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

        this.logger.LogInformation("RabitPubSubService initialized.");
    }

    public Task PublishAsync(IDomainEvent @event, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(@event);

        this.logger.LogInformation("Publishing event: {TEvent}.", @event.GetType().Name);

        return this.PrivatePublishAsync(@event);
    }

    public Task PublishAsync(IReadOnlyList<IDomainEvent> @event, CancellationToken cancellationToken)
    {
        var tasks = @event.Select(x => PublishAsync(x, cancellationToken));

        return Task.WhenAll(tasks);
    }


    private Task PrivatePublishAsync(IDomainEvent @event)
    {
        var channel = this.channelProvider.GetChannelPublish(@event.GetType());

        var exchangeName = this.channelProvider.ExchangeDeclare(@event.GetType());

        var message = JsonSerializer.Serialize(@event);

        var body = Encoding.UTF8.GetBytes(message);

        var properties = channel.CreateBasicProperties();
        properties.Persistent = true;
        properties.AppId = coreOptions.AppName;
        properties.Type = @event.GetType().Name;
        properties.Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds());
        properties.MessageId = @event.EventId.ToString();
        properties.CorrelationId = Guid.NewGuid().ToString();
        properties.ContentEncoding = "utf-8";
        properties.ContentType = "application/json";

        channel.BasicPublish(
            exchange: exchangeName,
            routingKey: string.Empty,
            basicProperties: properties,
            body: body
        );

        this.logger.LogInformation("Event {TEvent} published ", @event.GetType().Name);

        return Task.CompletedTask;
    }

    public Task SubscribeAsync<TEvent, TEventHandler>(CancellationToken cancellationToken)
        where TEvent : IDomainEvent
        where TEventHandler : IEventHandler<TEvent>
    {
        var channel = this.channelProvider.GetChannelConsumer<TEvent, TEventHandler>();

        var queueNameAttribute = typeof(TEventHandler).GetCustomAttribute<QueueNameAttribute>();
        var queueName = queueNameAttribute.GetQueueName(coreOptions.AppName, coreOptions.Business, coreOptions.Version);

        var exchangeName = this.domainEventResolverService.GetKeyDomainEvent(typeof(TEvent));

        ConfigQueue(channel, queueName, exchangeName);
        ConfigQueueDlx(channel, queueName, exchangeName);

        this.logger.LogInformation("Subscribed to event: {TEvent}.", typeof(TEvent).Name);

        var eventConsumer = new EventingBasicConsumer(channel);

        eventConsumer.Received += async (_, ea) => await RecivedEvent<TEvent, TEventHandler>(channel, ea, cancellationToken);

        var consumerTag = channel.BasicConsume(queue: queueName, autoAck: false, consumer: eventConsumer);

        this.channelProvider.SetConsumerTag<TEvent, TEventHandler>(consumerTag);

        return Task.CompletedTask;
    }

    public async Task RecivedEvent<TEvent, TEventHandler>(IModel channel, BasicDeliverEventArgs eventArguments, CancellationToken cancellationToken)
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

            channel.BasicAck(deliveryTag: eventArguments.DeliveryTag, multiple: false);
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Error processing event: {TEvent}.", typeof(TEvent).Name);

            channel.BasicNack(deliveryTag: eventArguments.DeliveryTag, multiple: false, requeue: false);
        }
    }

    private void ConfigQueue(IModel channel, string queue, string exchangeName)
    {
        argumentsQueue["x-dead-letter-exchange"] = GetExchangeNameDlx(exchangeName);
        channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Fanout, durable: true);
        channel.QueueDeclare(queue: queue, durable: true, exclusive: false, autoDelete: false, arguments: argumentsQueue);
        channel.QueueBind(queue: queue, exchange: exchangeName, routingKey: string.Empty);
    }

    private static void ConfigQueueDlx(IModel channel, string queue, string exchangeName)
    {
        exchangeName = GetExchangeNameDlx(exchangeName);
        queue = GetQueueNameDlx(queue);

        channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Fanout, durable: true);
        channel.QueueDeclare(queue: queue, durable: true, exclusive: false, autoDelete: false, arguments: null);
        channel.QueueBind(queue: queue, exchange: exchangeName, routingKey: string.Empty);
    }

    public static string GetExchangeNameDlx(string exchangeName) => $"{exchangeName}.dlx";
    public static string GetQueueNameDlx(string queueName) => $"{queueName}.dlx";

    public Task UnsubscribeAsync<TEvent, TEventHandler>(CancellationToken cancellationToken)
        where TEvent : IDomainEvent
        where TEventHandler : IEventHandler<TEvent>
    {
        var consumerTag = this.channelProvider.GetConsumerTag<TEvent, TEventHandler>();

        if (!string.IsNullOrEmpty(consumerTag))
        {
            var channel = this.channelProvider.GetChannelConsumer<TEvent, TEventHandler>();
            channel.BasicCancel(consumerTag);
            logger.LogInformation("Unsubscribed from event: {TEvent}.", typeof(TEvent).Name);
        }

        return Task.CompletedTask;
    }
}
