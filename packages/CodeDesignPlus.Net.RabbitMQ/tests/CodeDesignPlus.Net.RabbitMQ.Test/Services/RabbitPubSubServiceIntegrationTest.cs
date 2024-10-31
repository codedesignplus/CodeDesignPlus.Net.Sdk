using CodeDesignPlus.Net.Core.Abstractions;
using CodeDesignPlus.Net.Core.Abstractions.Options;
using CodeDesignPlus.Net.Core.Extensions;
using CodeDesignPlus.Net.RabbitMQ.Extensions;
using CodeDesignPlus.Net.RabbitMQ.Test.Helpers.Events;
using CodeDesignPlus.Net.xUnit.Helpers;
using CodeDesignPlus.Net.xUnit.Helpers.RabbitMQContainer;
using Moq;
using O = Microsoft.Extensions.Options;

namespace CodeDesignPlus.Net.RabbitMQ.Test.Services;

[Collection(RabbitMQCollectionFixture.Collection)]
public class RabbitPubSubServiceIntegrationTest 
{
    private readonly Mock<ILogger<RabbitPubSubService>> loggerMock;
    private readonly IMemoryHandler memoryHandler;
    private readonly RabbitPubSubService rabbitPubSubService;

    public RabbitPubSubServiceIntegrationTest(RabbitMQCollectionFixture fixture)
    {
        loggerMock = new Mock<ILogger<RabbitPubSubService>>();

        var configuration = Helpers.ConfigurationUtil.GetConfiguration(new
        {
            Core = Helpers.ConfigurationUtil.CoreOptions,
            RabbitMQ = new RabbitMQOptions
            {
                Host = "localhost",
                Port = fixture.Container.Port,
                UserName = "usr_codedesignplus",
                Password = "Temporal1",
                Enable = true,
                MaxRetry = 10,
                RetryInterval = 1000
            }
        });

        var serviceCollection = new ServiceCollection();
        serviceCollection.AddLogging();
        serviceCollection.AddSingleton(configuration);
        serviceCollection.AddCore(configuration);
        serviceCollection.AddRabbitMQ<RabbitPubSubServiceIntegrationTest>(configuration);
        serviceCollection.AddSingleton<UserCreatedDomainEventHandler>();
        serviceCollection.AddSingleton<ProductCreatedDomainEventHandler>();
        serviceCollection.AddSingleton<IMemoryHandler, MemoryHandler>();

        var serviceProvider = serviceCollection.BuildServiceProvider();
        var domainEventResolverService = serviceProvider.GetRequiredService<IDomainEventResolverService>();
        var channelProvider = serviceProvider.GetRequiredService<IChannelProvider>();
        var coreOptions = serviceProvider.GetRequiredService<O.IOptions<CoreOptions>>();
        var rabbitMQOptions = serviceProvider.GetRequiredService<O.IOptions<RabbitMQOptions>>();
        this.memoryHandler = serviceProvider.GetRequiredService<IMemoryHandler>();


        rabbitPubSubService = new RabbitPubSubService(loggerMock.Object, serviceProvider, domainEventResolverService, channelProvider, coreOptions, rabbitMQOptions);
    }

    [Fact]
    public async Task PublishAsync_InvokeHandler_EventEquals()
    {
        // Arrange
        var idAggregate = Guid.NewGuid();
        var domainEvent = new ProductCreatedDomainEvent(idAggregate, "CodeDesignPlus Name");

        await rabbitPubSubService.SubscribeAsync<ProductCreatedDomainEvent, ProductCreatedDomainEventHandler>(CancellationToken.None);

        await Task.Delay(2000);

        // Act
        await rabbitPubSubService.PublishAsync([domainEvent], CancellationToken.None);

        await Task.Delay(2000);

        // Assert
        Assert.Contains(idAggregate, memoryHandler.Memory.Keys);

        var @event = memoryHandler.Memory[idAggregate] as ProductCreatedDomainEvent;

        Assert.NotNull(@event);
        Assert.Equal(domainEvent.AggregateId, @event.AggregateId);
        Assert.Equal(domainEvent.Name, @event.Name);
        Assert.Equal(domainEvent.EventId, @event.EventId);
        Assert.Equal(domainEvent.OccurredAt, @event.OccurredAt);
    }

    [Fact]
    public async Task SubscribeAsync_UnhandlerException_ErrorProcessingEvent()
    {
        // Arrange
        var idAggregate = Guid.NewGuid();
        var domainEvent = new UserCreatedDomainEvent(idAggregate, "Throw Exception");

        await rabbitPubSubService.SubscribeAsync<UserCreatedDomainEvent, UserCreatedDomainEventHandler>(CancellationToken.None);

        await Task.Delay(2000);

        // Act
        await rabbitPubSubService.PublishAsync([domainEvent], CancellationToken.None);

        await Task.Delay(2000);

        // Assert
        Assert.Contains(idAggregate, memoryHandler.Memory.Keys);
        loggerMock.VerifyLogging($"Error processing event: {typeof(UserCreatedDomainEvent).Name}.", LogLevel.Error, Times.Once());
    }

    [Fact]
    public async Task UnsubscribeAsync_CheckUnsubcribe_EventNotReceived()
    {
        // Arrange
        var idAggregate = Guid.NewGuid();
        var domainEvent = new UserCreatedDomainEvent(idAggregate, "Unsubscribe");

        await rabbitPubSubService.SubscribeAsync<UserCreatedDomainEvent, UserCreatedDomainEventHandler>(CancellationToken.None);

        // Act
        await rabbitPubSubService.UnsubscribeAsync<UserCreatedDomainEvent, UserCreatedDomainEventHandler>(CancellationToken.None);

        await Task.Delay(2000);

        await rabbitPubSubService.PublishAsync([domainEvent], CancellationToken.None);

        await Task.Delay(2000);

        // Assert
        Assert.DoesNotContain(idAggregate, memoryHandler.Memory.Keys);
        loggerMock.VerifyLogging($"Unsubscribed from event: {typeof(UserCreatedDomainEvent).Name}.", LogLevel.Information, Times.Once());
    }

}
