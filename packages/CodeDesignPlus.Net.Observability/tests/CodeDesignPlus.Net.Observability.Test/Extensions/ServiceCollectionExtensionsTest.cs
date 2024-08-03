using Moq;
using Microsoft.Extensions.Hosting;

namespace CodeDesignPlus.Net.Observability.Extensions;

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
        var options = xUnit.Helpers.ConfigurationUtil.GetConfiguration(Test.Helpers.ConfigurationUtil.ObservabilityOptions);

        // Act
        var exception = Assert.Throws<ArgumentNullException>(() => serviceCollection.AddObservability(options, null));

        // Assert
        Assert.Equal("Value cannot be null. (Parameter 'environment')", exception.Message);
    }

    [Fact]
    public void AddObservability_SectionNotExist_ObservabilityException()
    {
        // Arrange
        var configuration = xUnit.Helpers.ConfigurationUtil.GetConfiguration(new object() { });
        var environment = Mock.Of<IHostEnvironment>();

        var serviceCollection = new ServiceCollection();

        // Act
        var exception = Assert.Throws<ObservabilityException>(() => serviceCollection.AddObservability(configuration, environment));

        // Assert
        Assert.Equal($"The section {ObservabilityOptions.Section} is required.", exception.Message);
    }
}
