using Microsoft.Extensions.DependencyInjection;

namespace CodeDesignPlus.Net.Mongo.Diagnostics.Extensions;

public class ServiceCollectionExtensionsTest
{
    [Fact]
    public void AddMongoDiagnostics_ServiceCollectionIsNull_ArgumentNullException()
    {
        // Arrange
        ServiceCollection? serviceCollection = null;

        // Act
        var exception = Assert.Throws<ArgumentNullException>(() => serviceCollection.AddMongoDiagnostics(configuration: null));

        // Assert
        Assert.Equal("Value cannot be null. (Parameter 'services')", exception.Message);
    }

    [Fact]
    public void AddMongoDiagnostics_ConfigurationIsNull_ArgumentNullException()
    {
        // Arrange
        var serviceCollection = new ServiceCollection();

        // Act
        var exception = Assert.Throws<ArgumentNullException>(() => serviceCollection.AddMongoDiagnostics(configuration: null));

        // Assert
        Assert.Equal("Value cannot be null. (Parameter 'configuration')", exception.Message);
    }

    [Fact]
    public void AddMongoDiagnostics_SectionNotExist_MongoDiagnosticsException()
    {
        // Arrange
        var configuration = ConfigurationUtil.GetConfiguration(new object() { });

        var serviceCollection = new ServiceCollection();

        // Act
        var exception = Assert.Throws<MongoDiagnosticsException>(() => serviceCollection.AddMongoDiagnostics(configuration));

        // Assert
        Assert.Equal($"The section {MongoDiagnosticsOptions.Section} is required.", exception.Message);
    }


    [Fact]
    public void AddMongoDiagnostics_CheckServices_Success()
    {
        // Arrange
        var configuration = ConfigurationUtil.GetConfiguration();

        var serviceCollection = new ServiceCollection();

        // Act
        serviceCollection.AddMongoDiagnostics(x =>
        {
            x.Enable = true;
            x.EnableCommandText = true;
        });

        // Assert
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var options = serviceProvider.GetService<IOptions<MongoDiagnosticsOptions>>();

        Assert.NotNull(options);

        //var libraryService = serviceCollection.FirstOrDefault(x => x.ServiceType == typeof(IMongoDiagnosticsService));

        // Assert.NotNull(libraryService);
        // Assert.Equal(ServiceLifetime.Singleton, libraryService.Lifetime);
        // Assert.Equal(typeof(MongoDiagnosticsService), libraryService.ImplementationType);
    }

    //[Fact]
    //public void AddMongoDiagnostics_CheckServices_Success()
    //{
    //    // Arrange
    //    var configuration = ConfigurationUtil.GetConfiguration();

    //    var serviceCollection = new ServiceCollection();

    //    // Act
    //    serviceCollection.AddMongoDiagnostics(configuration);

    //    // Assert
    //    var libraryService = serviceCollection.FirstOrDefault(x => x.ServiceType == typeof(IMongoDiagnosticsService));

    //    Assert.NotNull(libraryService);
    //    Assert.Equal(ServiceLifetime.Singleton, libraryService.Lifetime);
    //    Assert.Equal(typeof(MongoDiagnosticsService), libraryService.ImplementationType);
    //}

    //[Fact]
    //public void AddMongoDiagnostics_SameOptions_Success()
    //{
    //    // Arrange
    //    var configuration = ConfigurationUtil.GetConfiguration();

    //    var serviceCollection = new ServiceCollection();

    //    // Act
    //    serviceCollection.AddMongoDiagnostics(configuration);

    //    // Assert
    //    var serviceProvider = serviceCollection.BuildServiceProvider();

    //    var options = serviceProvider.GetService<IOptions<MongoDiagnosticsOptions>>();
    //    var value = options?.Value;

    //    Assert.NotNull(options);
    //    Assert.NotNull(value);

    //    Assert.Equal(ConfigurationUtil.MongoDiagnosticsOptions.Name, value.Name);
    //    Assert.Equal(ConfigurationUtil.MongoDiagnosticsOptions.Email, value.Email);
    //    Assert.Equal(ConfigurationUtil.MongoDiagnosticsOptions.Enable, value.Enable);
    //}


}
