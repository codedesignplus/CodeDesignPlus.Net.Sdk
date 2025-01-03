namespace CodeDesignPlus.Net.Kafka.Services;

/// <summary>
/// KafkaPubSub service for publishing and subscribing to Kafka topics.
/// </summary>
public class KafkaPubSub : IKafkaPubSub
{
    private readonly IDomainEventResolver domainEventResolverService;
    private readonly ILogger<KafkaPubSub> logger;
    private readonly KafkaOptions options;
    private readonly IServiceProvider serviceProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="KafkaPubSub"/> class.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="domainEventResolverService">The domain event resolver service.</param>
    /// <param name="options">The Kafka options.</param>
    /// <param name="serviceProvider">The service provider.</param>
    /// <exception cref="ArgumentNullException">Thrown when any of the parameters are null.</exception>
    public KafkaPubSub(ILogger<KafkaPubSub> logger, IDomainEventResolver domainEventResolverService, IOptions<KafkaOptions> options, IServiceProvider serviceProvider)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(serviceProvider);
        ArgumentNullException.ThrowIfNull(domainEventResolverService);

        this.domainEventResolverService = domainEventResolverService;
        this.logger = logger;
        this.serviceProvider = serviceProvider;
        this.options = options.Value;
    }

    /// <summary>
    /// Publishes an event to Kafka.
    /// </summary>
    /// <param name="event">The event to publish.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous publish operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the event is null.</exception>
    public async Task PublishAsync(IDomainEvent @event, CancellationToken cancellationToken)
    {
        var type = @event.GetType();

        this.logger.LogInformation("Starting to publish event to Kafka. Event type: {EventType}", type.Name);

        var topic = this.domainEventResolverService.GetKeyDomainEvent(type);

        var headers = new Headers
        {
            { "OccurredAt", Encoding.UTF8.GetBytes(@event.OccurredAt.ToString()) },
            { "EventId", Encoding.UTF8.GetBytes(@event.EventId.ToString()) },
        };

        foreach (var item in @event.Metadata.Where(x => x.Value != null))
        {
            headers.Add(item.Key, Encoding.UTF8.GetBytes(item.Value.ToString()));
        }

        var message = new Message<string, IDomainEvent>
        {
            Key = @event.EventId.ToString(),
            Value = @event,
            Headers = headers
        };

        var producer = this.serviceProvider.GetRequiredService<IProducer<string, IDomainEvent>>();

        await producer.ProduceAsync(topic, message, cancellationToken).ConfigureAwait(false);

        this.logger.LogInformation("Event published to Kafka successfully. Event type: {EventType}", @event.GetType().Name);
    }

    /// <summary>
    /// Publishes a list of events to Kafka.
    /// </summary>
    /// <param name="event">The list of events to publish.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous publish operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the events list is null.</exception>
    public Task PublishAsync(IReadOnlyList<IDomainEvent> @event, CancellationToken cancellationToken)
    {
        var tasks = @event.Select(@event => this.PublishAsync(@event, cancellationToken));

        return Task.WhenAll(tasks);
    }

    /// <summary>
    /// Subscribes to a Kafka topic for a specific event type and event handler.
    /// </summary>
    /// <typeparam name="TEvent">The type of the event.</typeparam>
    /// <typeparam name="TEventHandler">The type of the event handler.</typeparam>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous subscribe operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the event type or event handler is null.</exception>
    public async Task SubscribeAsync<TEvent, TEventHandler>(CancellationToken cancellationToken)
        where TEvent : IDomainEvent
        where TEventHandler : IEventHandler<TEvent>
    {
        var topic = this.domainEventResolverService.GetKeyDomainEvent<TEvent>();

        this.logger.LogInformation("{EventType} | Subscribing to Kafka topic {Topic} ", typeof(TEvent).Name, topic);

        await WaitTopicCreatedAsync<TEvent>(topic, cancellationToken).ConfigureAwait(false);

        await SubscribeTopicAsync<TEvent, TEventHandler>(topic, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Waits for a Kafka topic to be created.
    /// </summary>
    /// <typeparam name="TEvent">The type of the event.</typeparam>
    /// <param name="topic">The Kafka topic.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous wait operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the topic is null.</exception>
    internal async Task WaitTopicCreatedAsync<TEvent>(string topic, CancellationToken cancellationToken) where TEvent : IDomainEvent
    {
        using var adminClient = new AdminClientBuilder(this.options.AdminClientConfig).Build();

        var attempt = 0;

        while (!cancellationToken.IsCancellationRequested)
        {
            var metadata = adminClient.GetMetadata(TimeSpan.FromSeconds(10));
            bool topicExists = metadata.Topics.Exists(t => t.Topic == topic);

            if (topicExists)
            {
                break;
            }

            attempt++;
            if (attempt >= this.options.MaxAttempts)
            {
                this.logger.LogWarning("{EventType} | The topic {Topic} does not exist after {MaxAttempts} attempts. Exiting.", typeof(TEvent).Name, topic, this.options.MaxAttempts);

                return;
            }

            this.logger.LogInformation("{EventType} | The topic {Topic} does not exist, waiting for it to be created.", typeof(TEvent).Name, topic);

            await Task.Delay(1000, CancellationToken.None).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Subscribes to a Kafka topic for a specific event type and event handler.
    /// </summary>
    /// <typeparam name="TEvent">The type of the event.</typeparam>
    /// <typeparam name="TEventHandler">The type of the event handler.</typeparam>
    /// <param name="topic">The Kafka topic.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous subscribe operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the topic is null.</exception>
    internal async Task SubscribeTopicAsync<TEvent, TEventHandler>(string topic, CancellationToken cancellationToken)
        where TEvent : IDomainEvent
        where TEventHandler : IEventHandler<TEvent>
    {
        using var consumer = GetConsumer<TEvent>().Build();

        cancellationToken.Register(consumer.Close);

        consumer.Subscribe(topic);

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                this.logger.LogInformation("{EventType} | Listener the event {Topic}", typeof(TEvent).Name, topic);

                var value = consumer.Consume(cancellationToken);

                var eventHandler = this.serviceProvider.GetRequiredService<TEventHandler>();

                await eventHandler.HandleAsync(value.Message.Value, cancellationToken).ConfigureAwait(false);

                this.logger.LogInformation("{EventType} | End Listener the event {Topic}", typeof(TEvent).Name, topic);
            }
            catch (Exception e)
            {
                this.logger.LogError(e, "{EventType} | An error occurred while consuming a Kafka message for event topic: {Topic}", typeof(TEvent).Name, topic);
            }
        }
    }

    /// <summary>
    /// Gets a Kafka consumer for a specific event type.
    /// </summary>
    /// <typeparam name="TEvent">The type of the event.</typeparam>
    /// <returns>A consumer builder for the specified event type.</returns>
    internal ConsumerBuilder<string, TEvent> GetConsumer<TEvent>() where TEvent : IDomainEvent
    {
        var consumerBuilder = new ConsumerBuilder<string, TEvent>(this.options.ConsumerConfig);

        consumerBuilder.SetValueDeserializer(new JsonSystemTextSerializer<TEvent>());
        return consumerBuilder;
    }

    /// <summary>
    /// Unsubscribes from a Kafka topic for a specific event type and event handler.
    /// </summary>
    /// <typeparam name="TEvent">The type of the event.</typeparam>
    /// <typeparam name="TEventHandler">The type of the event handler.</typeparam>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous unsubscribe operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the event type or event handler is null.</exception>
    public Task UnsubscribeAsync<TEvent, TEventHandler>(CancellationToken cancellationToken)
        where TEvent : IDomainEvent
        where TEventHandler : IEventHandler<TEvent>
    {
        this.logger.LogInformation("Unsubscribing from event {EventType} for handler {EventHandlerType}", typeof(TEvent).Name, typeof(TEventHandler).Name);

        var consumer = this.serviceProvider.GetRequiredService<IConsumer<string, TEvent>>();
        consumer.Unsubscribe();

        return Task.CompletedTask;
    }
}
