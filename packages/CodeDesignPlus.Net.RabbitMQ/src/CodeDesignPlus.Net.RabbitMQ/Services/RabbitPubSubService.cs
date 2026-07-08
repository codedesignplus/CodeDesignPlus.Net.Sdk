using CodeDesignPlus.Net.Core.Abstractions.Attributes;
using CodeDesignPlus.Net.Exceptions;
using RabbitMQ.Client.Exceptions;

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
    private readonly RabbitMQOptions rabbitMQOptions;
    private readonly IChannelProvider channelProvider;
    private readonly Dictionary<string, object> argumentsQueue;
    private readonly IActivityService activityService;

    /// <summary>
    /// Initializes a new instance of the <see cref="RabbitPubSubService"/> class.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="serviceProvider">The service provider.</param>
    /// <param name="domainEventResolverService">The domain event resolver service.</param>
    /// <param name="channelProvider">The channel provider.</param>
    /// <param name="coreOptions">The core options.</param>
    /// <param name="rabbitMQOptions">The RabbitMQ options.</param>
    /// <param name="activityService">The activity service for distributed tracing (optional).</param>
    public RabbitPubSubService(ILogger<RabbitPubSubService> logger, IServiceProvider serviceProvider, IDomainEventResolver domainEventResolverService, IChannelProvider channelProvider, IOptions<CoreOptions> coreOptions, IOptions<RabbitMQOptions> rabbitMQOptions, IActivityService activityService = null)
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
        this.activityService = activityService;
        this.rabbitMQOptions = rabbitMQOptions.Value;

        this.argumentsQueue = this.rabbitMQOptions.QueueArguments.GetArguments();

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

        return this.PrivatePublishAsync(@event, cancellationToken);
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
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous publish operation.</returns>
    private async Task PrivatePublishAsync(IDomainEvent @event, CancellationToken cancellationToken)
    {
        var channel = await this.channelProvider.GetChannelPublishAsync(@event.GetType(), cancellationToken);

        var exchangeName = await this.channelProvider.ExchangeDeclareAsync(@event.GetType(), cancellationToken);

        var activity = this.activityService?.StartActivity($"publish {exchangeName}", ActivityKind.Producer);

        this.activityService?.Inject(activity, @event);

        activity?.AddTag("messaging.system", "rabbitmq");
        activity?.AddTag("messaging.operation.type", "publish");
        activity?.AddTag("messaging.destination.name", exchangeName);
        activity?.AddTag("event.type", @event.GetType().Name);
        activity?.AddTag("event.id", @event.EventId.ToString());
        activity?.AddTag("event.aggregate_id", @event.AggregateId.ToString());

        var message = JsonSerializer.Serialize(@event);

        var body = Encoding.UTF8.GetBytes(message);

        var headers = new Dictionary<string, object>();
        this.activityService?.InjectToHeaders(activity, headers);

        var properties = new BasicProperties
        {
            Headers = headers,
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
            body: body,
            cancellationToken: cancellationToken
        );

        activity?.SetStatus(ActivityStatusCode.Ok);
        activity?.Stop();

        this.logger.LogInformation("Event {TEvent} published", @event.GetType().Name);
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
        await this.channelProvider.ExchangeDeclareAsync(typeof(TEvent), cancellationToken);

        var channel = await this.channelProvider.GetChannelConsumerAsync<TEvent, TEventHandler>(cancellationToken);

        var queueNameAttribute = typeof(TEventHandler).GetCustomAttribute<QueueNameAttribute>();
        var queueName = queueNameAttribute.GetQueueName(coreOptions.AppName, coreOptions.Business, coreOptions.Version);

        var exchangeName = this.domainEventResolverService.GetKeyDomainEvent<TEvent>();

        await ConfigQueueDlxAsync(channel, queueName, exchangeName);
        await ConfigQueueAsync(channel, queueName, exchangeName);

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
        Activity activity = null;

        try
        {
            this.logger.LogDebug("Processing event: {TEvent}.", typeof(TEvent).Name);

            using var scope = this.serviceProvider.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<IEventContext>();

            var body = eventArguments.Body.ToArray();

            var message = Encoding.UTF8.GetString(body);

            var @event = JsonSerializer.Deserialize<TEvent>(message);

            var parentContext = this.activityService?.Extract(@event);

            if (parentContext?.ActivityContext == default && eventArguments.BasicProperties.Headers != null)
                parentContext = this.activityService?.ExtractFromHeaders(eventArguments.BasicProperties.Headers);

            activity = this.activityService?.StartActivity($"consume {typeof(TEvent).Name}", ActivityKind.Consumer, parentContext);

            activity?.AddTag("messaging.system", "rabbitmq");
            activity?.AddTag("messaging.operation.type", "process");
            activity?.AddTag("event.type", typeof(TEvent).Name);
            activity?.AddTag("event.id", @event.EventId.ToString());
            activity?.AddTag("event.aggregate_id", @event.AggregateId.ToString());

            context.SetCurrentDomainEvent(@event);

            var eventHandler = scope.ServiceProvider.GetRequiredService<TEventHandler>();

            await eventHandler.HandleAsync(@event, cancellationToken).ConfigureAwait(false);

            await channel.BasicAckAsync(deliveryTag: eventArguments.DeliveryTag, multiple: false, cancellationToken: cancellationToken);

            activity?.SetStatus(ActivityStatusCode.Ok);
        }
        catch (Exception ex)
        {
            var isBusinessError = ex is CodeDesignPlusException;

            if (isBusinessError)
            {
                activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
                this.logger.LogWarning(ex, "Business error processing event: {TEvent} | {Message}. Sending to DLQ.", typeof(TEvent).Name, ex.Message);
            }
            else
            {
                activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
                this.logger.LogError(ex, "Infrastructure error processing event: {TEvent} | {Message}.", typeof(TEvent).Name, ex.Message);
            }

            var retryCount = GetDeliveryCount(eventArguments.BasicProperties);
            var maxRetries = this.rabbitMQOptions.MaxRetry;

            if (isBusinessError || retryCount >= maxRetries)
            {
                this.logger.LogError(
                    "Event {TEvent} sent to DLQ. Reason: {Reason}. Delivery count: {RetryCount}/{MaxRetries}.",
                    typeof(TEvent).Name,
                    isBusinessError ? "Business error (non-retryable)" : "Max retries exceeded",
                    retryCount,
                    maxRetries
                );

                await channel.BasicNackAsync(deliveryTag: eventArguments.DeliveryTag, multiple: false, requeue: false, cancellationToken: cancellationToken);
            }
            else
            {
                this.logger.LogWarning(
                    "Event {TEvent} will be requeued. Delivery count: {RetryCount}/{MaxRetries}.",
                    typeof(TEvent).Name,
                    retryCount + 1,
                    maxRetries
                );

                await channel.BasicNackAsync(deliveryTag: eventArguments.DeliveryTag, multiple: false, requeue: true, cancellationToken: cancellationToken);
            }
        }
        finally
        {
            activity?.Stop();
        }
    }

    /// <summary>
    /// Gets the delivery count from the message properties.
    /// Uses x-delivery-count (quorum queues) or falls back to 0.
    /// </summary>
    private static int GetDeliveryCount(IReadOnlyBasicProperties properties)
    {
        if (properties.Headers is null)
            return 0;

        if (properties.Headers.TryGetValue("x-delivery-count", out var value))
        {
            return value switch
            {
                int intVal => intVal,
                long longVal => (int)longVal,
                _ => 0
            };
        }

        return 0;
    }

    /// <summary>
    /// Configures the RabbitMQ queue.
    /// </summary>
    /// <param name="channel">The RabbitMQ channel.</param>
    /// <param name="queue">The queue name.</param>
    /// <param name="exchangeName">The exchange name.</param>
    private async Task ConfigQueueAsync(IChannel channel, string queue, string exchangeName)
    {
        var arguments = new Dictionary<string, object>(this.argumentsQueue)
        {
            ["x-dead-letter-exchange"] = GetExchangeNameDlx(exchangeName)
        };
        await channel.ExchangeDeclareAsync(exchange: exchangeName, type: ExchangeType.Fanout, durable: true);
        await channel.QueueDeclareAsync(queue: queue, durable: true, exclusive: false, autoDelete: false, arguments: arguments);
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
            var channel = await this.channelProvider.GetChannelConsumerAsync<TEvent, TEventHandler>(cancellationToken);
            await channel.BasicCancelAsync(consumerTag, cancellationToken: cancellationToken);
            logger.LogInformation("Unsubscribed from event: {TEvent}.", typeof(TEvent).Name);
        }
    }
}
