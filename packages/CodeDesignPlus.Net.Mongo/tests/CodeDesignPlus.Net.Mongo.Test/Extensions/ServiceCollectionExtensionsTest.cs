using CodeDesignPlus.Net.xUnit.Helpers;
using Microsoft.Extensions.DependencyInjection;

namespace CodeDesignPlus.Net.Mongo.Extensions;

public class ServiceCollectionExtensionsTest
{
    [Fact]
    public void AddMongo_ServiceCollectionIsNull_ArgumentNullException()
    {
        // Arrange
        ServiceCollection? serviceCollection = null;

        // Act
        var exception = Assert.Throws<ArgumentNullException>(() => serviceCollection.AddMongo(null));

        // Assert
        Assert.Equal("Value cannot be null. (Parameter 'services')", exception.Message);
    }

    [Fact]
    public void AddMongo_ConfigurationIsNull_ArgumentNullException()
    {
        // Arrange
        var serviceCollection = new ServiceCollection();

        // Act
        var exception = Assert.Throws<ArgumentNullException>(() => serviceCollection.AddMongo(null));

        // Assert
        Assert.Equal("Value cannot be null. (Parameter 'configuration')", exception.Message);
    }

    [Fact]
    public void AddMongo_SectionNotExist_MongoException()
    {
        // Arrange
        var configuration = ConfigurationUtil.GetConfiguration(new object() { });

        var serviceCollection = new ServiceCollection();

        // Act
        var exception = Assert.Throws<MongoException>(() => serviceCollection.AddMongo(configuration));

        // Assert
        Assert.Equal($"The section {MongoOptions.Section} is required.", exception.Message);
    }

    [Fact]
    public void AddMongo_CheckServices_Success()
    {
        // Arrange
        var configuration = ConfigurationUtil.GetConfiguration(new { Mongo = OptionsUtil.MongoOptions });

        var serviceCollection = new ServiceCollection();

        // Act
        serviceCollection.AddMongo(configuration);

        // Assert
        var libraryService = serviceCollection.FirstOrDefault(x => x.ServiceType == typeof(IMongoService));

        Assert.NotNull(libraryService);
        Assert.Equal(ServiceLifetime.Singleton, libraryService.Lifetime);
        Assert.Equal(typeof(MongoService), libraryService.ImplementationType);
    }

    [Fact]
    public void AddMongo_SameOptions_Success()
    {
        // Arrange
        var configuration = ConfigurationUtil.GetConfiguration(new { Mongo = OptionsUtil.MongoOptions });

        var serviceCollection = new ServiceCollection();

        // Act
        serviceCollection.AddMongo(configuration);

        // Assert
        var serviceProvider = serviceCollection.BuildServiceProvider();

        var options = serviceProvider.GetService<IOptions<MongoOptions>>();
        var value = options?.Value;

        Assert.NotNull(options);
        Assert.NotNull(value);

        Assert.Equal(OptionsUtil.MongoOptions.Name, value.Name);
        Assert.Equal(OptionsUtil.MongoOptions.Email, value.Email);
        Assert.Equal(OptionsUtil.MongoOptions.Enable, value.Enable);
    }


}
