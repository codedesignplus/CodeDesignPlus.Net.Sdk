using CodeDesignPlus.Net.Mongo.Test.Helpers.Models;
using CodeDesignPlus.Net.xUnit.Helpers;
using CodeDesignPlus.Net.xUnit.Helpers.MongoContainer;
using MongoDB.Driver;
using Moq;

namespace CodeDesignPlus.Net.Mongo.Test.Repository;

public class RepositoryBaseTest : IClassFixture<MongoContainer>
{
    private readonly Mock<ILogger<ClientRepository>> loggerMock;
    private readonly MongoContainer mongoContainer;

    private readonly IOptions<MongoOptions> options;

    public RepositoryBaseTest(MongoContainer mongoContainer)
    {
        this.mongoContainer = mongoContainer;
        this.loggerMock = new Mock<ILogger<ClientRepository>>();
        this.options = Microsoft.Extensions.Options.Options.Create(OptionsUtil.GetOptions(this.mongoContainer.Port));
    }
    [Fact]
    public async Task ChangeStateAsync_WhenEntityIsInvalid_ReturnFalse()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        var (client, collection) = GetCollection();
        var serviceProvider = GetServiceProvider(client, collection);
        var guid = Guid.NewGuid();

        var repository = new ClientRepository(serviceProvider, this.options, loggerMock.Object);

        // Act
        await repository.ChangeStateAsync<Client>(guid, false, cancellationToken);

        // Assert
        var result = await collection.Find(x => x.Id == guid).FirstOrDefaultAsync(cancellationToken);

        Assert.Null(result);
    }

    [Fact]
    public async Task ChangeStateAsync_WhenEntityIsValid_ReturnTrue()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        var (client, collection) = GetCollection();
        var serviceProvider = GetServiceProvider(client, collection);

        var repository = new ClientRepository(serviceProvider, this.options, loggerMock.Object);

        var entity = new Client()
        {
            Id = Guid.NewGuid(),
            Name = "Test",
            IsActive = true
        };

        await repository.CreateAsync(entity, cancellationToken);

        // Act
        await repository.ChangeStateAsync<Client>(entity.Id, false, cancellationToken);

        var result = await collection.Find(x => x.Id == entity.Id).FirstOrDefaultAsync(cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(entity.Id, result.Id);
        Assert.Equal(entity.Name, result.Name);
        Assert.NotEqual(entity.IsActive, result.IsActive);
        Assert.Equal(entity.CreatedAt, result.CreatedAt);
        Assert.False(result.IsActive);
    }

    [Fact]
    public async Task CreateAsync_WhenEntityIsValid_ReturnEntityCreated()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        var (client, collection) = GetCollection();
        var serviceProvider = GetServiceProvider(client, collection);

        var repository = new ClientRepository(serviceProvider, this.options, loggerMock.Object);

        var entity = new Client()
        {
            Id = Guid.NewGuid(),
            Name = "Test",
            IsActive = true
        };

        // Act
        await repository.CreateAsync(entity, cancellationToken);

        var result = await collection.Find(x => x.Id == entity.Id).FirstOrDefaultAsync(cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(entity.Id, result.Id);
        Assert.Equal(entity.Name, result.Name);
        Assert.Equal(entity.IsActive, result.IsActive);
        Assert.Equal(entity.CreatedAt, result.CreatedAt);
    }

    [Fact]
    public async Task CreateRangeAsync_WhenEntityIsValid_ReturnEntityCreated()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        var (client, collection) = GetCollection();
        var serviceProvider = GetServiceProvider(client, collection);

        var repository = new ClientRepository(serviceProvider, this.options, loggerMock.Object);

        var entities = new List<Client>()
        {
            new ()
            {
                Id = Guid.NewGuid(),
                Name = "Test",
                IsActive = true
            },
            new ()
            {
                Id = Guid.NewGuid(),
                Name = "Test",
                IsActive = true
            }
        };

        // Act
        await repository.CreateRangeAsync(entities, cancellationToken);

        // Assert
        foreach (var entity in entities)
        {
            var result = await collection.Find(x => x.Id == entity.Id).FirstOrDefaultAsync(cancellationToken);

            Assert.NotNull(result);
            Assert.Equal(entity.Id, result.Id);
            Assert.Equal(entity.Name, result.Name);
            Assert.Equal(entity.IsActive, result.IsActive);
            Assert.Equal(entity.CreatedAt, result.CreatedAt);
        }
    }

    [Fact]
    public async Task DeleteAsync_WhenEntityIsValid_ReturnTrue()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        var (client, collection) = GetCollection();
        var serviceProvider = GetServiceProvider(client, collection);

        var repository = new ClientRepository(serviceProvider, this.options, loggerMock.Object);

        var entity = new Client()
        {
            Id = Guid.NewGuid(),
            Name = "Test",
            IsActive = true
        };

        await repository.CreateAsync(entity, cancellationToken);

        // Act
        var filter = Builders<Client>.Filter.Eq(x => x.Id, entity.Id);
        await repository.DeleteAsync(filter, cancellationToken);

        var result = await collection.Find(x => x.Id == entity.Id).FirstOrDefaultAsync(cancellationToken);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteRangeAsync_WhenEntityIsValid_ReturnTrue()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        var (client, collection) = GetCollection();
        var serviceProvider = GetServiceProvider(client, collection);

        var repository = new ClientRepository(serviceProvider, this.options, loggerMock.Object);

        var entities = new List<Client>()
        {
            new ()
            {
                Id = Guid.NewGuid(),
                Name = "Test",
                IsActive = true
            },
            new ()
            {
                Id = Guid.NewGuid(),
                Name = "Test",
                IsActive = true
            }
        };

        await repository.CreateRangeAsync(entities, cancellationToken);

        // Act
        await repository.DeleteRangeAsync(entities, cancellationToken);

        // Assert
        foreach (var entity in entities)
        {
            var result = await collection.Find(x => x.Id == entity.Id).FirstOrDefaultAsync(cancellationToken);

            Assert.Null(result);
        }
    }

    [Fact]
    public async Task UpdateAsync_WhenEntityIsValid_ReturnTrue()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        var (client, collection) = GetCollection();
        var serviceProvider = GetServiceProvider(client, collection);

        var repository = new ClientRepository(serviceProvider, this.options, loggerMock.Object);

        var entity = new Client()
        {
            Id = Guid.NewGuid(),
            Name = "Test",
            IsActive = true
        };

        await repository.CreateAsync(entity, cancellationToken);

        // Act
        entity.Name = "Test 2";
        await repository.UpdateAsync(entity, cancellationToken);

        var result = await collection.Find(x => x.Id == entity.Id).FirstOrDefaultAsync(cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(entity.Id, result.Id);
        Assert.Equal(entity.Name, result.Name);
        Assert.Equal(entity.IsActive, result.IsActive);
        Assert.Equal(entity.CreatedAt, result.CreatedAt);
    }

    [Fact]
    public async Task UpdateRangeAsync_WhenEntityIsValid_ReturnTrue()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        var (client, collection) = GetCollection();
        var serviceProvider = GetServiceProvider(client, collection);

        var repository = new ClientRepository(serviceProvider, this.options, loggerMock.Object);

        var entities = new List<Client>()
        {
            new ()
            {
                Id = Guid.NewGuid(),
                Name = "Test",
                IsActive = true
            },
            new ()
            {
                Id = Guid.NewGuid(),
                Name = "Test",
                IsActive = true
            }
        };

        await repository.CreateRangeAsync(entities, cancellationToken);

        // Act
        foreach (var entity in entities)
        {
            entity.Name = "Test 2";
        }

        await repository.UpdateRangeAsync(entities, cancellationToken);

        // Assert
        foreach (var entity in entities)
        {
            var result = await collection.Find(x => x.Id == entity.Id).FirstOrDefaultAsync(cancellationToken);

            Assert.NotNull(result);
            Assert.Equal(entity.Id, result.Id);
            Assert.Equal(entity.Name, result.Name);
            Assert.Equal(entity.IsActive, result.IsActive);
            Assert.Equal(entity.CreatedAt, result.CreatedAt);
        }
    }

    [Fact]
    public async Task TransactionAsync_WhenEntityIsValid_CommitSuccess()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        var (client, collection) = GetCollection();
        var serviceProvider = GetServiceProvider(client, collection);

        var repository = new ClientRepository(serviceProvider, this.options, loggerMock.Object);

        var entity = new Client()
        {
            Id = Guid.NewGuid(),
            Name = "Test",
            IsActive = true
        };

        // Act
        await repository.TransactionAsync(async (database, session) =>
        {
            var clientCollection = database.GetCollection<Client>(typeof(Client).Name);

            await clientCollection.InsertOneAsync(session, entity, cancellationToken: cancellationToken);

        }, cancellationToken);


        // Assert
        var result = await collection.Find(x => x.Id == entity.Id).FirstOrDefaultAsync(cancellationToken);

        Assert.NotNull(result);
        Assert.Equal(entity.Id, result.Id);
        Assert.Equal(entity.Name, result.Name);
        Assert.Equal(entity.IsActive, result.IsActive);
        Assert.Equal(entity.CreatedAt, result.CreatedAt);
    }

    [Fact]
    public async Task TransactionAsync_ThrowException_CommitFailed()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        var (client, collection) = GetCollection();
        var serviceProvider = GetServiceProvider(client, collection);

        var repository = new ClientRepository(serviceProvider, this.options, loggerMock.Object);

        var entity = new Client()
        {
            Id = Guid.NewGuid(),
            Name = "Test",
            IsActive = true
        };

        // Act
        await repository.TransactionAsync(async (database, session) =>
        {
            var clientCollection = database.GetCollection<Client>(typeof(Client).Name);

            await clientCollection.InsertOneAsync(session, entity, cancellationToken: cancellationToken);

            throw new Exception("Custom Message");
        }, cancellationToken);


        // Assert
        var result = await collection.Find(x => x.Id == entity.Id).FirstOrDefaultAsync(cancellationToken);

        Assert.Null(result);
        this.loggerMock.VerifyLogging("Failed to execute transaction", LogLevel.Error);
    }

    private static ServiceProvider GetServiceProvider(IMongoClient mongoClient, IMongoCollection<Client> collection)
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddSingleton(mongoClient);
        serviceCollection.AddSingleton(collection);
        var serviceProvider = serviceCollection.BuildServiceProvider();
        return serviceProvider;
    }

    private (IMongoClient, IMongoCollection<Client>) GetCollection()
    {
        var connectionString = OptionsUtil.GetOptions(this.mongoContainer.Port).ConnectionString;

        var client = new MongoClient(connectionString);
        var database = client.GetDatabase("dbtestmongo");
        var collection = database.GetCollection<Client>(typeof(Client).Name);
        return (client, collection);
    }
}