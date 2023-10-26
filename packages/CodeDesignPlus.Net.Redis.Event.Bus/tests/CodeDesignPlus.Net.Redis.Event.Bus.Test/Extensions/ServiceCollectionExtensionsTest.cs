using O = Microsoft.Extensions.Options;
using CodeDesignPlus.Net.Redis.Event.Bus.Extensions;
using Moq;
using CodeDesignPlus.Net.Redis.Abstractions;
using CodeDesignPlus.Net.Event.Bus.Abstractions;
using CodeDesignPlus.Net.Redis.Event.Bus.Test.Helpers.Events;
using CodeDesignPlus.Net.Event.Bus.Options;
using StackExchange.Redis;

namespace CodeDesignPlus.Net.Redis.Event.Bus.Test.Extensions;

public class ServiceCollectionExtensionsTest
{
    [Fact]
    public void AddRedisEventBus_ServiceCollectionIsNull_ArgumentNullException()
    {
        // Arrange
        ServiceCollection? serviceCollection = null;

        // Act
        var exception = Assert.Throws<ArgumentNullException>(() => serviceCollection.AddRedisEventBus(null));

        // Assert
        Assert.Equal("Value cannot be null. (Parameter 'services')", exception.Message);
    }

    [Fact]
    public void AddRedisEventBus_ConfigurationIsNull_ArgumentNullException()
    {
        // Arrange
        var serviceCollection = new ServiceCollection();

        // Act
        var exception = Assert.Throws<ArgumentNullException>(() => serviceCollection.AddRedisEventBus(null));

        // Assert
        Assert.Equal("Value cannot be null. (Parameter 'configuration')", exception.Message);
    }

    [Fact]
    public void AddRedisEventBus_SectionNotExist_RedisEventBusException()
    {
        // Arrange
        var configuration = ConfigurationUtil.GetConfiguration(new object() { });

        var serviceCollection = new ServiceCollection();

        // Act
        var exception = Assert.Throws<RedisEventBusException>(() => serviceCollection.AddRedisEventBus(configuration));

        // Assert
        Assert.Equal($"The section {RedisEventBusOptions.Section} is required.", exception.Message);
    }

    [Fact]
    public void AddRedisEventBus_CheckServices_Success()
    {
        // Arrange
        var configuration = ConfigurationUtil.GetConfiguration(new { RedisEventBus = new { Enable = false } });

        var serviceCollection = new ServiceCollection();

        // Act
        serviceCollection.AddRedisEventBus(configuration);

        // Assert
        var libraryService = serviceCollection.FirstOrDefault(x => x.ServiceType == typeof(IRedisEventBusService));

        Assert.NotNull(libraryService);
        Assert.Equal(ServiceLifetime.Singleton, libraryService.Lifetime);
        Assert.Equal(typeof(RedisEventBusService), libraryService.ImplementationType);
    }

    [Fact]
    public void AddRedisEventBus_SameOptions_Success()
    {
        // Arrange
        var configuration = ConfigurationUtil.GetConfiguration(new { RedisEventBus = OptionsUtil.RedisEventBusOptions });

        var serviceCollection = new ServiceCollection();

        // Act
        serviceCollection.AddRedisEventBus(configuration);

        // Assert
        var serviceProvider = serviceCollection.BuildServiceProvider();

        var options = serviceProvider.GetService<IOptions<RedisEventBusOptions>>();
        var value = options?.Value;

        Assert.NotNull(options);
        Assert.NotNull(value);

        Assert.Equal(OptionsUtil.RedisEventBusOptions.Name, value.Name);
        Assert.Equal(OptionsUtil.RedisEventBusOptions.Enable, value.Enable);
    }

    [Fact]
    public void ListenerEvent_NoSubscriptions_LogsWarning()
    {
        // Arrange
        var mockLogger = new Mock<ILogger<RedisEventBusService>>();
        var mockRedisServiceFactory = new Mock<IRedisServiceFactory>();
        var mockRedisService = new Mock<IRedisService>();
        var mockSubscriptionManager = new Mock<ISubscriptionManager>();
        var mockServiceProvider = new Mock<IServiceProvider>();

        mockRedisServiceFactory.Setup(x => x.Create(It.IsAny<string>())).Returns(mockRedisService.Object);

        // Simular que no hay suscripciones para el evento
        mockSubscriptionManager.Setup(x => x.HasSubscriptionsForEvent<UserCreatedEvent>()).Returns(false);

        var eventBusService = new RedisEventBusService(
            mockRedisServiceFactory.Object,
            mockSubscriptionManager.Object,
            mockServiceProvider.Object,
            mockLogger.Object,
            O.Options.Create(new RedisEventBusOptions { Name = "Test" }),
            O.Options.Create(new EventBusOptions())
        );

        // Act
        eventBusService.ListenerEvent<UserCreatedEvent, UserCreatedEventHandler>(new RedisValue(JsonSerializer.Serialize(new UserCreatedEvent())), new CancellationToken());

        // Assert
        mockLogger.VerifyLogging("No subscriptions found for event: UserCreatedEvent.", LogLevel.Warning);

    }
}
