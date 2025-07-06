using CodeDesignPlus.Net.Microservice.Commons.EntryPoints.Rest.Resources;

namespace CodeDesignPlus.Net.Microservice.Commons.Test.EntryPoints.Rest.Resources;

public class ResourcesExtensionsTest
{
    private class DummyProgram { }

    [Fact]
    public void AddResources_ThrowsArgumentNullException_WhenServicesIsNull()
    {
        // Arrange
        IServiceCollection services = null!;
        var configuration = new ConfigurationBuilder().Build();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => ResourcesExtensions.AddResources<DummyProgram>(services, configuration));
    }

    [Fact]
    public void AddResources_ThrowsArgumentNullException_WhenConfigurationIsNull()
    {
        // Arrange
        var services = new ServiceCollection();
        IConfiguration configuration = null!;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => ResourcesExtensions.AddResources<DummyProgram>(services, configuration));
    }

    [Fact]
    public void AddResources_ReturnsServices_WhenEnableIsFalse()
    {
        // Arrange
        var inMemorySettings = new Dictionary<string, string?>
        {
            [$"{ResourcesOptions.Section}:Enable"] = "false"
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        var services = new ServiceCollection();

        // Act
        var result = ResourcesExtensions.AddResources<DummyProgram>(services, configuration);

        // Assert
        Assert.Same(services, result);
        Assert.DoesNotContain(services, s => s.ServiceType == typeof(ResourceHealtCheck));
    }

    [Fact]
    public void AddResources_RegistersExpectedServices_WhenEnableIsTrue()
    {
        // Arrange
        var inMemorySettings = new Dictionary<string, string?>
        {
            [$"{ResourcesOptions.Section}:Enable"] = "true",
            [$"{ResourcesOptions.Section}:Server"] = "http://localhost"
        };
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        var services = new ServiceCollection();

        // Act
        var result = ResourcesExtensions.AddResources<DummyProgram>(services, configuration);
        var serviceProvider = result.BuildServiceProvider();

        // Assert
        Assert.Same(services, result);
        Assert.Contains(services, s => s.ServiceType == typeof(ResourceHealtCheck));
        Assert.Contains(services, s => s.ServiceType.Name.Contains("IHostedService"));
        Assert.Contains(services, s => s.ServiceType.Name.Contains("ServiceClient"));

        var serviceClient = serviceProvider.GetService<Services.gRpc.Service.ServiceClient>();

        Assert.NotNull(serviceClient);
    }
}
