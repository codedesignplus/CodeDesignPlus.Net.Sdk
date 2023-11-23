using CodeDesignPlus.Net.xUnit.Helpers;
using MongoDB.Driver;
using CodeDesignPlus.Net.Mongo.Extensions;

namespace CodeDesignPlus.Net.Mongo.Test.Extensions;

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
        var exception = Assert.Throws<Mongo.Exceptions.MongoException>(() => serviceCollection.AddMongo(configuration));

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
        var mongoClient = serviceCollection.FirstOrDefault(x => x.ServiceType == typeof(IMongoClient));

        Assert.NotNull(mongoClient);
        Assert.Equal(ServiceLifetime.Singleton, mongoClient.Lifetime);
        Assert.NotNull(mongoClient.ImplementationFactory);
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

        Assert.Equal(OptionsUtil.MongoOptions.ConnectionString, value.ConnectionString);
        Assert.Equal(OptionsUtil.MongoOptions.Database, value.Database);
        Assert.Equal(OptionsUtil.MongoOptions.Enable, value.Enable);
    }

    [Fact]
    public void AddRepositories_RepositoriesNotExist_Success()
    {
        // Arrange
        var serviceCollection = new ServiceCollection();

        // Act
        serviceCollection.AddRepositories<Guid, Guid>();

        // Assert
        var clientRepository = serviceCollection.FirstOrDefault(x => x.ServiceType == typeof(IClientRepository) && x.ImplementationType == typeof(ClientRepository));
        var productRepository = serviceCollection.FirstOrDefault(x => x.ServiceType == typeof(IProductRepository) && x.ImplementationType == typeof(ProductRepository));

        Assert.NotNull(clientRepository);
        Assert.NotNull(productRepository);
        Assert.Equal(ServiceLifetime.Singleton, clientRepository.Lifetime);
        Assert.Equal(ServiceLifetime.Singleton, productRepository.Lifetime);
    }
}
