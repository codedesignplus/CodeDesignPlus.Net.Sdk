using CodeDesignPlus.Net.Core.Abstractions;
using CodeDesignPlus.Net.Core.Abstractions.Options;
using CodeDesignPlus.Net.EventStore.Abstractions;
using CodeDesignPlus.Net.EventStore.Abstractions.Options;
using CodeDesignPlus.Net.EventStore.PubSub.Test.Helpers.Domain;
using CodeDesignPlus.Net.EventStore.PubSub.Test.Helpers.Events;
using CodeDesignPlus.Net.PubSub.Abstractions;
using CodeDesignPlus.Net.xUnit.Containers.EventStoreContainer;
using CodeDesignPlus.Net.xUnit.Output.Loggers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Moq;
using Xunit.Abstractions;
using MO = Microsoft.Extensions.Options;

namespace CodeDesignPlus.Net.EventStore.PubSub.Test.Services;

[Collection(EventStoreCollectionFixture.Collection)]
public class EventStorePubSubServiceTest(ITestOutputHelper output, EventStoreCollectionFixture eventStoreCollectionFixture)
{
    private readonly EventStoreContainer container = eventStoreCollectionFixture.Container;
    private readonly ITestOutputHelper testOutput = output;

    [Fact]
    public void Constructor_NullEventStoreFactory_ThrowsArgumentNullException()
    {
        // Arrange
        IEventStoreFactory eventStoreFactory = null!;
        var serviceProvider = Mock.Of<IServiceProvider>();
        var logger = Mock.Of<ILogger<EventStorePubSubService>>();
        var eventStorePubSubOptions = MO.Options.Create(new EventStorePubSubOptions());
        var domainEventResolverService = Mock.Of<IDomainEventResolver>();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new EventStorePubSubService(eventStoreFactory, serviceProvider, logger, eventStorePubSubOptions, domainEventResolverService));
    }

    [Fact]
    public void Constructor_NullServiceProvider_ThrowsArgumentNullException()
    {
        // Arrange
        IServiceProvider serviceProvider = null!;
        var eventStoreFactory = Mock.Of<IEventStoreFactory>();
        var logger = Mock.Of<ILogger<EventStorePubSubService>>();
        var eventStorePubSubOptions = MO.Options.Create(new EventStorePubSubOptions());
        var domainEventResolverService = Mock.Of<IDomainEventResolver>();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new EventStorePubSubService(eventStoreFactory, serviceProvider, logger, eventStorePubSubOptions, domainEventResolverService));
    }

    [Fact]
    public void Constructor_NullOptions_ThrowsArgumentNullException()
    {
        // Arrange
        IServiceProvider serviceProvider = null!;
        var eventStoreFactory = Mock.Of<IEventStoreFactory>();
        var logger = Mock.Of<ILogger<EventStorePubSubService>>();
        var domainEventResolverService = Mock.Of<IDomainEventResolver>();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new EventStorePubSubService(eventStoreFactory, serviceProvider, logger, null!, domainEventResolverService));
    }

    [Fact]
    public void Constructor_NullLogger_ThrowsArgumentNullException()
    {
        // Arrange
        var serviceProvider = Mock.Of<IServiceProvider>();
        var eventStoreFactory = Mock.Of<IEventStoreFactory>();
        var eventStorePubSubOptions = MO.Options.Create(new EventStorePubSubOptions());
        var domainEventResolverService = Mock.Of<IDomainEventResolver>();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new EventStorePubSubService(eventStoreFactory, serviceProvider, null!, eventStorePubSubOptions, domainEventResolverService));
    }

    [Fact]
    public void Constructor_NullDomainEventResolverService_ThrowsArgumentNullException()
    {
        // Arrange
        var serviceProvider = Mock.Of<IServiceProvider>();
        var eventStoreFactory = Mock.Of<IEventStoreFactory>();
        var logger = Mock.Of<ILogger<EventStorePubSubService>>();
        var eventStorePubSubOptions = MO.Options.Create(new EventStorePubSubOptions());

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new EventStorePubSubService(eventStoreFactory, serviceProvider, logger, eventStorePubSubOptions, null!));
    }

    [Fact]
    public async Task PublishAndSubscribe()
    {
        var testServer = BuildTestServer(true, testOutput);

        var pubSub = testServer.Host.Services.GetRequiredService<IMessage>();

        var eventOrderCreated = new OrderCreatedEvent(Guid.NewGuid(), Guid.NewGuid(), OrderStatus.Pending, new Client()
        {
            Id = Guid.NewGuid(),
            Name = "Test",
        }, DateTime.UtcNow);

        var eventOrderComplete = new OrderCompletedEvent(Guid.NewGuid(), DateTime.UtcNow);

        var events = new List<DomainEvent>()
        {
            eventOrderCreated,
            eventOrderComplete
        };

        await Task.Delay(TimeSpan.FromSeconds(2));

        _ = Task.Run(() => pubSub.PublishAsync(events, CancellationToken.None));


        OrderCreatedEvent? userEvent;

        do
        {
            await Task.Delay(TimeSpan.FromSeconds(2));

            userEvent = Startup.MemoryService.OrderCreatedEvent.FirstOrDefault(x => x.EventId == eventOrderCreated.EventId);
        }
        while (userEvent == null);

        Assert.NotEmpty(Startup.MemoryService.OrderCreatedEvent);
        Assert.NotNull(userEvent);
        Console.WriteLine("{0} {1}", eventOrderCreated.EventId, userEvent.EventId);
        Assert.Equal(eventOrderCreated.AggregateId, userEvent.AggregateId);
        Assert.Equal(eventOrderCreated.Client.Name, userEvent.Client.Name);
        Assert.Equal(eventOrderCreated.Client.Id, userEvent.Client.Id);
        Assert.Equal(eventOrderCreated.DateCreated, userEvent.DateCreated);
        Assert.Equal(eventOrderCreated.OccurredAt, userEvent.OccurredAt);
        Assert.Equal(eventOrderCreated.OrderStatus, userEvent.OrderStatus);
        Assert.Equal(eventOrderCreated.EventId, userEvent.EventId);
    }


    [Fact]
    public async Task PublishAndSubscribe_StreamExist_CatchException()
    {
        // This server create the stream with the same name as the event type
        _ = BuildTestServer(true, testOutput);

        // This generate catch exception because the stream already exist
        var testServer = BuildTestServer(true, testOutput);

        var pubSub = testServer.Host.Services.GetRequiredService<IMessage>();

        var eventOrderCreated = new OrderCreatedEvent(Guid.NewGuid(), Guid.NewGuid(), OrderStatus.Pending, new Client()
        {
            Id = Guid.NewGuid(),
            Name = "Test",
        }, DateTime.UtcNow);

        await Task.Delay(TimeSpan.FromSeconds(2));

        _ = Task.Run(() => pubSub.PublishAsync(eventOrderCreated, CancellationToken.None));


        OrderCreatedEvent? userEvent;

        do
        {
            await Task.Delay(TimeSpan.FromSeconds(2));

            userEvent = Startup.MemoryService.OrderCreatedEvent.FirstOrDefault(x => x.EventId == eventOrderCreated.EventId);
        }
        while (userEvent == null);

        Assert.NotEmpty(Startup.MemoryService.OrderCreatedEvent);
        Assert.NotNull(userEvent);
        Console.WriteLine("{0} {1}", eventOrderCreated.EventId, userEvent.EventId);
        Assert.Equal(eventOrderCreated.AggregateId, userEvent.AggregateId);
        Assert.Equal(eventOrderCreated.Client.Name, userEvent.Client.Name);
        Assert.Equal(eventOrderCreated.Client.Id, userEvent.Client.Id);
        Assert.Equal(eventOrderCreated.DateCreated, userEvent.DateCreated);
        Assert.Equal(eventOrderCreated.OccurredAt, userEvent.OccurredAt);
        Assert.Equal(eventOrderCreated.OrderStatus, userEvent.OrderStatus);
        Assert.Equal(eventOrderCreated.EventId, userEvent.EventId);
    }

    [Fact]
    public async Task UnsubscribeAsync_UnsubscribesSuccessfully()
    {
        // Arrange
        var eventStoreFactoryMock = new Mock<IEventStoreFactory>();
        var serviceProviderMock = new Mock<IServiceProvider>();
        var loggerMock = new Mock<ILogger<EventStorePubSubService>>();
        var eventStorePubSubOptions = MO.Options.Create(new EventStorePubSubOptions());
        var domainEventResolverService = Mock.Of<IDomainEventResolver>();

        var service = new EventStorePubSubService(eventStoreFactoryMock.Object, serviceProviderMock.Object, loggerMock.Object, eventStorePubSubOptions, domainEventResolverService);

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
                        AddJsonStream(config, enableQueue);
                    }).UseStartup<Startup>();

        var testServer = new TestServer(webHostBuilder);
        return testServer;
    }

    private void AddJsonStream(IConfigurationBuilder config, bool enableQueue)
    {
        var json = JsonSerializer.Serialize(new
        {
            Core = new CoreOptions
            {
                Business = "CodeDesignPlus",
                AppName = "ms-test",
                Version = "v1",
                Description = "Description Test",
                Contact = new Contact
                {
                    Name = "CodeDesignPlus",
                    Email = "codedesignplus@outlook.com"
                }
            },
            EventStore = new EventStoreOptions()
            {
                Servers = new Dictionary<string, Server>()
                {
                    {
                        EventStoreFactoryConst.Core, new Server()
                        {
                            ConnectionString = new Uri($"tcp://admin:112345678@localhost:{container.Port}"),
                            User = "admin",
                            Password = "112345678"
                        }
                    }
                }
            },
            EventStorePubSub = new EventStorePubSubOptions
            {
                Enabled = true,
                Group = "testGroup",
                UseQueue = enableQueue 
            }
        });

        var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(json));

        config.AddJsonStream(memoryStream);
    }
}
