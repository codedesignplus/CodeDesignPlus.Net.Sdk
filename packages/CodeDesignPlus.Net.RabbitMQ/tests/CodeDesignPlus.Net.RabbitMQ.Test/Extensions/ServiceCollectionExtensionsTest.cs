using CodeDesignPlus.Net.RabbitMQ.Extensions;

namespace CodeDesignPlus.Net.RabbitMQ.Test.Extensions;

public class ServiceCollectionExtensionsTest
{
    [Fact]
    public void AddRabbitMQ_ServiceCollectionIsNull_ArgumentNullException()
    {
        // Arrange
        ServiceCollection? serviceCollection = null;

        // Act
        var exception = Assert.Throws<ArgumentNullException>(() => serviceCollection.AddRabbitMQ(null));

        // Assert
        Assert.Equal("Value cannot be null. (Parameter 'services')", exception.Message);
    }

    [Fact]
    public void AddRabbitMQ_ConfigurationIsNull_ArgumentNullException()
    {
        // Arrange
        var serviceCollection = new ServiceCollection();

        // Act
        var exception = Assert.Throws<ArgumentNullException>(() => serviceCollection.AddRabbitMQ(null));

        // Assert
        Assert.Equal("Value cannot be null. (Parameter 'configuration')", exception.Message);
    }

    [Fact]
    public void AddRabbitMQ_SectionNotExist_RabbitMQException()
    {
        // Arrange
        var configuration = ConfigurationUtil.GetConfiguration(new object() { });

        var serviceCollection = new ServiceCollection();

        // Act
        var exception = Assert.Throws<RabbitMQException>(() => serviceCollection.AddRabbitMQ(configuration));

        // Assert
        Assert.Equal($"The section {RabbitMQOptions.Section} is required.", exception.Message);
    }

    [Fact]
    public void AddRabbitMQ_CheckServices_Success()
    {
        // Arrange
        var configuration = ConfigurationUtil.GetConfiguration();

        var serviceCollection = new ServiceCollection();

        // Act
        serviceCollection.AddRabbitMQ(configuration);

        // Assert
        var libraryService = serviceCollection.FirstOrDefault(x => x.ServiceType == typeof(IRabbitPubSubService));

        Assert.NotNull(libraryService);
        Assert.Equal(ServiceLifetime.Singleton, libraryService.Lifetime);
        Assert.Equal(typeof(RabbitPubSubService), libraryService.ImplementationType);
    }

    [Fact]
    public void AddRabbitMQ_SameOptions_Success()
    {
        // Arrange
        var configuration = ConfigurationUtil.GetConfiguration();

        var serviceCollection = new ServiceCollection();

        // Act
        serviceCollection.AddRabbitMQ(configuration);

        // Assert
        var serviceProvider = serviceCollection.BuildServiceProvider();

        var options = serviceProvider.GetService<IOptions<RabbitMQOptions>>();
        var value = options?.Value;

        Assert.NotNull(options);
        Assert.NotNull(value);

        Assert.Equal(ConfigurationUtil.RabbitMQOptions.Host, value.Host);
        Assert.Equal(ConfigurationUtil.RabbitMQOptions.UserName, value.UserName);
        Assert.Equal(ConfigurationUtil.RabbitMQOptions.Enable, value.Enable);
    }


}
