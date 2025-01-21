using CodeDesignPlus.Net.Mongo.Serializers;
using CodeDesignPlus.Net.Mongo.Test.Helpers.Models;
using CodeDesignPlus.Net.Security.Abstractions;
using CodeDesignPlus.Net.xUnit.Containers.MongoContainer;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using Moq;

namespace CodeDesignPlus.Net.Mongo.Test.Operations;


[Collection(MongoCollectionFixture.Collection)]
public class OperationBaseTest : IClassFixture<MongoContainer>
{
    private readonly Mock<ILogger<ProductRepository>> loggerMock;
    private readonly MongoContainer mongoContainer;
    private readonly IOptions<MongoOptions> options;

    public OperationBaseTest(MongoContainer mongoContainer)
    {
        try
        {
            BsonSerializer.TryRegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        this.mongoContainer = mongoContainer;
        this.loggerMock = new Mock<ILogger<ProductRepository>>();
        this.options = Microsoft.Extensions.Options.Options.Create(OptionsUtil.GetOptions(this.mongoContainer.Port));
    }

    [Fact]
    public async Task CreateAsync_WhenEntityIsValid_ThenReturnIdEntity()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        var (client, collection) = GetCollection();
        var serviceProvider = GetServiceProvider(client, collection);

        var idUser = Guid.NewGuid();
        var date = new LocalDateTime(2021, 1, 1, 0, 0, 0).InUtc().ToInstant();
        var product = new ProductEntity
        {
            IsActive = true,
            Id = Guid.NewGuid(),
            CreatedAt = date,
            CreatedBy = idUser
        };

        await Task.Delay(1000);

        var userContextMock = new Mock<IUserContext>();

        userContextMock.SetupGet(x => x.IdUser).Returns(idUser);

        var repository = new ProductRepository(userContextMock.Object, serviceProvider, this.options, this.loggerMock.Object);

        // Act
        await repository.CreateAsync(product, CancellationToken.None);

        // Assert
        var result = await collection.Find(x => x.Id == product.Id).FirstOrDefaultAsync(cancellationToken);

        Assert.NotNull(result);
        Assert.Equal(product.Id, result.Id);
        Assert.Equal(product.IsActive, result.IsActive);
        Assert.Equal(idUser, result.CreatedBy);
    }

    [Fact]
    public async Task DeleteAsync_WhenEntityIsValid_ThenReturnTrue()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        var (client, collection) = GetCollection();
        var serviceProvider = GetServiceProvider(client, collection);

        var idUser = Guid.NewGuid();
        var date = SystemClock.Instance.GetCurrentInstant();
        var product = new ProductEntity
        {
            IsActive = true,
            Id = Guid.NewGuid(),
            CreatedAt = date,
            CreatedBy = idUser
        };

        var userContextMock = new Mock<IUserContext>();

        userContextMock.SetupGet(x => x.IdUser).Returns(idUser);

        var repository = new ProductRepository(userContextMock.Object, serviceProvider, this.options, this.loggerMock.Object);

        await repository.CreateAsync(product, CancellationToken.None);

        // Act
        await repository.DeleteAsync(product.Id, CancellationToken.None);

        // Assert
        var result = await collection.Find(x => x.Id == product.Id).FirstOrDefaultAsync(cancellationToken);

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateAsync_WhenEntityIsValid_ThenReturnTrue()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        var (client, collection) = GetCollection();
        var serviceProvider = GetServiceProvider(client, collection);

        var idUser = Guid.NewGuid();
        var date = SystemClock.Instance.GetCurrentInstant();
        var product = new ProductEntity
        {
            IsActive = true,
            Id = Guid.NewGuid(),
            CreatedAt = date,
            CreatedBy = idUser
        };

        var userContextMock = new Mock<IUserContext>();

        userContextMock.SetupGet(x => x.IdUser).Returns(idUser);

        var repository = new ProductRepository(userContextMock.Object, serviceProvider, this.options, this.loggerMock.Object);

        await repository.CreateAsync(product, CancellationToken.None);

        // Act
        product.IsActive = false;
        await repository.UpdateAsync(product.Id, product, CancellationToken.None);

        // Assert
        var result = await collection.Find(x => x.Id == product.Id).FirstOrDefaultAsync(cancellationToken);

        Assert.NotNull(result);
        Assert.Equal(product.Id, result.Id);
        Assert.Equal(product.IsActive, result.IsActive);
        Assert.Equal(product.CreatedBy, result.CreatedBy);
    }

    private static ServiceProvider GetServiceProvider(IMongoClient mongoClient, IMongoCollection<ProductEntity> collection)
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddSingleton(mongoClient);
        serviceCollection.AddSingleton(collection);
        var serviceProvider = serviceCollection.BuildServiceProvider();
        return serviceProvider;
    }

    private (IMongoClient, IMongoCollection<ProductEntity>) GetCollection()
    {
        var connectionString = OptionsUtil.GetOptions(this.mongoContainer.Port).ConnectionString;

        var client = new MongoClient(connectionString);
        var database = client.GetDatabase("dbtestmongo");
        var collection = database.GetCollection<ProductEntity>(typeof(ProductEntity).Name);
        return (client, collection);
    }
}