using System.Reflection;
using CodeDesignPlus.Net.Event.Bus.Abstractions;
using CodeDesignPlus.Net.Event.Bus.Extensions;
using CodeDesignPlus.Net.Event.Bus.Options;
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

namespace CodeDesignPlus.Net.Kafka.Test.Services;

public class KafkaServiceTest : IClassFixture<KafkaContainer>
{
    private readonly KafkaContainer kafkaContainer;
    private readonly ITestOutputHelper testOutput;

    private readonly Mock<ILogger<KafkaEventBus>> _mockLogger = new();
    private readonly Mock<IOptions<KafkaOptions>> _mockKafkaOptions = new();
    private readonly Mock<ISubscriptionManager> _mockSubscriptionManager = new();
    private readonly Mock<IServiceProvider> _mockServiceProvider = new();
    private readonly Mock<IOptions<EventBusOptions>> _mockEventBusOptions = new();


    public KafkaServiceTest(ITestOutputHelper output, KafkaContainer kafkaContainer)
    {
        this.kafkaContainer = kafkaContainer;
        this.testOutput = output;
    }

    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenLoggerIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new KafkaEventBus(null, _mockKafkaOptions.Object, _mockSubscriptionManager.Object, _mockServiceProvider.Object, _mockEventBusOptions.Object));
    }

    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenOptionsIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new KafkaEventBus(_mockLogger.Object, null, _mockSubscriptionManager.Object, _mockServiceProvider.Object, _mockEventBusOptions.Object));
    }

    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenSubscriptionManagerIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new KafkaEventBus(_mockLogger.Object, _mockKafkaOptions.Object, null, _mockServiceProvider.Object, _mockEventBusOptions.Object));
    }

    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenServiceProviderIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new KafkaEventBus(_mockLogger.Object, _mockKafkaOptions.Object, _mockSubscriptionManager.Object, null, _mockEventBusOptions.Object));
    }

    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenEventBusOptionsIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new KafkaEventBus(_mockLogger.Object, _mockKafkaOptions.Object, _mockSubscriptionManager.Object, _mockServiceProvider.Object, null));
    }

    [Fact]
    public void Constructor_Succeeds_WhenAllArgumentsAreValid()
    {
        var instance = new KafkaEventBus(_mockLogger.Object, _mockKafkaOptions.Object, _mockSubscriptionManager.Object, _mockServiceProvider.Object, _mockEventBusOptions.Object);
        Assert.NotNull(instance);
    }

    [Fact]
    public async Task PublishAsync_ThrowsKafkaException_WhenTopicAttributeIsMissing()
    {
        // Arrange
        var eventWithoutTopic = new DummyEventWithoutTopic();
        var kafkaEventBus = new KafkaEventBus(_mockLogger.Object, _mockKafkaOptions.Object, _mockSubscriptionManager.Object, _mockServiceProvider.Object, _mockEventBusOptions.Object);

        // Act and Assert
        await Assert.ThrowsAsync<Kafka.Exceptions.KafkaException>(() => kafkaEventBus.PublishAsync(eventWithoutTopic, new CancellationToken()));
    }

    [Theory]
    [InlineData(true, "ConsumerQueue")]
    [InlineData(false, "ConsumerWithouQueue")]
    public async Task PublishAsync_Event_InvokeHandler(bool enableQueue, string gorup)
    {
        var @event = new UserCreatedEvent()
        {
            Id = 1,
            Names = "Code",
            Lastnames = "Design Plus",
            Username = "coded",
            Birthdate = new DateTime(2019, 11, 21)
        };

        var testServer = BuildTestServer(enableQueue, this.testOutput, gorup);

        var pubSub = testServer.Host.Services.GetRequiredService<IEventBus>();

        await pubSub.PublishAsync(@event, CancellationToken.None);

        UserCreatedEvent? userEvent;

        do
        {
            await Task.Delay(TimeSpan.FromSeconds(5));

            userEvent = Startup.MemoryService.UserEventTrace.FirstOrDefault(x => x.IdEvent == @event.IdEvent);
        }
        while (userEvent == null);

        Assert.NotEmpty(Startup.MemoryService.UserEventTrace);
        Assert.NotNull(userEvent);
        Console.WriteLine("{0} {1}", @event.IdEvent, userEvent.IdEvent);
        Assert.Equal(@event.Id, userEvent.Id);
        Assert.Equal(@event.Username, userEvent.Username);
        Assert.Equal(@event.Names, userEvent.Names);
        Assert.Equal(@event.Birthdate, userEvent.Birthdate);
        Assert.Equal(@event.Lastnames, userEvent.Lastnames);
        Assert.Equal(@event.EventDate.Day, userEvent.EventDate.Day);
        Assert.Equal(@event.EventDate.Month, userEvent.EventDate.Month);
        Assert.Equal(@event.EventDate.Year, userEvent.EventDate.Year);
        Assert.Equal(@event.IdEvent, userEvent.IdEvent);
    }

    [Fact]
    public async Task SubscribeAsync_ThrowsKafkaException_WhenTopicAttributeIsMissing()
    {
        // Arrange
        var serviceCollection = new ServiceCollection().AddSingleton(x => new Mock<IConsumer<string, DummyEventWithoutTopic>>().Object);
        var serviceProvider = serviceCollection.BuildServiceProvider();

        var kafkaEventBus = new KafkaEventBus(_mockLogger.Object, _mockKafkaOptions.Object, _mockSubscriptionManager.Object, serviceProvider, _mockEventBusOptions.Object);

        // Act and Assert
        await Assert.ThrowsAsync<Kafka.Exceptions.KafkaException>(() => kafkaEventBus.SubscribeAsync<DummyEventWithoutTopic, DummyEventHandler>(new CancellationToken()));
    }

    [Fact]
    public async Task SubscribeAsync_ExitsLoop_When_CancellationTokenIsCancelled()
    {
        // Preparar
        var mockLogger = new Mock<ILogger<KafkaEventBus>>();
        var mockOptions = O.Options.Create(new KafkaOptions());
        var mockServiceProvider = new Mock<IServiceProvider>();
        var mockSubscriptionManager = new Mock<ISubscriptionManager>();
        var mockEventBusOptions = O.Options.Create(new EventBusOptions());
        var mockConsume = new Mock<IConsumer<string, UserCreatedEvent>>();

        mockConsume.Setup(x => x.Consume(It.IsAny<CancellationToken>())).Returns(new ConsumeResult<string, UserCreatedEvent>()
        {
            Message = new Message<string, UserCreatedEvent>()
            {
                Key = "dummyKey",
                Value = new UserCreatedEvent()
            }
        });

        var serviceCollection = new ServiceCollection().AddSingleton(x => mockConsume.Object);
        var serviceProvider = serviceCollection.BuildServiceProvider();

        var kafkaEventBus = new KafkaEventBus(mockLogger.Object, mockOptions, mockSubscriptionManager.Object, serviceProvider, mockEventBusOptions);

        var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(TimeSpan.FromMilliseconds(100));  // Cancela después de 100 ms

        // Actuar
        await kafkaEventBus.SubscribeAsync<UserCreatedEvent, UserCreatedEventHandler>(cancellationTokenSource.Token);

        // Afirmar
        mockLogger.VerifyLogging("Kafka event listening has stopped for event type: UserCreatedEvent due to cancellation request.", LogLevel.Information);
    }

    [Fact]
    public async Task ProcessEventAsync_LogsWarning_When_NoSubscriptionsFound()
    {
        // Arrange
        _mockSubscriptionManager.Setup(sm => sm.HasSubscriptionsForEvent<UserCreatedEvent>()).Returns(false);

        var kafkaEventBus = new KafkaEventBus(_mockLogger.Object, _mockKafkaOptions.Object, _mockSubscriptionManager.Object, _mockServiceProvider.Object, _mockEventBusOptions.Object);

        var method = kafkaEventBus.GetType().GetMethod("ProcessEventAsync", BindingFlags.NonPublic | BindingFlags.Instance);
        var genericMethod = method!.MakeGenericMethod(typeof(UserCreatedEvent), typeof(UserCreatedEventHandler));

        // Act
        await (genericMethod.Invoke(kafkaEventBus, new object[] { "dummyKey", new UserCreatedEvent(), new CancellationToken() }) as Task)!;

        // Assert 
        _mockLogger.VerifyLogging("No subscriptions found for event type UserCreatedEvent. Skipping processing.", LogLevel.Warning);
    }

    [Fact]
    public void Unsubscribe_InvokesConsumerUnsubscribeAndLogsInformation()
    {
        // Arrange
        var mockConsumer = new Mock<IConsumer<string, UserCreatedEvent>>();
        var serviceCollection = new ServiceCollection().AddSingleton(x => mockConsumer.Object);
        var serviceProvider = serviceCollection.BuildServiceProvider();

        var kafkaEventBus = new KafkaEventBus(_mockLogger.Object, _mockKafkaOptions.Object, _mockSubscriptionManager.Object, serviceProvider, _mockEventBusOptions.Object);

        // Act
        kafkaEventBus.Unsubscribe<UserCreatedEvent, UserCreatedEventHandler>();

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
            EventBus = new
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
                NameMicroservice = group
            }
        });

        var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(json));

        config.AddJsonStream(memoryStream);
    }
}
