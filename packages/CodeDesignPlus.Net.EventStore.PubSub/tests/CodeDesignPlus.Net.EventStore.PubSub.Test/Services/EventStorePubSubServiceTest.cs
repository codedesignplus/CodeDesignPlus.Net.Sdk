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
using CodeDesignPlus.Net.PubSub.Abstractions.Options;
using CodeDesignPlus.Net.Core.Abstractions;

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
        var serviceProvider = Mock.Of<IServiceProvider>();
        var logger = Mock.Of<ILogger<EventStorePubSubService>>();
        var eventStorePubSubOptions = Options.Create(new EventStorePubSubOptions());
        var pubSubOptions = Options.Create(new PubSubOptions());
        var domainEventResolverService = Mock.Of<IDomainEventResolverService>();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new EventStorePubSubService(eventStoreFactory, serviceProvider, logger, eventStorePubSubOptions, pubSubOptions, domainEventResolverService));
    }

    [Fact]
    public void Constructor_NullServiceProvider_ThrowsArgumentNullException()
    {
        // Arrange
        IServiceProvider serviceProvider = null!;
        var eventStoreFactory = Mock.Of<IEventStoreFactory>();
        var logger = Mock.Of<ILogger<EventStorePubSubService>>();
        var eventStorePubSubOptions = Options.Create(new EventStorePubSubOptions());
        var pubSubOptions = Options.Create(new PubSubOptions());
        var domainEventResolverService = Mock.Of<IDomainEventResolverService>();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new EventStorePubSubService(eventStoreFactory, serviceProvider, logger, eventStorePubSubOptions, pubSubOptions, domainEventResolverService));
    }

    [Fact]
    public void Constructor_NullOptions_ThrowsArgumentNullException()
    {
        // Arrange
        IServiceProvider serviceProvider = null!;
        var eventStoreFactory = Mock.Of<IEventStoreFactory>();
        var logger = Mock.Of<ILogger<EventStorePubSubService>>();
        var pubSubOptions = Options.Create(new PubSubOptions());
        var domainEventResolverService = Mock.Of<IDomainEventResolverService>();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new EventStorePubSubService(eventStoreFactory, serviceProvider, logger, null!, pubSubOptions, domainEventResolverService));
    }

    [Fact]
    public void Constructor_NullLogger_ThrowsArgumentNullException()
    {
        // Arrange
        var serviceProvider = Mock.Of<IServiceProvider>();
        var eventStoreFactory = Mock.Of<IEventStoreFactory>();
        var eventStorePubSubOptions = Options.Create(new EventStorePubSubOptions());
        var pubSubOptions = Options.Create(new PubSubOptions());
        var domainEventResolverService = Mock.Of<IDomainEventResolverService>();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new EventStorePubSubService(eventStoreFactory, serviceProvider, null!, eventStorePubSubOptions, pubSubOptions, domainEventResolverService));
    }

    [Fact]
    public void Constructor_NullPubSubOptions_ThrowsArgumentNullException()
    {
        // Arrange
        var serviceProvider = Mock.Of<IServiceProvider>();
        var eventStoreFactory = Mock.Of<IEventStoreFactory>();
        var logger = Mock.Of<ILogger<EventStorePubSubService>>();
        var eventStorePubSubOptions = Options.Create(new EventStorePubSubOptions());
        var domainEventResolverService = Mock.Of<IDomainEventResolverService>();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new EventStorePubSubService(eventStoreFactory, serviceProvider, logger, eventStorePubSubOptions, null!, domainEventResolverService));
    }

    [Fact]
    public void Constructor_NullDomainEventResolverService_ThrowsArgumentNullException()
    {
        // Arrange
        var serviceProvider = Mock.Of<IServiceProvider>();
        var eventStoreFactory = Mock.Of<IEventStoreFactory>();
        var logger = Mock.Of<ILogger<EventStorePubSubService>>();
        var eventStorePubSubOptions = Options.Create(new EventStorePubSubOptions());
        var pubSubOptions = Options.Create(new PubSubOptions());

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new EventStorePubSubService(eventStoreFactory, serviceProvider, logger, eventStorePubSubOptions, pubSubOptions, null!));
    }

    [Fact]
    public async Task PublishAndSubscribe()
    {
        var testServer = this.BuildTestServer(true, this.testOutput);

        var pubSub = testServer.Host.Services.GetRequiredService<IMessage>();

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

            userEvent = Startup.MemoryService.OrderCreatedEvent.FirstOrDefault(x => x.EventId == @event.EventId);
        }
        while (userEvent == null);

        Assert.NotEmpty(Startup.MemoryService.OrderCreatedEvent);
        Assert.NotNull(userEvent);
        Console.WriteLine("{0} {1}", @event.EventId, userEvent.EventId);
        Assert.Equal(@event.AggregateId, userEvent.AggregateId);
        Assert.Equal(@event.Client.Name, userEvent.Client.Name);
        Assert.Equal(@event.Client.Id, userEvent.Client.Id);
        Assert.Equal(@event.EventType, userEvent.EventType);
        Assert.Equal(@event.DateCreated, userEvent.DateCreated);
        Assert.Equal(@event.OccurredAt, userEvent.OccurredAt);
        Assert.Equal(@event.OrderStatus, userEvent.OrderStatus);
        Assert.Equal(@event.EventId, userEvent.EventId);
    }

    [Fact]
    public async Task UnsubscribeAsync_UnsubscribesSuccessfully()
    {
        // Arrange
        var eventStoreFactoryMock = new Mock<IEventStoreFactory>();
        var serviceProviderMock = new Mock<IServiceProvider>();
        var loggerMock = new Mock<ILogger<EventStorePubSubService>>();
        var eventStorePubSubOptions = Options.Create(new EventStorePubSubOptions());
        var pubSubOptions = Options.Create(new PubSubOptions());
        var domainEventResolverService = Mock.Of<IDomainEventResolverService>();

        var service = new EventStorePubSubService(eventStoreFactoryMock.Object, serviceProviderMock.Object, loggerMock.Object, eventStorePubSubOptions, pubSubOptions, domainEventResolverService);

        // Act
        var task = service.UnsubscribeAsync<DomainEvent, IEventHandler<DomainEvent>>(CancellationToken.None);

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
            Core = new
            {
                AppName = "Test",
            },
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
