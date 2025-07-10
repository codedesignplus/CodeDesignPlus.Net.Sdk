using CodeDesignPlus.Net.Core.Abstractions;
using CodeDesignPlus.Net.Core.Abstractions.Options;
using CodeDesignPlus.Net.Kafka.Test.Helpers.Events;
using CodeDesignPlus.Net.PubSub.Abstractions;
using CodeDesignPlus.Net.PubSub.Abstractions.Options;
using CodeDesignPlus.Net.xUnit.Extensions;
using CodeDesignPlus.Net.xUnit.Containers.KafkaContainer;
using CodeDesignPlus.Net.xUnit.Output.Loggers;
using Confluent.Kafka;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;
using Moq;
using Xunit.Abstractions;
using CodeDesignPlus.Net.Kafka.Test.Helpers;
using CodeDesignPlus.Net.Core.Services;

namespace CodeDesignPlus.Net.Kafka.Test.Services;

[Collection(KafkaCollectionFixture.Collection)]
public class KafkaPubSubTest
{
    private readonly IOptions<CoreOptions> coreOptions;
    private readonly KafkaContainer kafkaContainer;
    private readonly ITestOutputHelper testOutput;

    private readonly Mock<ILogger<KafkaPubSub>> _mockLogger = new();
    private readonly Mock<IOptions<KafkaOptions>> _mockKafkaOptions = new();
    private readonly IServiceProvider serviceProvider;
    private readonly Mock<IDomainEventResolver> _mockDomainEventResolverService = new();
    private readonly Mock<IProducer<string, IDomainEvent>> _mockProducer = new();


    public KafkaPubSubTest(ITestOutputHelper output, KafkaCollectionFixture kafkaCollectionFixture)
    {
        this.kafkaContainer = kafkaCollectionFixture.Container;
        this.testOutput = output;
        this.coreOptions = Microsoft.Extensions.Options.Options.Create(OptionUtils.CoreOptions);

        var serviceCollection = new ServiceCollection()
            .AddSingleton(x => _mockProducer.Object);

        this.serviceProvider = serviceCollection.BuildServiceProvider();
    }

    [Theory]
    [InlineData(true, "ConsumerQueue")]
    [InlineData(false, "ConsumerWithouQueue")]
    public async Task PublishAsync_Event_InvokeHandler(bool enableQueue, string group)
    {
        var @event = new UserCreatedEvent(Guid.NewGuid())
        {
            Names = "Code",
            Lastnames = "Design Plus",
            Username = "coded",
            Birthdate =  new DateTime(2019, 11, 21, 0, 0, 0, DateTimeKind.Utc),
        };

        @event.Metadata.Add("key", "value");

        var testServer = BuildTestServer(enableQueue, this.testOutput, group);

        var pubSub = testServer.Host.Services.GetRequiredService<IMessage>();

        await Task.Delay(TimeSpan.FromSeconds(2));

        await pubSub.PublishAsync([@event], CancellationToken.None);

        UserCreatedEvent? userEvent;

        do
        {
            await Task.Delay(TimeSpan.FromSeconds(5));

            userEvent = Startup.MemoryService.UserEventTrace.First(x => x.EventId == @event.EventId);
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
        Assert.Equal(@event.Metadata, userEvent.Metadata);
    }

    [Fact]
    public async Task Unsubscribe_InvokesConsumerUnsubscribeAndLogsInformation()
    {
        // Arrange
        var mockConsumer = new Mock<IConsumer<string, UserCreatedEvent>>();
        var serviceCollection = new ServiceCollection().AddSingleton(x => mockConsumer.Object);
        var provider = serviceCollection.BuildServiceProvider();

        var kafkaEventBus = new KafkaPubSub(_mockLogger.Object, _mockDomainEventResolverService.Object, _mockKafkaOptions.Object, provider, this.coreOptions);

        // Act
        await kafkaEventBus.UnsubscribeAsync<UserCreatedEvent, UserCreatedEventHandler>(CancellationToken.None);

        // Assert
        mockConsumer.Verify(x => x.Unsubscribe(), Times.Once);
        _mockLogger.VerifyLogging($"Unsubscribing from event {typeof(UserCreatedEvent).Name} for handler {typeof(UserCreatedEventHandler).Name}", LogLevel.Information);
    }

    [Fact]
    public async Task SubscribeAsync_MaxRetry_WriteLoggerError()
    {
        // Arrange
        var topic = nameof(SubscribeAsync_MaxRetry_WriteLoggerError);
        _mockDomainEventResolverService.Setup(x => x.GetKeyDomainEvent<ProductCreatedEvent>()).Returns(topic);

        var mockConsumer = new Mock<IConsumer<string, ProductCreatedEvent>>();
        var serviceCollection = new ServiceCollection()
            .AddSingleton(x => mockConsumer.Object)
            .AddScoped<IEventContext, EventContext>();
            
        var provider = serviceCollection.BuildServiceProvider();
        var maxAttempts = 3;
        var options = Microsoft.Extensions.Options.Options.Create(new KafkaOptions
        {
            Enable = true,
            BootstrapServers = kafkaContainer.BrokerList,
            Acks = Acks.All ,
            BatchSize = 4096,
            LingerMs = 5,
            CompressionType = CompressionType.Gzip,
            MaxAttempts = maxAttempts
        });

        var kafkaEventBus = new KafkaPubSub(_mockLogger.Object, _mockDomainEventResolverService.Object, options, provider, this.coreOptions);

        // Act
        _ = Task.Run(() => kafkaEventBus.SubscribeAsync<ProductCreatedEvent, ProductCreatedEventHandler>(CancellationToken.None));

        await Task.Delay(TimeSpan.FromSeconds(10));

        // Assert
        _mockLogger.VerifyLogging($"{typeof(ProductCreatedEvent).Name} | Subscribing to Kafka topic {topic} ", LogLevel.Information);

        _mockLogger.VerifyLogging($"{typeof(ProductCreatedEvent).Name} | The topic {topic} does not exist, waiting for it to be created.", LogLevel.Information, Times.Exactly(maxAttempts - 1));

        _mockLogger.VerifyLogging($"{typeof(ProductCreatedEvent).Name} | The topic {topic} does not exist after {maxAttempts} attempts. Exiting.", LogLevel.Warning);

        _mockLogger.VerifyLogging($"{typeof(ProductCreatedEvent).Name} | Listener the event {topic}", LogLevel.Information, Times.AtLeastOnce());
    }

    private TestServer BuildTestServer(bool enableQueue, ITestOutputHelper output, string group)
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


    private void AddJsonStream(IConfigurationBuilder config, bool enableQueue, string group)
    {
        var json = JsonSerializer.Serialize(new
        {
            Core = new CoreOptions
            {
                Id = Guid.NewGuid(),
                AppName = "ms-kafka-test",
                TypeEntryPoint = "rest",
                Version = "v1",
                Business = "CodeDesignPlus",
                Description = "Microservice Test",
                Contact = new Contact
                {
                    Email = "codedesignplus@outlook.com",
                    Name = "CodeDesignPlus",
                }
            },
            PubSub = new
            {
                EnableQueue = enableQueue,
            },
            Kafka = new
            {
                Enable = true,
                BootstrapServers = kafkaContainer.BrokerList,
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
