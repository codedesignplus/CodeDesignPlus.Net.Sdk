using CodeDesignPlus.Net.Redis.PubSub.Extensions;

namespace CodeDesignPlus.Net.Redis.PubSub.Test.Extensions;

public class ServiceCollectionExtensionsTest
{
    [Fact]
    public void AddRedisPubSub_ServiceCollectionIsNull_ArgumentNullException()
    {
        // Arrange
        ServiceCollection? serviceCollection = null;

        // Act
        var exception = Assert.Throws<ArgumentNullException>(() => serviceCollection.AddRedisPubSub(null));

        // Assert
        Assert.Equal("Value cannot be null. (Parameter 'services')", exception.Message);
    }

    [Fact]
    public void AddRedisPubSub_ConfigurationIsNull_ArgumentNullException()
    {
        // Arrange
        var serviceCollection = new ServiceCollection();

        // Act
        var exception = Assert.Throws<ArgumentNullException>(() => serviceCollection.AddRedisPubSub(null));

        // Assert
        Assert.Equal("Value cannot be null. (Parameter 'configuration')", exception.Message);
    }

    [Fact]
    public void AddRedisPubSub_SectionNotExist_RedisPubSubException()
    {
        // Arrange
        var configuration = ConfigurationUtil.GetConfiguration(new object() { });

        var serviceCollection = new ServiceCollection();

        // Act
        var exception = Assert.Throws<RedisPubSubException>(() => serviceCollection.AddRedisPubSub(configuration));

        // Assert
        Assert.Equal($"The section {RedisPubSubOptions.Section} is required.", exception.Message);
    }

    [Fact]
    public void AddRedisPubSub_CheckServices_Success()
    {
        // Arrange
        var configuration = ConfigurationUtil.GetConfiguration(OptionsUtil.AppSettings);

        var serviceCollection = new ServiceCollection();

        // Act
        serviceCollection.AddRedisPubSub(configuration);

        // Assert
        var libraryService = serviceCollection.FirstOrDefault(x => x.ServiceType == typeof(IRedisPubSubService));

        Assert.NotNull(libraryService);
        Assert.Equal(ServiceLifetime.Singleton, libraryService.Lifetime);
        Assert.Equal(typeof(RedisPubSubService), libraryService.ImplementationType);
    }

    [Fact]
    public void AddRedisPubSub_SameOptions_Success()
    {
        // Arrange
        var redisPubSubOptions = OptionsUtil.RedisPubSubOptions(true);
        var configuration = ConfigurationUtil.GetConfiguration(OptionsUtil.AppSettings);

        var serviceCollection = new ServiceCollection();

        // Act
        serviceCollection.AddRedisPubSub(configuration);

        // Assert
        var serviceProvider = serviceCollection.BuildServiceProvider();

        var options = serviceProvider.GetService<IOptions<RedisPubSubOptions>>();
        var value = options?.Value;

        Assert.NotNull(options);
        Assert.NotNull(value);

        Assert.Equal(redisPubSubOptions.Enable, value.Enable);
        Assert.Equal(redisPubSubOptions.UseQueue, value.UseQueue);
        Assert.Equal(redisPubSubOptions.EnableDiagnostic, value.EnableDiagnostic);
        Assert.Equal(redisPubSubOptions.RegisterAutomaticHandlers, value.RegisterAutomaticHandlers);
        Assert.Equal(redisPubSubOptions.SecondsWaitQueue, value.SecondsWaitQueue);
    }
}
