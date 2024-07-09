using CodeDesignPlus.Net.Core.Abstractions;
using CodeDesignPlus.Net.PubSub.Abstractions;
using CodeDesignPlus.Net.PubSub.Abstractions.Options;
using CodeDesignPlus.Net.RabitMQ.Abstractions.Options;
using CodeDesignPlus.Net.RabitMQ.Exceptions;
using CodeDesignPlus.Net.Serializers;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace CodeDesignPlus.Net.RabitMQ.Services;

/// <summary>
/// Default implementation of the <see cref="IRabitPubSubService"/>
/// </summary>
public class RabitPubSubService : IRabitPubSubService, IDisposable
{
    /// <summary>
    /// Logger Service
    /// </summary>
    private readonly ILogger<RabitPubSubService> logger;
    private readonly IServiceProvider serviceProvider;
    private readonly IDomainEventResolverService domainEventResolverService;
    private readonly IRabitConnection rabitConnection;
    private bool disposed = false;
    private readonly IModel channel;

    public RabitPubSubService(ILogger<RabitPubSubService> logger, IServiceProvider serviceProvider, IDomainEventResolverService domainEventResolverService, IRabitConnection rabitConnection)
    {
        this.logger = logger;
        this.serviceProvider = serviceProvider;
        this.domainEventResolverService = domainEventResolverService;
        this.rabitConnection = rabitConnection;

        this.channel = this.rabitConnection.Connection.CreateModel();

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

        channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Fanout);

        var message = JsonConvert.SerializeObject(@event);

        var body = Encoding.UTF8.GetBytes(message);

        channel.BasicPublish(
            exchange: exchangeName,
            routingKey: string.Empty,
            basicProperties: null,
            body: body
        );

        this.logger.LogInformation("Event {TEvent} published ", @event.GetType().Name);

        return Task.CompletedTask;
    }

    public Task SubscribeAsync<TEvent, TEventHandler>(CancellationToken cancellationToken)
        where TEvent : IDomainEvent
        where TEventHandler : IEventHandler<TEvent>
    {

        var exchangeName = this.domainEventResolverService.GetKeyDomainEvent(typeof(TEvent));

        channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Fanout);

        var queueName = channel.QueueDeclare(exclusive: false, autoDelete: false).QueueName;
        channel.QueueBind(queue: queueName, exchange: exchangeName, routingKey: string.Empty);

        this.logger.LogInformation("Subscribed to event: {TEvent}.", typeof(TEvent).Name);


        var eventConsumer = new EventingBasicConsumer(channel);

        eventConsumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            var @event = JsonConvert.DeserializeObject<TEvent>(message);

            if (@event is null)
                throw new RabitMQException($"The event {typeof(TEvent).Name} could not be deserialized.");

            var eventHandler = this.serviceProvider.GetRequiredService<TEventHandler>();

            await eventHandler.HandleAsync(@event, cancellationToken).ConfigureAwait(false);

            channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
        };

        channel.BasicConsume(queue: queueName, autoAck: false, consumer: eventConsumer);

        return Task.CompletedTask;
    }

    public Task UnsubscribeAsync<TEvent, TEventHandler>(CancellationToken cancellationToken)
        where TEvent : IDomainEvent
        where TEventHandler : IEventHandler<TEvent>
    {

        return Task.CompletedTask;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposed)
        {
            if (disposing)
            {
                // Liberar recursos administrados
                this.channel?.Dispose();
                this.rabitConnection?.Dispose();
            }

            // No hay recursos no administrados en esta clase

            disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~RabitPubSubService()
    {
        Dispose(false);
    }
}
