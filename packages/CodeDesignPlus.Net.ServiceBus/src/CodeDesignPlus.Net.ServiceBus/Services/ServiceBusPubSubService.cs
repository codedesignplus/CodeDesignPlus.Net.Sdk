using CodeDesignPlus.Net.Exceptions;

namespace CodeDesignPlus.Net.ServiceBus.Services;

/// <summary>
/// Service to publish and subscribe to domain events using Azure Service Bus topics and subscriptions.
/// </summary>
public class ServiceBusPubSubService : IServiceBusPubSub
{
    /// <summary>
    /// Largest payload the Standard tier accepts for a single message, in bytes.
    /// </summary>
    public const int MaxMessageSizeBytes = 256 * 1024;

    /// <summary>
    /// Reason recorded on a message dead-lettered because the handler rejected it on business grounds.
    /// </summary>
    public const string ReasonBusinessError = "BusinessError";

    /// <summary>
    /// Reason recorded on a message dead-lettered because it exhausted its retries.
    /// </summary>
    public const string ReasonMaxRetriesExceeded = "MaxRetriesExceeded";

    private readonly ILogger<ServiceBusPubSubService> logger;
    private readonly IServiceProvider serviceProvider;
    private readonly IDomainEventResolver domainEventResolverService;
    private readonly IServiceBusClientProvider clientProvider;
    private readonly ISubscriptionNameResolver subscriptionNameResolver;
    private readonly IEntityProvisioner entityProvisioner;
    private readonly CoreOptions coreOptions;
    private readonly ServiceBusOptions serviceBusOptions;
    private readonly IActivityService activityService;

    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceBusPubSubService"/> class.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="serviceProvider">The service provider.</param>
    /// <param name="domainEventResolverService">The domain event resolver service.</param>
    /// <param name="clientProvider">The Azure Service Bus client provider.</param>
    /// <param name="subscriptionNameResolver">The subscription name resolver.</param>
    /// <param name="entityProvisioner">The entity provisioner.</param>
    /// <param name="coreOptions">The core options.</param>
    /// <param name="serviceBusOptions">The Azure Service Bus options.</param>
    /// <param name="activityService">The activity service for distributed tracing (optional).</param>
    public ServiceBusPubSubService(ILogger<ServiceBusPubSubService> logger, IServiceProvider serviceProvider, IDomainEventResolver domainEventResolverService, IServiceBusClientProvider clientProvider, ISubscriptionNameResolver subscriptionNameResolver, IEntityProvisioner entityProvisioner, IOptions<CoreOptions> coreOptions, IOptions<ServiceBusOptions> serviceBusOptions, IActivityService activityService = null)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(serviceProvider);
        ArgumentNullException.ThrowIfNull(domainEventResolverService);
        ArgumentNullException.ThrowIfNull(clientProvider);
        ArgumentNullException.ThrowIfNull(subscriptionNameResolver);
        ArgumentNullException.ThrowIfNull(entityProvisioner);
        ArgumentNullException.ThrowIfNull(coreOptions);
        ArgumentNullException.ThrowIfNull(serviceBusOptions);

        this.logger = logger;
        this.serviceProvider = serviceProvider;
        this.domainEventResolverService = domainEventResolverService;
        this.clientProvider = clientProvider;
        this.subscriptionNameResolver = subscriptionNameResolver;
        this.entityProvisioner = entityProvisioner;
        this.coreOptions = coreOptions.Value;
        this.serviceBusOptions = serviceBusOptions.Value;
        this.activityService = activityService;

        this.logger.LogInformation("ServiceBusPubSubService initialized.");
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
        var tasks = @event.Select(x => this.PublishAsync(x, cancellationToken));

        return Task.WhenAll(tasks);
    }

    /// <summary>
    /// Publishes a domain event to its topic.
    /// </summary>
    /// <param name="event">The domain event to publish.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous publish operation.</returns>
    private async Task PrivatePublishAsync(IDomainEvent @event, CancellationToken cancellationToken)
    {
        var topic = this.domainEventResolverService.GetKeyDomainEvent(@event.GetType());

        await this.entityProvisioner.EnsureTopicAsync(topic, cancellationToken).ConfigureAwait(false);

        var activity = this.activityService?.StartActivity($"publish {topic}", ActivityKind.Producer);

        this.activityService?.Inject(activity, @event);

        activity?.AddTag("messaging.system", "servicebus");
        activity?.AddTag("messaging.operation.type", "publish");
        activity?.AddTag("messaging.destination.name", topic);
        activity?.AddTag("event.type", @event.GetType().Name);
        activity?.AddTag("event.id", @event.EventId.ToString());
        activity?.AddTag("event.aggregate_id", @event.AggregateId.ToString());

        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(@event));

        // Standard corta en 256 KB y devuelve un error generico del transporte. Se comprueba antes para que el
        // mensaje diga que evento fue y cuanto ocupaba, que es lo unico accionable.
        if (body.Length > MaxMessageSizeBytes)
        {
            activity?.SetStatus(ActivityStatusCode.Error, "Message too large");
            activity?.Stop();

            throw new ServiceBusPubSubException($"The event {@event.GetType().Name} serializes to {body.Length} bytes and exceeds the {MaxMessageSizeBytes} bytes accepted by Azure Service Bus.");
        }

        var message = new ServiceBusMessage(body)
        {
            MessageId = @event.EventId.ToString(),
            CorrelationId = Guid.NewGuid().ToString(),
            Subject = @event.GetType().Name,
            ContentType = "application/json"
        };

        var headers = new Dictionary<string, object>();
        this.activityService?.InjectToHeaders(activity, headers);

        foreach (var header in headers)
            message.ApplicationProperties[header.Key] = header.Value;

        message.ApplicationProperties["x-cdp-business"] = this.coreOptions.Business;
        message.ApplicationProperties["x-cdp-microservice"] = this.coreOptions.AppName;

        await this.clientProvider.GetSender(topic).SendMessageAsync(message, cancellationToken).ConfigureAwait(false);

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
        var topic = this.domainEventResolverService.GetKeyDomainEvent<TEvent>();
        var subscription = this.subscriptionNameResolver.GetSubscriptionName(typeof(TEventHandler));

        await this.entityProvisioner.EnsureSubscriptionAsync(topic, subscription, cancellationToken).ConfigureAwait(false);

        var processor = this.clientProvider.GetProcessor(topic, subscription);

        processor.ProcessMessageAsync += this.ReceivedEventAsync<TEvent, TEventHandler>;
        processor.ProcessErrorAsync += arguments =>
        {
            this.logger.LogError(arguments.Exception, "Error on the {Source} of the subscription {Subscription} of the topic {Topic}.", arguments.ErrorSource, subscription, topic);

            return Task.CompletedTask;
        };

        await processor.StartProcessingAsync(cancellationToken).ConfigureAwait(false);

        this.logger.LogInformation("Subscribed to event: {TEvent} on subscription {Subscription} of topic {Topic}.", typeof(TEvent).Name, subscription, topic);
    }

    /// <summary>
    /// Processes the received event.
    /// </summary>
    /// <typeparam name="TEvent">The type of the domain event.</typeparam>
    /// <typeparam name="TEventHandler">The type of the event handler.</typeparam>
    /// <param name="arguments">The arguments of the received message.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task ReceivedEventAsync<TEvent, TEventHandler>(ProcessMessageEventArgs arguments)
        where TEvent : IDomainEvent
        where TEventHandler : IEventHandler<TEvent>
    {
        Activity activity = null;

        var cancellationToken = arguments.CancellationToken;

        try
        {
            this.logger.LogDebug("Processing event: {TEvent}.", typeof(TEvent).Name);

            using var scope = this.serviceProvider.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<IEventContext>();

            var @event = JsonSerializer.Deserialize<TEvent>(arguments.Message.Body.ToString());

            var parentContext = this.activityService?.Extract(@event);

            if (parentContext?.ActivityContext == default && arguments.Message.ApplicationProperties != null)
                parentContext = this.activityService?.ExtractFromHeaders(new Dictionary<string, object>(arguments.Message.ApplicationProperties));

            activity = this.activityService?.StartActivity($"consume {typeof(TEvent).Name}", ActivityKind.Consumer, parentContext);

            activity?.AddTag("messaging.system", "servicebus");
            activity?.AddTag("messaging.operation.type", "process");
            activity?.AddTag("event.type", typeof(TEvent).Name);
            activity?.AddTag("event.id", @event.EventId.ToString());
            activity?.AddTag("event.aggregate_id", @event.AggregateId.ToString());

            context.SetCurrentDomainEvent(@event);

            await scope.ServiceProvider.InitializeEventScopeAsync(@event, cancellationToken).ConfigureAwait(false);

            var eventHandler = scope.ServiceProvider.GetRequiredService<TEventHandler>();

            await eventHandler.HandleAsync(@event, cancellationToken).ConfigureAwait(false);

            await arguments.CompleteMessageAsync(arguments.Message, cancellationToken).ConfigureAwait(false);

            activity?.SetStatus(ActivityStatusCode.Ok);
        }
        catch (Exception exception)
        {
            await this.HandleFailureAsync<TEvent>(arguments, exception, activity, cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            activity?.Stop();
        }
    }

    /// <summary>
    /// Decides whether a failed message is retried or dead-lettered, and waits before retrying.
    /// </summary>
    /// <typeparam name="TEvent">The type of the domain event.</typeparam>
    /// <param name="arguments">The arguments of the received message.</param>
    /// <param name="exception">The exception raised while processing the message.</param>
    /// <param name="activity">The activity of the current consumption, if any.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    private async Task HandleFailureAsync<TEvent>(ProcessMessageEventArgs arguments, Exception exception, Activity activity, CancellationToken cancellationToken)
    {
        var isBusinessError = exception is CodeDesignPlusException;

        activity?.SetStatus(ActivityStatusCode.Error, exception.Message);

        if (isBusinessError)
            this.logger.LogWarning(exception, "Business error processing event: {TEvent} | {Message}. Sending to DLQ.", typeof(TEvent).Name, exception.Message);
        else
            this.logger.LogError(exception, "Infrastructure error processing event: {TEvent} | {Message}.", typeof(TEvent).Name, exception.Message);

        var deliveryCount = arguments.Message.DeliveryCount;
        var maxRetries = this.serviceBusOptions.MaxRetry;

        if (isBusinessError || deliveryCount >= maxRetries)
        {
            this.logger.LogError(
                "Event {TEvent} sent to DLQ. Reason: {Reason}. Delivery count: {DeliveryCount}/{MaxRetries}.",
                typeof(TEvent).Name,
                isBusinessError ? "Business error (non-retryable)" : "Max retries exceeded",
                deliveryCount,
                maxRetries
            );

            await arguments.DeadLetterMessageAsync(
                arguments.Message,
                isBusinessError ? ReasonBusinessError : ReasonMaxRetriesExceeded,
                exception.Message,
                cancellationToken).ConfigureAwait(false);

            return;
        }

        var delay = this.GetRetryDelay(deliveryCount);

        this.logger.LogWarning(
            "Event {TEvent} will be retried in {Delay}. Delivery count: {DeliveryCount}/{MaxRetries}.",
            typeof(TEvent).Name,
            delay,
            deliveryCount + 1,
            maxRetries
        );

        // La espera transcurre aqui, con el bloqueo del mensaje retenido y renovandose solo. Reprogramar una
        // copia y completar el original seria mas barato en concurrencia, pero completar y reprogramar no son
        // atomicos: una caida entre ambos pierde el evento.
        await Task.Delay(delay, cancellationToken).ConfigureAwait(false);

        await arguments.AbandonMessageAsync(arguments.Message, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Computes the exponential backoff applied before the next delivery.
    /// </summary>
    /// <param name="deliveryCount">The number of times the broker already delivered the message.</param>
    /// <returns>The delay to wait before abandoning the message.</returns>
    /// <remarks>
    /// Sin esta espera los reintentos se consumen en milisegundos y un fallo pasajero de un segundo agota las
    /// entregas antes de que el origen se recupere.
    /// </remarks>
    public TimeSpan GetRetryDelay(int deliveryCount)
    {
        var exponent = Math.Min(Math.Max(deliveryCount - 1, 0), 30);

        var delay = (double)this.serviceBusOptions.RetryIntervalMs * Math.Pow(2, exponent);

        var capped = Math.Min(delay, this.serviceBusOptions.MaxRetryIntervalMs);

        // Sin dispersion, todas las replicas que fallaron a la vez reintentan a la vez y vuelven a tumbar el
        // recurso que se estaba recuperando.
        var jitter = capped * 0.2 * Random.Shared.NextDouble();

        return TimeSpan.FromMilliseconds(capped + jitter);
    }

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
        var topic = this.domainEventResolverService.GetKeyDomainEvent<TEvent>();
        var subscription = this.subscriptionNameResolver.GetSubscriptionName(typeof(TEventHandler));

        var processor = this.clientProvider.FindProcessor(topic, subscription);

        if (processor is not null && processor.IsProcessing)
        {
            await processor.StopProcessingAsync(cancellationToken).ConfigureAwait(false);

            this.logger.LogInformation("Unsubscribed from event: {TEvent}.", typeof(TEvent).Name);
        }
    }
}
