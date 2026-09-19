using Azure.Messaging.ServiceBus;
using CodeDesignPlus.Net.Core.Extensions;
using CodeDesignPlus.Net.ServiceBus.Test.Helpers.Events;
using CodeDesignPlus.Net.xUnit.Containers.ServiceBusContainer;

namespace CodeDesignPlus.Net.ServiceBus.Test.Services;

[Collection(ServiceBusCollectionFixture.Collection)]
public class ServiceBusPubSubServiceIntegrationTest(ServiceBusCollectionFixture fixture)
{
    private readonly ServiceBusCollectionFixture fixture = fixture;

    /// <summary>
    /// Builds a provider wired to the emulator.
    /// </summary>
    /// <remarks>
    /// El emulador rechaza <c>MaxDeliveryCount</c> por encima de 10 y un TTL de mas de una hora, que son los
    /// valores de produccion. Se bajan aqui: es una limitacion del emulador, no de Azure.
    /// </remarks>
    private ServiceProvider BuildProvider(int maxRetry = 4, int retryIntervalMs = 1000, int maxRetryIntervalMs = 4000)
    {
        var configuration = ConfigurationUtil.GetConfiguration(new
        {
            Core = ConfigurationUtil.CoreOptions,
            ServiceBus = new
            {
                Enable = true,
                ConnectionString = this.fixture.Container.ConnectionString,
                ManagementConnectionString = this.fixture.Container.ManagementConnectionString,
                RegisterHealthCheck = false,
                RegisterAutomaticHandlers = false,
                UseQueue = false,
                MaxRetry = maxRetry,
                RetryIntervalMs = retryIntervalMs,
                MaxRetryIntervalMs = maxRetryIntervalMs,
                MessageTimeToLiveHours = 1,
                MaxConcurrentCalls = 1
            }
        });

        var services = new ServiceCollection();

        services.AddLogging();
        services.AddCore(configuration);
        services.AddSingleton<IMemoryHandler, MemoryHandler>();
        services.AddTransient<UserCreatedDomainEventHandler>();
        services.AddTransient<OrderCreatedDomainEventHandler>();
        services.AddServiceBus<ServiceBusPubSubServiceIntegrationTest>(configuration);

        return services.BuildServiceProvider();
    }

    private static async Task WaitUntilAsync(Func<bool> condition, TimeSpan timeout)
    {
        var deadline = DateTime.UtcNow.Add(timeout);

        while (DateTime.UtcNow < deadline)
        {
            if (condition())
                return;

            await Task.Delay(200);
        }
    }

    [Fact]
    public async Task SubscribeAsync_PublishEvent_HandlerReceivesIt()
    {
        await using var provider = this.BuildProvider();
        var message = provider.GetRequiredService<IMessage>();
        var memory = provider.GetRequiredService<IMemoryHandler>();

        await message.SubscribeAsync<UserCreatedDomainEvent, UserCreatedDomainEventHandler>(CancellationToken.None);

        var @event = new UserCreatedDomainEvent(Guid.NewGuid(), "Wilson");

        await message.PublishAsync(@event, CancellationToken.None);

        await WaitUntilAsync(() => memory.Memory.ContainsKey(@event.AggregateId), TimeSpan.FromSeconds(30));

        var received = Assert.IsType<UserCreatedDomainEvent>(memory.Memory[@event.AggregateId]);

        Assert.Equal(@event.AggregateId, received.AggregateId);
        Assert.Equal(@event.EventId, received.EventId);
        Assert.Equal("Wilson", received.Name);

        await message.UnsubscribeAsync<UserCreatedDomainEvent, UserCreatedDomainEventHandler>(CancellationToken.None);
    }

    [Fact]
    public async Task SubscribeAsync_AutoProvision_CreatesTopicAndSubscription()
    {
        // Evento propio: si compartiera la suscripcion con las demas pruebas, la primera que la creara fijaria
        // su MaxDeliveryCount y esta comprobacion dependeria del orden de ejecucion.
        await using var provider = this.BuildProvider();
        var message = provider.GetRequiredService<IMessage>();
        var clientProvider = provider.GetRequiredService<IServiceBusClientProvider>();
        var resolver = provider.GetRequiredService<IDomainEventResolver>();
        var names = provider.GetRequiredService<ISubscriptionNameResolver>();

        await message.SubscribeAsync<OrderCreatedDomainEvent, OrderCreatedDomainEventHandler>(CancellationToken.None);

        var topic = resolver.GetKeyDomainEvent<OrderCreatedDomainEvent>();
        var subscription = names.GetSubscriptionName(typeof(OrderCreatedDomainEventHandler));

        Assert.True(await clientProvider.AdministrationClient.TopicExistsAsync(topic));
        Assert.True(await clientProvider.AdministrationClient.SubscriptionExistsAsync(topic, subscription));

        var properties = (await clientProvider.AdministrationClient.GetSubscriptionAsync(topic, subscription)).Value;

        // Una entrega por encima del tope del consumidor: el broker es la red de seguridad, no el arbitro.
        Assert.Equal(5, properties.MaxDeliveryCount);
        Assert.True(properties.DeadLetteringOnMessageExpiration);
        Assert.True(subscription.Length <= SubscriptionNameResolver.MaxLength);

        await message.UnsubscribeAsync<OrderCreatedDomainEvent, OrderCreatedDomainEventHandler>(CancellationToken.None);
    }

    [Fact]
    public async Task SubscribeAsync_InfrastructureError_RetriesWithGrowingDelay()
    {
        // Es la prueba del pendiente 101. Con el comportamiento anterior las entregas se consumian en
        // milisegundos; aqui se exige que cada reintento tarde mas que el anterior.
        await using var provider = this.BuildProvider(maxRetry: 6, retryIntervalMs: 1000, maxRetryIntervalMs: 4000);
        var message = provider.GetRequiredService<IMessage>();
        var memory = provider.GetRequiredService<IMemoryHandler>();

        await message.SubscribeAsync<UserCreatedDomainEvent, UserCreatedDomainEventHandler>(CancellationToken.None);

        var @event = new UserCreatedDomainEvent(Guid.NewGuid(), "Throw Infrastructure Exception");

        await message.PublishAsync(@event, CancellationToken.None);

        await WaitUntilAsync(() => memory.Attempts(@event.AggregateId) >= 4, TimeSpan.FromSeconds(60));

        var times = memory.DeliveryTimes(@event.AggregateId);

        Assert.True(times.Count >= 4, $"Se esperaban al menos 4 entregas y hubo {times.Count}.");

        var firstGap = times[1] - times[0];
        var secondGap = times[2] - times[1];
        var thirdGap = times[3] - times[2];

        Assert.True(firstGap >= TimeSpan.FromMilliseconds(900), $"El primer reintento no espero: {firstGap}.");
        Assert.True(secondGap > firstGap, $"El segundo reintento ({secondGap}) no supero al primero ({firstGap}).");
        Assert.True(thirdGap > secondGap, $"El tercer reintento ({thirdGap}) no supero al segundo ({secondGap}).");

        await message.UnsubscribeAsync<UserCreatedDomainEvent, UserCreatedDomainEventHandler>(CancellationToken.None);
    }

    [Fact]
    public async Task SubscribeAsync_BusinessError_DeadLettersWithoutRetrying()
    {
        await using var provider = this.BuildProvider();
        var message = provider.GetRequiredService<IMessage>();
        var memory = provider.GetRequiredService<IMemoryHandler>();
        var clientProvider = provider.GetRequiredService<IServiceBusClientProvider>();
        var resolver = provider.GetRequiredService<IDomainEventResolver>();
        var names = provider.GetRequiredService<ISubscriptionNameResolver>();

        await message.SubscribeAsync<UserCreatedDomainEvent, UserCreatedDomainEventHandler>(CancellationToken.None);

        var @event = new UserCreatedDomainEvent(Guid.NewGuid(), "Throw Business Exception");

        await message.PublishAsync(@event, CancellationToken.None);

        await WaitUntilAsync(() => memory.Attempts(@event.AggregateId) >= 1, TimeSpan.FromSeconds(30));

        var topic = resolver.GetKeyDomainEvent<UserCreatedDomainEvent>();
        var subscription = names.GetSubscriptionName(typeof(UserCreatedDomainEventHandler));

        await message.UnsubscribeAsync<UserCreatedDomainEvent, UserCreatedDomainEventHandler>(CancellationToken.None);

        var options = provider.GetRequiredService<IOptions<ServiceBusOptions>>().Value;
        await using var client = new ServiceBusClient(options.ConnectionString);
        await using var receiver = client.CreateReceiver(topic, subscription, new ServiceBusReceiverOptions { SubQueue = SubQueue.DeadLetter });

        var dead = await receiver.ReceiveMessageAsync(TimeSpan.FromSeconds(20));

        Assert.NotNull(dead);
        Assert.Equal(ServiceBusPubSubService.ReasonBusinessError, dead.DeadLetterReason);

        // Un error de negocio no mejora al repetirlo: va a la DLQ en la primera entrega.
        Assert.Equal(1, memory.Attempts(@event.AggregateId));

        await receiver.CompleteMessageAsync(dead);
    }
}
