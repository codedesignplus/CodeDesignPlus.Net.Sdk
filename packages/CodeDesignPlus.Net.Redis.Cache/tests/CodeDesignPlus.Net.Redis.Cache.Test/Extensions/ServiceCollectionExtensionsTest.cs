using CodeDesignPlus.Net.Redis.Cache.Abstractions.Options;
using CodeDesignPlus.Net.Redis.Cache.Extensions;
using CodeDesignPlus.Net.xUnit.Extensions;

namespace CodeDesignPlus.Net.Redis.Cache.Test.Extensions;

public class ServiceCollectionExtensionsTest
{
    [Fact]
    public void AddCache_ServiceCollectionIsNull_ArgumentNullException()
    {
        // Arrange
        ServiceCollection? serviceCollection = null;

        // Act
        var exception = Assert.Throws<ArgumentNullException>(() => serviceCollection.AddCache(null));

        // Assert
        Assert.Equal("Value cannot be null. (Parameter 'services')", exception.Message);
    }

    [Fact]
    public void AddCache_ConfigurationIsNull_ArgumentNullException()
    {
        // Arrange
        var serviceCollection = new ServiceCollection();

        // Act
        var exception = Assert.Throws<ArgumentNullException>(() => serviceCollection.AddCache(null));

        // Assert
        Assert.Equal("Value cannot be null. (Parameter 'configuration')", exception.Message);
    }

    [Fact]
    public void AddCache_SectionNotExist_RedisCacheException()
    {
        // Arrange
        var configuration = ConfigurationUtil.GetConfiguration(new { });

        var serviceCollection = new ServiceCollection();

        // Act
        var exception = Assert.Throws<RedisCacheException>(() => serviceCollection.AddCache(configuration));

        // Assert
        Assert.Equal($"The section {RedisCacheOptions.Section} is required.", exception.Message);
    }

    [Fact]
    public void AddCache_CheckServices_Success()
    {
        // Arrange
        var configuration = ConfigurationUtil.GetConfiguration(new { RedisCache = new RedisCacheOptions { Enable = true } });

        var serviceCollection = new ServiceCollection();

        // Act
        serviceCollection.AddCache(configuration);

        // Assert
        var cacheManager = serviceCollection.FirstOrDefault(x => x.ServiceType == typeof(IRedisCacheManager));
        var redisCacheManager = serviceCollection.FirstOrDefault(x => x.ServiceType == typeof(IRedisCacheManager));

        Assert.NotNull(cacheManager);
        Assert.Equal(ServiceLifetime.Singleton, cacheManager.Lifetime);
        Assert.Equal(typeof(RedisCacheManager), cacheManager.ImplementationType);

        Assert.NotNull(redisCacheManager);
        Assert.Equal(ServiceLifetime.Singleton, redisCacheManager.Lifetime);
        Assert.Equal(typeof(RedisCacheManager), redisCacheManager.ImplementationType);
    }

    [Fact]
    public void AddCache_SameOptions_Success()
    {
        // Arrange
        var redisCacheOptions = new RedisCacheOptions { Enable = true, Expiration = TimeSpan.FromSeconds(10) };
        var configuration = ConfigurationUtil.GetConfiguration(new { RedisCache = redisCacheOptions });

        var serviceCollection = new ServiceCollection();

        // Act
        serviceCollection.AddCache(configuration);

        // Assert
        var serviceProvider = serviceCollection.BuildServiceProvider();

        var options = serviceProvider.GetService<IOptions<RedisCacheOptions>>();
        var value = options?.Value;

        Assert.NotNull(options);
        Assert.NotNull(value);

        Assert.Equal(redisCacheOptions.Expiration, value.Expiration);
    }
}
