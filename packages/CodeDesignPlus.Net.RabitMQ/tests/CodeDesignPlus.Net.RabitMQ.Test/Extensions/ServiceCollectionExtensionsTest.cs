using CodeDesignPlus.Net.RabitMQ.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace CodeDesignPlus.Net.RabitMQ.Test.Extensions;

public class ServiceCollectionExtensionsTest
{
    [Fact]
    public void AddRabitMQ_ServiceCollectionIsNull_ArgumentNullException()
    {
        // Arrange
        ServiceCollection? serviceCollection = null;

        // Act
        var exception = Assert.Throws<ArgumentNullException>(() => serviceCollection.AddRabitMQ(null));

        // Assert
        Assert.Equal("Value cannot be null. (Parameter 'services')", exception.Message);
    }

    [Fact]
    public void AddRabitMQ_ConfigurationIsNull_ArgumentNullException()
    {
        // Arrange
        var serviceCollection = new ServiceCollection();

        // Act
        var exception = Assert.Throws<ArgumentNullException>(() => serviceCollection.AddRabitMQ(null));

        // Assert
        Assert.Equal("Value cannot be null. (Parameter 'configuration')", exception.Message);
    }

    [Fact]
    public void AddRabitMQ_SectionNotExist_RabitMQException()
    {
        // Arrange
        var configuration = ConfigurationUtil.GetConfiguration(new object() { });

        var serviceCollection = new ServiceCollection();

        // Act
        var exception = Assert.Throws<RabitMQException>(() => serviceCollection.AddRabitMQ(configuration));

        // Assert
        Assert.Equal($"The section {RabitMQOptions.Section} is required.", exception.Message);
    }

    [Fact]
    public void AddRabitMQ_CheckServices_Success()
    {
        // Arrange
        var configuration = ConfigurationUtil.GetConfiguration();

        var serviceCollection = new ServiceCollection();

        // Act
        serviceCollection.AddRabitMQ(configuration);

        // Assert
        var libraryService = serviceCollection.FirstOrDefault(x => x.ServiceType == typeof(IRabitPubSubService));

        Assert.NotNull(libraryService);
        Assert.Equal(ServiceLifetime.Singleton, libraryService.Lifetime);
        Assert.Equal(typeof(RabitPubSubService), libraryService.ImplementationType);
    }

    [Fact]
    public void AddRabitMQ_SameOptions_Success()
    {
        // Arrange
        var configuration = ConfigurationUtil.GetConfiguration();

        var serviceCollection = new ServiceCollection();

        // Act
        serviceCollection.AddRabitMQ(configuration);

        // Assert
        var serviceProvider = serviceCollection.BuildServiceProvider();

        var options = serviceProvider.GetService<IOptions<RabitMQOptions>>();
        var value = options?.Value;

        Assert.NotNull(options);
        Assert.NotNull(value);

        Assert.Equal(ConfigurationUtil.RabitMQOptions.Host, value.Host);
        Assert.Equal(ConfigurationUtil.RabitMQOptions.UserName, value.UserName);
        Assert.Equal(ConfigurationUtil.RabitMQOptions.Enable, value.Enable);
    }


}
