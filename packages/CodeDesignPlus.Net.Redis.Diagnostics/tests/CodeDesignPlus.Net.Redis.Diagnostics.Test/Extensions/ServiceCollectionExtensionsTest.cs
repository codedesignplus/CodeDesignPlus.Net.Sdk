using Microsoft.Extensions.DependencyInjection;

namespace CodeDesignPlus.Net.Redis.Diagnostics.Extensions;

public class ServiceCollectionExtensionsTest
{
    [Fact]
    public void AddRedisDiagnostics_ServiceCollectionIsNull_ArgumentNullException()
    {
        // Arrange
        ServiceCollection? serviceCollection = null;

        // Act
        var exception = Assert.Throws<ArgumentNullException>(() => serviceCollection.AddRedisDiagnostics(null));

        // Assert
        Assert.Equal("Value cannot be null. (Parameter 'services')", exception.Message);
    }

    [Fact]
    public void AddRedisDiagnostics_ConfigurationIsNull_ArgumentNullException()
    {
        // Arrange
        var serviceCollection = new ServiceCollection();

        // Act
        var exception = Assert.Throws<ArgumentNullException>(() => serviceCollection.AddRedisDiagnostics(null));

        // Assert
        Assert.Equal("Value cannot be null. (Parameter 'configuration')", exception.Message);
    }

    [Fact]
    public void AddRedisDiagnostics_SectionNotExist_RedisDiagnosticsException()
    {
        // Arrange
        var configuration = ConfigurationUtil.GetConfiguration(new object() { });

        var serviceCollection = new ServiceCollection();

        // Act
        var exception = Assert.Throws<RedisDiagnosticsException>(() => serviceCollection.AddRedisDiagnostics(configuration));

        // Assert
        Assert.Equal($"The section {RedisDiagnosticsOptions.Section} is required.", exception.Message);
    }

    [Fact]
    public void AddRedisDiagnostics_CheckServices_Success()
    {
        // Arrange
        var configuration = ConfigurationUtil.GetConfiguration();

        var serviceCollection = new ServiceCollection();

        // Act
        serviceCollection.AddRedisDiagnostics(configuration);

        // Assert
        var libraryService = serviceCollection.FirstOrDefault(x => x.ServiceType == typeof(IRedisDiagnosticsService));

        Assert.NotNull(libraryService);
        Assert.Equal(ServiceLifetime.Singleton, libraryService.Lifetime);
        Assert.Equal(typeof(RedisDiagnosticsService), libraryService.ImplementationType);
    }

    [Fact]
    public void AddRedisDiagnostics_SameOptions_Success()
    {
        // Arrange
        var configuration = ConfigurationUtil.GetConfiguration();

        var serviceCollection = new ServiceCollection();

        // Act
        serviceCollection.AddRedisDiagnostics(configuration);

        // Assert
        var serviceProvider = serviceCollection.BuildServiceProvider();

        var options = serviceProvider.GetService<IOptions<RedisDiagnosticsOptions>>();
        var value = options?.Value;

        Assert.NotNull(options);
        Assert.NotNull(value);

        Assert.Equal(ConfigurationUtil.RedisDiagnosticsOptions.Name, value.Name);
        Assert.Equal(ConfigurationUtil.RedisDiagnosticsOptions.Email, value.Email);
        Assert.Equal(ConfigurationUtil.RedisDiagnosticsOptions.Enable, value.Enable);
    }


}
