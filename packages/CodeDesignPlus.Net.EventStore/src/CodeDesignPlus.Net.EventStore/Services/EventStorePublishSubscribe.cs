using System.Text;
using CodeDesignPlus.Net.Event.Bus.Abstractions;
using CodeDesignPlus.Net.Event.Bus.Options;
using CodeDesignPlus.Net.EventStore.Options;
using EventStore.ClientAPI;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace CodeDesignPlus.Net.EventStore;

public class EventStorePublishSubscribe : IEventBus
{
    private IEventStoreFactory eventStoreFactory;
    private ISubscriptionManager subscriptionManager;
    private IServiceProvider serviceProvider;
    private ILogger<EventStorePublishSubscribe> logger;
    private EventBusOptions eventBusOptions;

    public EventStorePublishSubscribe(
        IEventStoreFactory eventStoreFactory,
        ISubscriptionManager subscriptionManager,
        IServiceProvider serviceProvider,
        ILogger<EventStorePublishSubscribe> logger,
        IOptions<EventStoreOptions> options,
        IOptions<EventBusOptions> eventBusOptions)
    {
        if (eventStoreFactory == null)
            throw new ArgumentNullException(nameof(eventStoreFactory));

        if (options == null)
            throw new ArgumentNullException(nameof(options));

        if (eventBusOptions == null)
            throw new ArgumentNullException(nameof(eventBusOptions));

        this.eventStoreFactory = eventStoreFactory;

        this.subscriptionManager = subscriptionManager ?? throw new ArgumentNullException(nameof(subscriptionManager));
        this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.eventBusOptions = eventBusOptions.Value;

        this.logger.LogInformation("RedisEventBusService initialized.");
    }


    public async Task PublishAsync(EventBase @event, CancellationToken token)
    {
        var connection = await this.eventStoreFactory.CreateAsync(EventStoreFactoryConst.Core);

        var stream = @event.GetType().Name;

        var eventData = new EventData(
            @event.IdEvent,
            @event.GetType().Name,
            true,
            Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(@event)),
            null);

        await connection.AppendToStreamAsync(stream, ExpectedVersion.Any, eventData);
    }

    public async Task SubscribeAsync<TEvent, TEventHandler>(CancellationToken token)
        where TEvent : EventBase
        where TEventHandler : IEventHandler<TEvent>
    {
        var connection = await this.eventStoreFactory.CreateAsync(EventStoreFactoryConst.Core);

        var stream = typeof(TEvent).Name;

        await connection.SubscribeToStreamAsync(
            stream,
            true,
            (suscription, @event) => EventAppearedAsync<TEvent, TEventHandler>(suscription, @event, token),
            (subscription, reason, exception) => SubscriptionDropped<TEvent, TEventHandler>(subscription, reason, exception, token)
        );
    }

    private void SubscriptionDropped<TEvent, TEventHandler>(EventStoreSubscription subscription, SubscriptionDropReason reason, Exception exception, CancellationToken token)
    where TEvent : EventBase
    where TEventHandler : IEventHandler<TEvent>
    {
        switch (reason)
        {
            case SubscriptionDropReason.UserInitiated:
                // La suscripción se canceló de forma manual (por tu código).
                logger.LogInformation($"La suscripción a '{typeof(TEvent).Name}' se canceló manualmente.");
                break;

            case SubscriptionDropReason.ConnectionClosed:
                // La conexión con EventStore se cerró.
                logger.LogError($"La conexión con EventStore se cerró. Intentando restablecer la suscripción a '{typeof(TEvent).Name}'.");
                // Puedes intentar restablecer la suscripción aquí si es apropiado.
                ReconnectSubscription<TEvent, TEventHandler>(subscription.StreamId, token);
                break;

            case SubscriptionDropReason.CatchUpError:
                // Hubo un error en la captura.
                logger.LogError($"Error en la captura de eventos para '{typeof(TEvent).Name}': {exception.Message}");

                break;

            case SubscriptionDropReason.SubscribingError:
                // Hubo un error en el suscriptor (manejo de eventos).
                logger.LogError($"Error en el suscriptor de eventos para '{typeof(TEvent).Name}': {exception.Message}");
                ReconnectSubscription<TEvent, TEventHandler>(subscription.StreamId, token);

                break;

            default:
                // Otro motivo de desconexión no manejado.
                logger.LogWarning($"La suscripción a '{typeof(TEvent).Name}' se canceló debido a una razón no manejada: {reason}");
                break;
        }
    }


    private async void ReconnectSubscription<TEvent, TEventHandler>(string streamId, CancellationToken token)
        where TEvent : EventBase
        where TEventHandler : IEventHandler<TEvent>
    {
        try
        {
            // Puedes intentar restablecer la suscripción aquí.
            await SubscribeAsync<TEvent, TEventHandler>(token);
        }
        catch (Exception ex)
        {
            logger.LogError($"Error al intentar restablecer la suscripción a '{typeof(TEvent).Name}': {ex.Message}");
        }
    }

    private async Task EventAppearedAsync<TEvent, TEventHandler>(EventStoreSubscription subscription, ResolvedEvent @event, CancellationToken token)
        where TEvent : EventBase
        where TEventHandler : IEventHandler<TEvent>
    {
        var domainEvent = JsonConvert.DeserializeObject<TEvent>(Encoding.UTF8.GetString(@event.Event.Data));

        var projectionHandler = this.serviceProvider.GetRequiredService<TEventHandler>();

        await projectionHandler.HandleAsync(domainEvent, token);
    }

    public void Unsubscribe<TEvent, TEventHandler>()
        where TEvent : EventBase
        where TEventHandler : IEventHandler<TEvent>
    {

    }
}
