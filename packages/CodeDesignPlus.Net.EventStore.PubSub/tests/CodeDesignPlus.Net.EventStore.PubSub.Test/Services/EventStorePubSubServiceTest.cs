using CodeDesignPlus.Net.PubSub.Abstractions;
using CodeDesignPlus.Net.EventStore.PubSub.Test.Helpers.Domain;
using CodeDesignPlus.Net.EventStore.PubSub.Test.Helpers.Events;
using CodeDesignPlus.Net.xUnit.Helpers.EventStoreContainer;
using CodeDesignPlus.Net.xUnit.Helpers.Loggers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Xunit.Abstractions;
using CodeDesignPlus.Net.EventStore.Abstractions;
using Moq;
using CodeDesignPlus.Net.EventStore.Abstractions.Options;
using CodeDesignPlus.Net.PubSub.Abstractions.Options;
using System.Reflection;
using EventStore.ClientAPI;
using CodeDesignPlus.Net.xUnit.Helpers;

namespace CodeDesignPlus.Net.EventStore.Test.Services;

public class EventStorePubSubServiceTest : IClassFixture<EventStoreContainer>
{
    private readonly EventStoreContainer container;
    private readonly ITestOutputHelper testOutput;

    public EventStorePubSubServiceTest(ITestOutputHelper output, EventStoreContainer container)
    {
        this.container = container;
        this.testOutput = output;
    }

    [Fact]
    public void Constructor_NullEventStoreFactory_ThrowsArgumentNullException()
    {
        // Arrange
        IEventStoreFactory eventStoreFactory = null!;
        var subscriptionManager = Mock.Of<ISubscriptionManager>();
        var serviceProvider = Mock.Of<IServiceProvider>();
        var logger = Mock.Of<ILogger<EventStorePubSubService>>();
        var pubSubOptions = Options.Create(new EventStorePubSubOptions());

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new EventStorePubSubService(eventStoreFactory, subscriptionManager, serviceProvider, logger, pubSubOptions));
    }

    [Fact]
    public void Constructor_NullSubscriptionManager_ThrowsArgumentNullException()
    {
        // Arrange
        ISubscriptionManager subscriptionManager = null;
        var eventStoreFactory = Mock.Of<IEventStoreFactory>();
        var serviceProvider = Mock.Of<IServiceProvider>();
        var logger = Mock.Of<ILogger<EventStorePubSubService>>();
        var pubSubOptions = Options.Create(new EventStorePubSubOptions());

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new EventStorePubSubService(eventStoreFactory, subscriptionManager, serviceProvider, logger, pubSubOptions));
    }

    [Fact]
    public void Constructor_NullServiceProvider_ThrowsArgumentNullException()
    {
        // Arrange
        IServiceProvider serviceProvider = null;
        var eventStoreFactory = Mock.Of<IEventStoreFactory>();
        var subscriptionManager = Mock.Of<ISubscriptionManager>();
        var logger = Mock.Of<ILogger<EventStorePubSubService>>();
        var pubSubOptions = Options.Create(new EventStorePubSubOptions());

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new EventStorePubSubService(eventStoreFactory, subscriptionManager, serviceProvider, logger, pubSubOptions));
    }

    [Fact]
    public async Task PublishAndSubscribe()
    {
        var testServer = this.BuildTestServer(true, this.testOutput);

        var pubSub = testServer.Host.Services.GetRequiredService<IPubSub>();

        var @event = new OrderCreatedEvent(Guid.NewGuid(), Guid.NewGuid(), OrderStatus.Pending, new Client()
        {
            Id = Guid.NewGuid(),
            Name = "Test",
        }, DateTime.UtcNow);

        await pubSub.PublishAsync(@event, CancellationToken.None);


        OrderCreatedEvent? userEvent;
        do
        {
            await Task.Delay(TimeSpan.FromSeconds(15));

            userEvent = Startup.MemoryService.OrderCreatedEvent.FirstOrDefault(x => x.IdEvent == @event.IdEvent);
        }
        while (userEvent == null);

        Assert.NotEmpty(Startup.MemoryService.OrderCreatedEvent);
        Assert.NotNull(userEvent);
        Console.WriteLine("{0} {1}", @event.IdEvent, userEvent.IdEvent);
        Assert.Equal(@event.AggregateId, userEvent.AggregateId);
        Assert.Equal(@event.Client.Name, userEvent.Client.Name);
        Assert.Equal(@event.Client.Id, userEvent.Client.Id);
        Assert.Equal(@event.EventType, userEvent.EventType);
        Assert.Equal(@event.DateCreated, userEvent.DateCreated);
        Assert.Equal(@event.EventDate, userEvent.EventDate);
        Assert.Equal(@event.OrderStatus, userEvent.OrderStatus);
        Assert.Equal(@event.IdEvent, userEvent.IdEvent);
    }

    [Fact]
    public async Task UnsubscribeAsync_UnsubscribesSuccessfully()
    {
        // Arrange
        var eventStoreFactoryMock = new Mock<IEventStoreFactory>();
        var subscriptionManagerMock = new Mock<ISubscriptionManager>();
        var serviceProviderMock = new Mock<IServiceProvider>();
        var loggerMock = new Mock<ILogger<EventStorePubSubService>>();
        var pubSubOptions = Options.Create(new EventStorePubSubOptions());

        var service = new EventStorePubSubService(eventStoreFactoryMock.Object, subscriptionManagerMock.Object, serviceProviderMock.Object, loggerMock.Object, pubSubOptions);

        // Act
        var task = service.UnsubscribeAsync<EventBase, IEventHandler<EventBase>>();

        await task;

        // Assert
        Assert.True(task.IsCompleted);
    }

    private TestServer BuildTestServer(bool enableQueue, ITestOutputHelper output)
    {
        var webHostBuilder = new WebHostBuilder()
                    .ConfigureLogging(logging =>
                    {
                        // check if scopes are used in normal operation
                        var useScopes = logging.UsesScopes();
                        // remove other logging providers, such as remote loggers or unnecessary event logs
                        logging.ClearProviders();
                        logging.Services.AddSingleton<ILoggerProvider>(r => new XunitLoggerProvider(output, useScopes));
                    })
                    .ConfigureAppConfiguration((hostingContext, config) =>
                    {
                        this.AddJsonStream(config, enableQueue);
                    }).UseStartup<Startup>();

        var testServer = new TestServer(webHostBuilder);
        return testServer;
    }

    private void AddJsonStream(IConfigurationBuilder config, bool enableQueue)
    {
        var json = JsonSerializer.Serialize(new
        {
            PubSub = new
            {
                EnableQueue = enableQueue,
            },
            EventSourcing = new
            {
                MainName = "aggregate",
                SnapshotSuffix = "snapshot"
            },
            EventStore = new
            {
                Servers = new Dictionary<string, Server>()
                {
                    {
                        EventStoreFactoryConst.Core, new Server()
                        {
                            ConnectionString = new Uri($"tcp://admin:112345678@localhost:{this.container.Port}"),
                            User = "admin",
                            Password = "112345678"
                        }
                    }
                }
            },
            EventStorePubSub = new
            {
                Enable = true,
                Group = "testGroup",
            }
        });

        var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(json));

        config.AddJsonStream(memoryStream);
    }
}
