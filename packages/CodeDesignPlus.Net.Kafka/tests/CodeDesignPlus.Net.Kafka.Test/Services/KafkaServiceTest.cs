using System.Reflection;
using CodeDesignPlus.Net.PubSub.Abstractions;
using CodeDesignPlus.Net.PubSub.Extensions;
using CodeDesignPlus.Net.Kafka.Extensions;
using CodeDesignPlus.Net.Kafka.Test.Helpers.Events;
using CodeDesignPlus.Net.Kafka.Test.Helpers.Memory;
using CodeDesignPlus.Net.xUnit;
using CodeDesignPlus.Net.xUnit.Helpers.Loggers;
using Confluent.Kafka;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;
using Moq;
using Xunit.Abstractions;
using CodeDesignPlus.Net.xUnit.Helpers;
using O = Microsoft.Extensions.Options;
using CodeDesignPlus.Net.PubSub.Abstractions.Options;
using CodeDesignPlus.Net.Core.Abstractions;
using CodeDesignPlus.Net.Core.Services;

namespace CodeDesignPlus.Net.Kafka.Test.Services;

public class KafkaServiceTest : IClassFixture<KafkaContainer>
{
    private readonly KafkaContainer kafkaContainer;
    private readonly ITestOutputHelper testOutput;

    private readonly Mock<ILogger<KafkaEventBus>> _mockLogger = new();
    private readonly Mock<IOptions<KafkaOptions>> _mockKafkaOptions = new();
    private readonly IServiceProvider serviceProvider;
    private readonly Mock<IOptions<PubSubOptions>> _mockPubSubOptions = new();
    private readonly Mock<IDomainEventResolverService> _mockDomainEventResolverService = new();
    private readonly Mock<IProducer<string, IDomainEvent>> _mockProducer = new();


    public KafkaServiceTest(ITestOutputHelper output, KafkaContainer kafkaContainer)
    {
        this.kafkaContainer = kafkaContainer;
        this.testOutput = output;

        var serviceCollection = new ServiceCollection()
            .AddSingleton<IProducer<string, IDomainEvent>>(x => _mockProducer.Object);

        this.serviceProvider = serviceCollection.BuildServiceProvider();
    }

    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenLoggerIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new KafkaEventBus(null, _mockDomainEventResolverService.Object, _mockKafkaOptions.Object, serviceProvider, _mockPubSubOptions.Object));
    }

    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenOptionsIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new KafkaEventBus(_mockLogger.Object, _mockDomainEventResolverService.Object, null, serviceProvider, _mockPubSubOptions.Object));
    }

    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenServiceProviderIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new KafkaEventBus(_mockLogger.Object, _mockDomainEventResolverService.Object, _mockKafkaOptions.Object, null, _mockPubSubOptions.Object));
    }

    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenPubSubOptionsIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new KafkaEventBus(_mockLogger.Object, _mockDomainEventResolverService.Object, _mockKafkaOptions.Object, serviceProvider, null));
    }

    [Fact]
    public void Constructor_Succeeds_WhenAllArgumentsAreValid()
    {
        var instance = new KafkaEventBus(_mockLogger.Object, _mockDomainEventResolverService.Object, _mockKafkaOptions.Object, serviceProvider, _mockPubSubOptions.Object);
        Assert.NotNull(instance);
    }

    [Theory]
    [InlineData(true, "ConsumerQueue")]
    [InlineData(false, "ConsumerWithouQueue")]
    public async Task PublishAsync_Event_InvokeHandler(bool enableQueue, string gorup)
    {
        var @event = new UserCreatedEvent(Guid.NewGuid())
        {
            Names = "Code",
            Lastnames = "Design Plus",
            Username = "coded",
            Birthdate = new DateTime(2019, 11, 21)
        };

        var testServer = BuildTestServer(enableQueue, this.testOutput, gorup);

        var pubSub = testServer.Host.Services.GetRequiredService<IMessage>();

        await pubSub.PublishAsync(@event, CancellationToken.None);

        UserCreatedEvent? userEvent;

        do
        {
            await Task.Delay(TimeSpan.FromSeconds(5));

            userEvent = Startup.MemoryService.UserEventTrace.FirstOrDefault(x => x.EventId == @event.EventId);
        }
        while (userEvent == null);

        Assert.NotEmpty(Startup.MemoryService.UserEventTrace);
        Assert.NotNull(userEvent);
        Console.WriteLine("{0} {1}", @event.EventId, userEvent.EventId);
        Assert.Equal(@event.Username, userEvent.Username);
        Assert.Equal(@event.Names, userEvent.Names);
        Assert.Equal(@event.Birthdate, userEvent.Birthdate);
        Assert.Equal(@event.Lastnames, userEvent.Lastnames);
        Assert.Equal(@event.OccurredAt, userEvent.OccurredAt);
        Assert.Equal(@event.EventId, userEvent.EventId);
    }

    // [Fact]
    // public async Task SubscribeAsync_ExitsLoop_When_CancellationTokenIsCancelled()
    // {
    //     // Preparar
    //     var mockLogger = new Mock<ILogger<KafkaEventBus>>();
    //     var mockOptions = O.Options.Create(new KafkaOptions()
    //     {
    //         Enable = true,
    //         BootstrapServers = "localhost:29092",
    //         Acks = "all",
    //         BatchSize = 4096,
    //         LingerMs = 5,
    //         CompressionType = "snappy",
    //         NameMicroservice = "Test"
    //     });
    //     var mockPubSubOptions = O.Options.Create(new PubSubOptions());
    //     var mockProducer = new Mock<IProducer<string, IDomainEvent>>();
    //     var domainEventResolverService = new DomainEventResolverService();

    //     var serviceCollection = new ServiceCollection()
    //         .AddSingleton(x => new UserCreatedEventHandler(Mock.Of<ILogger<UserCreatedEventHandler>>(), new MemoryService()));

    //     var serviceProvider = serviceCollection.BuildServiceProvider();

    //     var kafkaEventBus = new KafkaEventBus(mockLogger.Object, domainEventResolverService, mockOptions, serviceProvider, mockPubSubOptions);

    //     var cancellationTokenSource = new CancellationTokenSource();
    //     cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(10));  // Cancela después de 100 ms

    //     // Actuar
    //     await kafkaEventBus.SubscribeAsync<UserCreatedEvent, UserCreatedEventHandler>(cancellationTokenSource.Token);

    //     // Afirmar
    //     mockLogger.VerifyLogging("Kafka event listening has stopped for event type: UserCreatedEvent due to cancellation request.", LogLevel.Information);
    // }

    [Fact]
    public async Task Unsubscribe_InvokesConsumerUnsubscribeAndLogsInformation()
    {
        // Arrange
        var mockConsumer = new Mock<IConsumer<string, UserCreatedEvent>>();
        var serviceCollection = new ServiceCollection().AddSingleton(x => mockConsumer.Object);
        var serviceProvider = serviceCollection.BuildServiceProvider();

        var kafkaEventBus = new KafkaEventBus(_mockLogger.Object, _mockDomainEventResolverService.Object, _mockKafkaOptions.Object, serviceProvider, _mockPubSubOptions.Object);

        // Act
        await kafkaEventBus.UnsubscribeAsync<UserCreatedEvent, UserCreatedEventHandler>(CancellationToken.None);

        // Assert
        mockConsumer.Verify(x => x.Unsubscribe(), Times.Once);
        _mockLogger.VerifyLogging($"Unsubscribing from event {typeof(UserCreatedEvent).Name} for handler {typeof(UserCreatedEventHandler).Name}", LogLevel.Information);
    }


    private static TestServer BuildTestServer(bool enableQueue, ITestOutputHelper output, string group)
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
                        AddJsonStream(config, enableQueue, group);
                    }).UseStartup<Startup>();

        var testServer = new TestServer(webHostBuilder);
        return testServer;
    }


    private static void AddJsonStream(IConfigurationBuilder config, bool enableQueue, string group)
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
            Kafka = new
            {
                Enable = true,
                BootstrapServers = "localhost:29092",
                Acks = "all",
                BatchSize = 4096,
                LingerMs = 5,
                CompressionType = "snappy",
                NameMicroservice = group,
                ListenerEvents = true
            }
        });

        var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(json));

        config.AddJsonStream(memoryStream);
    }
}
