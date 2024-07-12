using CodeDesignPlus.Net.Core.Abstractions;
using CodeDesignPlus.Net.Core.Abstractions.Options;
using CodeDesignPlus.Net.PubSub.Abstractions;
using CodeDesignPlus.Net.RabbitMQ.Attributes;
using CodeDesignPlus.Net.RabbitMQ.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Collections.Concurrent;
using System.Reflection;
using System.Text;

namespace CodeDesignPlus.Net.RabbitMQ.Services;

/// <summary>
/// Default implementation of the <see cref="IRabbitPubSubService"/>
/// </summary>
public class RabbitPubSubService : IRabbitPubSubService, IDisposable
{
    /// <summary>
    /// Logger Service
    /// </summary>
    private readonly ILogger<RabbitPubSubService> logger;
    private readonly IServiceProvider serviceProvider;
    private readonly IDomainEventResolverService domainEventResolverService;
    private readonly IRabbitConnection rabitConnection;
    private readonly CoreOptions coreOptions;
    private bool disposed = false;
    private readonly IModel channel;
    private readonly IBasicProperties properties;
    private readonly ConcurrentDictionary<string, string> consumerTags = new();
    private readonly Dictionary<string, object> argumentsQueue = new()
    {
        { "x-message-ttl", 1000 },
        { "x-ha-policy", "all" }
    };


    public RabbitPubSubService(ILogger<RabbitPubSubService> logger, IServiceProvider serviceProvider, IDomainEventResolverService domainEventResolverService, IRabbitConnection rabitConnection, IOptions<CoreOptions> coreOptions)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(serviceProvider);
        ArgumentNullException.ThrowIfNull(domainEventResolverService);
        ArgumentNullException.ThrowIfNull(rabitConnection);
        ArgumentNullException.ThrowIfNull(coreOptions);

        this.logger = logger;
        this.serviceProvider = serviceProvider;
        this.domainEventResolverService = domainEventResolverService;
        this.rabitConnection = rabitConnection;
        this.coreOptions = coreOptions.Value;

        this.channel = this.rabitConnection.Connection.CreateModel();

        this.properties = channel.CreateBasicProperties();
        properties.Persistent = true;

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
        var exchangeName = this.domainEventResolverService.GetKeyDomainEvent(@event.GetType());

        channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Fanout, durable: true);

        var message = JsonConvert.SerializeObject(@event);

        var body = Encoding.UTF8.GetBytes(message);

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
        var queueNameAttribute = typeof(TEventHandler).GetCustomAttribute<QueueNameAttribute>();
        var queueName = queueNameAttribute.GetQueueName(coreOptions.AppName, coreOptions.Business, coreOptions.Version);

        var exchangeName = this.domainEventResolverService.GetKeyDomainEvent(typeof(TEvent));

        ConfigQueue(queueName, exchangeName);
        ConfigQueueDlx(queueName, exchangeName);

        this.logger.LogInformation("Subscribed to event: {TEvent}.", typeof(TEvent).Name);

        var eventConsumer = new EventingBasicConsumer(channel);

        eventConsumer.Received += async (_, ea) => await RecivedEvent<TEvent, TEventHandler>(ea, cancellationToken);

        var consumerTag = channel.BasicConsume(queue: queueName, autoAck: false, consumer: eventConsumer);
        consumerTags.TryAdd($"{typeof(TEventHandler).FullName}", consumerTag);


        return Task.CompletedTask;
    }

    public async Task RecivedEvent<TEvent, TEventHandler>(BasicDeliverEventArgs eventArguments, CancellationToken cancellationToken)
        where TEvent : IDomainEvent
        where TEventHandler : IEventHandler<TEvent>
    {
        try
        {
            this.logger.LogDebug("Processing event: {TEvent}.", typeof(TEvent).Name);

            var body = eventArguments.Body.ToArray();

            var message = Encoding.UTF8.GetString(body);

            var @event = JsonConvert.DeserializeObject<TEvent>(message);

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

    private void ConfigQueue(string queue, string exchangeName)
    {
        argumentsQueue["x-dead-letter-exchange"] = GetExchangeNameDlx(exchangeName);
        channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Fanout, durable: true);
        channel.QueueDeclare(queue: queue, durable: true, exclusive: false, autoDelete: false, arguments: argumentsQueue);
        channel.QueueBind(queue: queue, exchange: exchangeName, routingKey: string.Empty);
    }

    private void ConfigQueueDlx(string queue, string exchangeName)
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
        var key = $"{typeof(TEventHandler).FullName}";

        if (consumerTags.TryRemove(key, out string consumerTag))
        {
            channel.BasicCancel(consumerTag);
            logger.LogInformation("Unsubscribed from event: {TEvent}.", typeof(TEvent).Name);
        }

        return Task.CompletedTask;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposed)
        {
            if (disposing)
            {
                this.channel.Dispose();
                this.rabitConnection.Dispose();
            }

            disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~RabbitPubSubService()
    {
        Dispose(false);
    }
}
