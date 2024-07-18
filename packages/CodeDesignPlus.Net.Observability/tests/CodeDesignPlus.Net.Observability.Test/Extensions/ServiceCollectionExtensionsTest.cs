namespace CodeDesignPlus.Net.Observability.Extensions;

public class ServiceCollectionExtensionsTest
{
    [Fact]
    public void AddObservability_ServiceCollectionIsNull_ArgumentNullException()
    {
        // Arrange
        ServiceCollection? serviceCollection = null;

        // Act
        var exception = Assert.Throws<ArgumentNullException>(() => serviceCollection.AddObservability(null));

        // Assert
        Assert.Equal("Value cannot be null. (Parameter 'services')", exception.Message);
    }

    [Fact]
    public void AddObservability_ConfigurationIsNull_ArgumentNullException()
    {
        // Arrange
        var serviceCollection = new ServiceCollection();

        // Act
        var exception = Assert.Throws<ArgumentNullException>(() => serviceCollection.AddObservability(null));

        // Assert
        Assert.Equal("Value cannot be null. (Parameter 'configuration')", exception.Message);
    }

    [Fact]
    public void AddObservability_SectionNotExist_ObservabilityException()
    {
        // Arrange
        var configuration = xUnit.Helpers.ConfigurationUtil.GetConfiguration(new object() { });

        var serviceCollection = new ServiceCollection();

        // Act
        var exception = Assert.Throws<ObservabilityException>(() => serviceCollection.AddObservability(configuration));

        // Assert
        Assert.Equal($"The section {ObservabilityOptions.Section} is required.", exception.Message);
    }

    // [Fact]
    // public void AddObservability_CheckServices_Success()
    // {
    //     // Arrange
    //     var configuration = xUnit.Helpers.ConfigurationUtil.GetConfiguration();

    //     var serviceCollection = new ServiceCollection();

    //     // Act
    //     serviceCollection.AddObservability(configuration);

    //     // Assert
    //     var libraryService = serviceCollection.FirstOrDefault(x => x.ServiceType == typeof(IObservabilityService));

    //     Assert.NotNull(libraryService);
    //     Assert.Equal(ServiceLifetime.Singleton, libraryService.Lifetime);
    //     Assert.Equal(typeof(ObservabilityService), libraryService.ImplementationType);
    // }

    // [Fact]
    // public void AddObservability_SameOptions_Success()
    // {
    //     // Arrange
    //     var configuration = xUnit.Helpers.ConfigurationUtil.GetConfiguration();

    //     var serviceCollection = new ServiceCollection();

    //     // Act
    //     serviceCollection.AddObservability(configuration);

    //     // Assert
    //     var serviceProvider = serviceCollection.BuildServiceProvider();

    //     var options = serviceProvider.GetService<IOptions<ObservabilityOptions>>();
    //     var value = options?.Value;

    //     Assert.NotNull(options);
    //     Assert.NotNull(value);

    //     Assert.Equal(ConfigurationUtil.ObservabilityOptions.Name, value.Name);
    //     Assert.Equal(ConfigurationUtil.ObservabilityOptions.Email, value.Email);
    //     Assert.Equal(ConfigurationUtil.ObservabilityOptions.Enable, value.Enable);
    // }


}
