using CodeDesignPlus.Net.xUnit.Helpers;

namespace CodeDesignPlus.Net.Redis.Extensions;

public class ServiceCollectionExtensionsTest
{
    [Fact]
    public void AddRedis_ServiceCollectionIsNull_ArgumentNullException()
    {
        // Arrange
        ServiceCollection? serviceCollection = null;

        // Act
        var exception = Assert.Throws<ArgumentNullException>(() => serviceCollection.AddRedis(null));

        // Assert
        Assert.Equal("Value cannot be null. (Parameter 'services')", exception.Message);
    }

    [Fact]
    public void AddRedis_ConfigurationIsNull_ArgumentNullException()
    {
        // Arrange
        var serviceCollection = new ServiceCollection();

        // Act
        var exception = Assert.Throws<ArgumentNullException>(() => serviceCollection.AddRedis(null));

        // Assert
        Assert.Equal("Value cannot be null. (Parameter 'configuration')", exception.Message);
    }

    [Fact]
    public void AddRedis_SectionNotExist_RedisException()
    {
        // Arrange
        var configuration = ConfigurationUtil.GetConfiguration(new object() { });

        var serviceCollection = new ServiceCollection();

        // Act
        var exception = Assert.Throws<RedisException>(() => serviceCollection.AddRedis(configuration));

        // Assert
        Assert.Equal($"The section {RedisOptions.Section} is required.", exception.Message);
    }

    [Fact]
    public void AddRedis_CheckServices_Success()
    {
        // Arrange
        var configuration = ConfigurationUtil.GetConfiguration(OptionsUtil.AppSettings);

        var serviceCollection = new ServiceCollection();

        // Act
        serviceCollection.AddRedis(configuration);

        // Assert
        var redisService = serviceCollection.FirstOrDefault(x => x.ServiceType == typeof(IRedisService));
        var redisFactory = serviceCollection.FirstOrDefault(x => x.ServiceType == typeof(IRedisServiceFactory));

        Assert.NotNull(redisService);
        Assert.Equal(ServiceLifetime.Singleton, redisService.Lifetime);
        Assert.Equal(typeof(RedisService), redisService.ImplementationType);

        Assert.NotNull(redisFactory);
        Assert.Equal(ServiceLifetime.Singleton, redisFactory.Lifetime);
        Assert.Equal(typeof(RedisServiceFactory), redisFactory.ImplementationType);
    }

    [Fact]
    public void AddRedis_SameOptions_Success()
    {
        // Arrange
        var configuration = ConfigurationUtil.GetConfiguration(OptionsUtil.AppSettings);

        var serviceCollection = new ServiceCollection();

        // Act
        serviceCollection.AddRedis(configuration);

        // Assert
        var serviceProvider = serviceCollection.BuildServiceProvider();

        var options = serviceProvider.GetService<IOptions<RedisOptions>>();
        var value = options?.Value;

        Assert.NotNull(options);
        Assert.NotNull(value);
    }


}
