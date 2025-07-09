using Moq;
using Microsoft.Extensions.Hosting;
using CodeDesignPlus.Net.xUnit.Extensions;
using CodeDesignPlus.Net.Observability.Extensions;

namespace CodeDesignPlus.Net.Observability.Test.Extensions;

public class ServiceCollectionExtensionsTest
{
    [Fact]
    public void AddObservability_ServiceCollectionIsNull_ArgumentNullException()
    {
        // Arrange
        ServiceCollection? serviceCollection = null;

        // Act
        var exception = Assert.Throws<ArgumentNullException>(() => serviceCollection.AddObservability(null, null));

        // Assert
        Assert.Equal("Value cannot be null. (Parameter 'services')", exception.Message);
    }

    [Fact]
    public void AddObservability_ConfigurationIsNull_ArgumentNullException()
    {
        // Arrange
        var serviceCollection = new ServiceCollection();

        // Act
        var exception = Assert.Throws<ArgumentNullException>(() => serviceCollection.AddObservability(configuration: null, environment: null));

        // Assert
        Assert.Equal("Value cannot be null. (Parameter 'configuration')", exception.Message);
    }

    [Fact]
    public void AddObservability_EnvironmentIsNull_ArgumentNullException()
    {
        // Arrange
        var serviceCollection = new ServiceCollection();
        var options = ConfigurationUtil.GetConfiguration(Test.Helpers.ConfigurationUtil.ObservabilityOptions);

        // Act
        var exception = Assert.Throws<ArgumentNullException>(() => serviceCollection.AddObservability(options, null));

        // Assert
        Assert.Equal("Value cannot be null. (Parameter 'environment')", exception.Message);
    }

    [Fact]
    public void AddObservability_SectionNotExist_ObservabilityException()
    {
        // Arrange
        var configuration = ConfigurationUtil.GetConfiguration(new object() { });
        var environment = Mock.Of<IHostEnvironment>();

        var serviceCollection = new ServiceCollection();

        // Act
        var exception = Assert.Throws<ObservabilityException>(() => serviceCollection.AddObservability(configuration, environment));

        // Assert
        Assert.Equal($"The section {ObservabilityOptions.Section} is required.", exception.Message);
    }

    [Fact]
    public void AddObservability_AddServices_Success()
    {
        // Arrange
        var configuration = ConfigurationUtil.GetConfiguration(new
        {
            Core = Test.Helpers.ConfigurationUtil.CoreOptions,
            Observability = Test.Helpers.ConfigurationUtil.ObservabilityOptions
        });
        var environment = Mock.Of<IHostEnvironment>();

        var serviceCollection = new ServiceCollection();

        // Act
        serviceCollection.AddObservability(configuration, environment);

        // Assert
        Assert.NotEmpty(serviceCollection);
    }

    [Fact]
    public void AddObservability_DisableMetricsAndTracing_Success()
    {
        // Arrange
        var options = Test.Helpers.ConfigurationUtil.ObservabilityOptions;
        options.Metrics.Enable = false;
        options.Trace.Enable = false;

        var configuration = ConfigurationUtil.GetConfiguration(new
        {
            Core = Test.Helpers.ConfigurationUtil.CoreOptions,
            Observability = options
        });
        var environment = Mock.Of<IHostEnvironment>();

        var serviceCollection = new ServiceCollection();

        // Act
        serviceCollection.AddObservability(configuration, environment);

        // Assert
        Assert.NotEmpty(serviceCollection);
        Assert.Equal(23, serviceCollection.Count);
    }

    [Fact]
    public void AddObservability_OnlyMetrics_CheckNumberServices()
    {
        // Arrange
        var options = new ObservabilityOptions()
        {
            Enable = true,
            ServerOtel = new Uri("http://localhost:4317"),
            Metrics = new Metrics()
            {
                Enable = true,
                AspNetCore = true
            },
            Trace = new Trace()
            {
                Enable = false,
                AspNetCore = true,
                CodeDesignPlusSdk = true,
                Redis = true,
                Kafka = true,
                SqlClient = true,
                GrpcClient = true
            }
        };

        var configuration = ConfigurationUtil.GetConfiguration(new
        {
            Core = Test.Helpers.ConfigurationUtil.CoreOptions,
            Observability = options
        });
        var environmentMock = new Mock<IHostEnvironment>();

        environmentMock.SetupGet(x => x.EnvironmentName).Returns(Environments.Development);

        var serviceCollection = new ServiceCollection();

        // Act
        serviceCollection.AddObservability(configuration, environmentMock.Object);

        // Assert
        Assert.NotEmpty(serviceCollection);
        Assert.Equal(40, serviceCollection.Count);
    }

    [Fact]
    public void AddObservability_OnlyTrace_CheckNumberServices()
    {
        // Arrange
        var options = new ObservabilityOptions()
        {
            Enable = true,
            ServerOtel = new Uri("http://localhost:4317"),
            Metrics = new Metrics()
            {
                Enable = false
            },
            Trace = new Trace()
            {
                Enable = true,
                AspNetCore = true,
                CodeDesignPlusSdk = true,
                Redis = true,
                Kafka = true,
                SqlClient = true,
                GrpcClient = true,
                RabbitMQ = true,
            }
        };

        var configuration = ConfigurationUtil.GetConfiguration(new
        {
            Core = Test.Helpers.ConfigurationUtil.CoreOptions,
            Observability = options
        });
        var environmentMock = new Mock<IHostEnvironment>();

        environmentMock.SetupGet(x => x.EnvironmentName).Returns(Environments.Development);

        var serviceCollection = new ServiceCollection();

        // Act
        serviceCollection.AddObservability(configuration, environmentMock.Object);

        // Assert
        Assert.NotEmpty(serviceCollection);
        Assert.Equal(54, serviceCollection.Count);
    }
}
