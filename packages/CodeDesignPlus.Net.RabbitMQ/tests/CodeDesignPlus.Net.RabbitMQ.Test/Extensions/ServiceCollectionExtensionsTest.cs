using CodeDesignPlus.Net.RabbitMQ.Extensions;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace CodeDesignPlus.Net.RabbitMQ.Test.Extensions;

public class ServiceCollectionExtensionsTest
{
    [Fact]
    public void AddRabbitMQ_ServiceCollectionIsNull_ArgumentNullException()
    {
        // Arrange
        ServiceCollection? serviceCollection = null;

        // Act
        var exception = Assert.Throws<ArgumentNullException>(() => serviceCollection.AddRabbitMQ<ServiceCollectionExtensionsTest>(null));

        // Assert
        Assert.Equal("Value cannot be null. (Parameter 'services')", exception.Message);
    }

    [Fact]
    public void AddRabbitMQ_ConfigurationIsNull_ArgumentNullException()
    {
        // Arrange
        var serviceCollection = new ServiceCollection();

        // Act
        var exception = Assert.Throws<ArgumentNullException>(() => serviceCollection.AddRabbitMQ<ServiceCollectionExtensionsTest>(null));

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
        var exception = Assert.Throws<RabbitMQException>(() => serviceCollection.AddRabbitMQ<ServiceCollectionExtensionsTest>(configuration));

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
        serviceCollection.AddRabbitMQ<ServiceCollectionExtensionsTest>(configuration);

        // Assert
        var libraryService = serviceCollection.FirstOrDefault(x => x.ServiceType == typeof(IRabbitPubSub));

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
        serviceCollection.AddRabbitMQ<ServiceCollectionExtensionsTest>(configuration);

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

    [Fact]
    public void AddRabbitMQ_DisabledServices_NotRegisterServices()
    {
        // Arrange
        var services = new ServiceCollection();
        var options = new
        {
            Core = ConfigurationUtil.CoreOptions,
            RabbitMQ = new RabbitMQOptions()
            {
                Enable = false,
                Host = nameof(RabbitMQOptions.Host),
                UserName = $"{nameof(RabbitMQOptions.Host)}@codedesignplus.com",
                Password = nameof(RabbitMQOptions.Password),
                Port = 5672,
            }
        };

        var configuration = ConfigurationUtil.GetConfiguration(options);

        // Act
        services.AddRabbitMQ<ServiceCollectionExtensionsTest>(configuration);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        Assert.Null(serviceProvider.GetService<IRabbitConnection>());
        Assert.Null(serviceProvider.GetService<IRabbitPubSub>());
    }

    [Fact]
    public void AddRabbitMQ_HealthCheck_Success()
    {
        // Arrange
        var configuration = ConfigurationUtil.GetConfiguration();

        var serviceCollection = new ServiceCollection();

        // Act
        serviceCollection.AddRabbitMQ<ServiceCollectionExtensionsTest>(configuration);

        var serviceProvider = serviceCollection.BuildServiceProvider();

        // Assert
        var options = serviceProvider.GetRequiredService<IOptions<HealthCheckServiceOptions>>();

        var rabbit = options.Value.Registrations.FirstOrDefault(x => x.Name == "RabbitMQ");

        Assert.NotNull(rabbit);

        var client = rabbit.Factory(serviceProvider);

        Assert.NotNull(client);
    }

}
