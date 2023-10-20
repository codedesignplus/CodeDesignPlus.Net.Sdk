using Microsoft.Extensions.DependencyInjection;

namespace CodeDesignPlus.Net.Redis.Event.Bus.Extensions;

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
        var configuration = ConfigurationUtil.GetConfiguration();

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
        var configuration = ConfigurationUtil.GetConfiguration();

        var serviceCollection = new ServiceCollection();

        // Act
        serviceCollection.AddRedisEventBus(configuration);

        // Assert
        var serviceProvider = serviceCollection.BuildServiceProvider();

        var options = serviceProvider.GetService<IOptions<RedisEventBusOptions>>();
        var value = options?.Value;

        Assert.NotNull(options);
        Assert.NotNull(value);

        Assert.Equal(ConfigurationUtil.RedisEventBusOptions.Name, value.Name);
        Assert.Equal(ConfigurationUtil.RedisEventBusOptions.Email, value.Email);
        Assert.Equal(ConfigurationUtil.RedisEventBusOptions.Enable, value.Enable);
    }


}
