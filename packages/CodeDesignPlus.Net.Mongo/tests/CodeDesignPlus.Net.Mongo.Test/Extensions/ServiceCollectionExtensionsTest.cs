using CodeDesignPlus.Net.Mongo.Extensions;
using CodeDesignPlus.Net.xUnit.Extensions;
using CodeDesignPlus.Net.xUnit.Containers.MongoContainer;
using MongoDB.Driver;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace CodeDesignPlus.Net.Mongo.Test.Extensions;


[Collection(MongoCollectionFixture.Collection)]

public class ServiceCollectionExtensionsTest(MongoCollectionFixture fixture)
{
    [Fact]
    public void AddMongo_ServiceCollectionIsNull_ArgumentNullException()
    {
        // Arrange
        ServiceCollection? serviceCollection = null;

        // Act
        var exception = Assert.Throws<ArgumentNullException>(() => serviceCollection.AddMongo<StartupFake>(null));

        // Assert
        Assert.Equal("Value cannot be null. (Parameter 'services')", exception.Message);
    }

    [Fact]
    public void AddMongo_ConfigurationIsNull_ArgumentNullException()
    {
        // Arrange
        var serviceCollection = new ServiceCollection();

        // Act
        var exception = Assert.Throws<ArgumentNullException>(() => serviceCollection.AddMongo<StartupFake>(null));

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
        var exception = Assert.Throws<Mongo.Exceptions.MongoException>(() => serviceCollection.AddMongo<StartupFake>(configuration));

        // Assert
        Assert.Equal($"The section {MongoOptions.Section} is required.", exception.Message);
    }

    [Fact]
    public void AddMongo_CheckServices_Success()
    {
        // Arrange
        var configuration = ConfigurationUtil.GetConfiguration(new { Mongo = OptionsUtil.GetOptions(fixture.Container.Port) });

        var serviceCollection = new ServiceCollection();

        // Act
        serviceCollection.AddLogging();
        serviceCollection.AddMongo<StartupFake>(configuration);

        var serviceProvider = serviceCollection.BuildServiceProvider();

        // Assert
        var mongoClient = serviceCollection.FirstOrDefault(x => x.ServiceType == typeof(IMongoClient));

        Assert.NotNull(mongoClient);
        Assert.Equal(ServiceLifetime.Singleton, mongoClient.Lifetime);
        Assert.NotNull(mongoClient.ImplementationFactory);

        // Assert
        var client = serviceProvider.GetService<IMongoClient>();

        Assert.NotNull(client);
        Assert.IsType<MongoClient>(client);
    }

    [Fact]
    public void AddMongo_SameOptions_Success()
    {
        // Arrange
        var configuration = ConfigurationUtil.GetConfiguration(new { Mongo = OptionsUtil.MongoOptions });

        var serviceCollection = new ServiceCollection();

        // Act
        serviceCollection.AddMongo<StartupFake>(configuration);

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
        serviceCollection.AddRepositories<StartupFake>();

        // Assert
        var clientRepository = serviceCollection.FirstOrDefault(x => x.ServiceType == typeof(IClientRepository) && x.ImplementationType == typeof(ClientRepository));
        var productRepository = serviceCollection.FirstOrDefault(x => x.ServiceType == typeof(IProductRepository) && x.ImplementationType == typeof(ProductRepository));

        Assert.NotNull(clientRepository);
        Assert.NotNull(productRepository);
        Assert.Equal(ServiceLifetime.Singleton, clientRepository.Lifetime);
        Assert.Equal(ServiceLifetime.Singleton, productRepository.Lifetime);
    }


    [Fact]
    public void AddMongo_Disable_NotRegisterServices()
    {
        // Arrange
        var options = OptionsUtil.GetOptions(fixture.Container.Port);
        options.Enable = false;

        var configuration = ConfigurationUtil.GetConfiguration(new
        {
            Mongo = options
        });

        var serviceCollection = new ServiceCollection();

        // Act
        serviceCollection.AddMongo<StartupFake>(configuration);

        // Assert
        var mongoClient = serviceCollection.FirstOrDefault(x => x.ServiceType == typeof(IMongoClient));

        Assert.Null(mongoClient);
    }

    [Fact]
    public void AddMongo_HealthCheck_Success()
    {
        // Arrange
        var configuration = ConfigurationUtil.GetConfiguration(new { Mongo = OptionsUtil.MongoOptions });

        var serviceCollection = new ServiceCollection();

        // Act
        serviceCollection.AddMongo<StartupFake>(configuration);

        var serviceProvider = serviceCollection.BuildServiceProvider();

        // Assert
        var options = serviceProvider.GetRequiredService<IOptions<HealthCheckServiceOptions>>();

        var mongo = options.Value.Registrations.FirstOrDefault(x => x.Name == "MongoDB");

        Assert.NotNull(mongo);

        var client = mongo.Factory(serviceProvider);

        Assert.NotNull(client);
    }
}
