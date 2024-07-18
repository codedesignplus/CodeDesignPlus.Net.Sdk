namespace CodeDesignPlus.Net.Kafka.Services;

/// <summary>
/// Provides the default implementation for the <see cref="IKafkaService"/> interface.
/// </summary>
public class KafkaPubSub : IKafkaPubSub
{
    private readonly IDomainEventResolverService domainEventResolverService;
    private readonly ILogger<KafkaPubSub> logger;
    private readonly KafkaOptions options;
    private readonly IServiceProvider serviceProvider;


    /// <summary>
    /// Initializes a new instance of the <see cref="KafkaPubSub"/> class.
    /// </summary>
    /// <param name="logger">Service for logging.</param>
    /// <param name="options">Configuration options for Kafka.</param>
    /// <param name="serviceProvider">Provides an instance of a service.</param>
    /// <param name="pubSubOptions">Configuration options for the event bus.</param>
    public KafkaPubSub(ILogger<KafkaPubSub> logger, IDomainEventResolverService domainEventResolverService, IOptions<KafkaOptions> options, IServiceProvider serviceProvider)
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
    /// <param name="event">The event to be published.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
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
    /// Publish a list of domain events
    /// </summary>
    /// <param name="event">Domains event to publish</param>
    /// <param name="cancellationToken">The cancellation token that will be assigned to the new task.</param>
    /// <returns>Return a <see cref="Task"/></returns>
    public Task PublishAsync(IReadOnlyList<IDomainEvent> @event, CancellationToken cancellationToken)
    {
        var tasks = @event.Select(@event => this.PublishAsync(@event, cancellationToken));

        return Task.WhenAll(tasks);
    }

    /// <summary>
    /// Subscribes to a specific event from Kafka.
    /// </summary>
    /// <typeparam name="TEvent">The type of event.</typeparam>
    /// <typeparam name="TEventHandler">The type of event handler.</typeparam>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task SubscribeAsync<TEvent, TEventHandler>(CancellationToken cancellationToken)
        where TEvent : IDomainEvent
        where TEventHandler : IEventHandler<TEvent>
    {
        var topic = this.domainEventResolverService.GetKeyDomainEvent<TEvent>();

        this.logger.LogInformation("{EventType} | Subscribing to Kafka topic {topic} ", typeof(TEvent).Name, topic);

        await WaitTopicCreatedAsync<TEvent>(topic, cancellationToken).ConfigureAwait(false);

        await SubscribeTopicAsync<TEvent, TEventHandler>(topic, cancellationToken).ConfigureAwait(false);
    }

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
                return; // O manejar según sea apropiado (ej. lanzar una excepción)
            }

            this.logger.LogInformation("{EventType} | The topic {Topic} does not exist, waiting for it to be created.", typeof(TEvent).Name, topic);
            await Task.Delay(1000, CancellationToken.None).ConfigureAwait(false);
        }
    }


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
                this.logger.LogInformation("{EventType} | Listener the event {topic}", typeof(TEvent).Name, topic);

                var value = consumer.Consume(cancellationToken);

                var eventHandler = this.serviceProvider.GetRequiredService<TEventHandler>();

                await eventHandler.HandleAsync(value.Message.Value, cancellationToken).ConfigureAwait(false);

                this.logger.LogInformation("{EventType} | End Listener the event {topic}", typeof(TEvent).Name, topic);

            }
            catch (ConsumeException e)
            {
                this.logger.LogError(e, "{EventType} | An error occurred while consuming a Kafka message for event topic: {topic}", typeof(TEvent).Name, topic);
            }
            catch (Exception e)
            {
                this.logger.LogError(e, "{EventType} | An unexpected error occurred for topic: {topic}", typeof(TEvent).Name, topic);
            }
        }

        this.logger.LogInformation("{EventType} | Kafka event listening has stopped for topic: {topic} due to cancellation request.", typeof(TEvent).Name, topic);
    }

    internal ConsumerBuilder<string, TEvent> GetConsumer<TEvent>() where TEvent : IDomainEvent
    {
        var consumerBuilder = new ConsumerBuilder<string, TEvent>(this.options.ConsumerConfig);

        consumerBuilder.SetValueDeserializer(new JsonSystemTextSerializer<TEvent>());
        return consumerBuilder;
    }

    /// <summary>
    /// Unsubscribes from a specific event in Kafka.
    /// </summary>
    /// <typeparam name="TEvent">The type of event.</typeparam>
    /// <typeparam name="TEventHandler">The type of event handler.</typeparam>
    /// <param name="cancellationToken">Cancellation token.</param>
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
