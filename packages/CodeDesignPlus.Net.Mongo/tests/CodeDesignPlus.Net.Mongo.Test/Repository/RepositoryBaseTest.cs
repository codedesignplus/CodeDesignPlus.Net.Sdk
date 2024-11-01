using CodeDesignPlus.Net.Mongo.Repository;
using CodeDesignPlus.Net.Mongo.Test.Helpers.Models;
using CodeDesignPlus.Net.xUnit.Helpers;
using CodeDesignPlus.Net.xUnit.Helpers.MongoContainer;
using MongoDB.Driver;
using Moq;
using System.Linq.Expressions;
using System.Reflection;

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

    [Fact]
    public async Task FindAsync_WhenEntityIsValid_ReturnEntityFound()
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
        var result = await repository.FindAsync<Client>(entity.Id, cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(entity.Id, result.Id);
        Assert.Equal(entity.Name, result.Name);
        Assert.Equal(entity.IsActive, result.IsActive);
        Assert.Equal(entity.CreatedAt, result.CreatedAt);
    }

    [Fact]
    public async Task MatchingAsync_CheckFiltersWithSubDocuments_Success()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        var loggerOrderMock = new Mock<ILogger<OrderRepository>>();
        var (client, collection) = GetCollection();
        var serviceProvider = GetServiceProvider(client, collection);

        var criteria = new Core.Abstractions.Models.Criteria.Criteria
        {
            Filters = "name=Order 1|and|description~=Order|and|client.name^=Client"
        };

        var repository = new OrderRepository(serviceProvider, this.options, loggerOrderMock.Object);

        var orders = OrderData.GetOrders();

        await repository.CreateRangeAsync(orders, cancellationToken);

        // Act
        var result = await repository.MatchingAsync<Order>(criteria, cancellationToken);

        // Assert
        var order = orders.FirstOrDefault(x => x.Name == "Order 1");

        Assert.NotNull(order);
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.Contains(result, x => x.Id == order.Id);
    }

    [Fact]
    public async Task MatchingAsync_CheckFiltersWithProjection_Success()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        var loggerOrderMock = new Mock<ILogger<OrderRepository>>();
        var (client, collection) = GetCollection();
        var serviceProvider = GetServiceProvider(client, collection);

        var criteria = new Core.Abstractions.Models.Criteria.Criteria
        {
            Filters = "name=Order 1|and|description~=Order|and|client.name^=Client"
        };

        var repository = new OrderRepository(serviceProvider, this.options, loggerOrderMock.Object);

        var orders = OrderData.GetOrders();

        await repository.CreateRangeAsync(orders, cancellationToken);

        // Act
        var result = await repository.MatchingAsync<Order, Order>(criteria, x => new Order()
        {
            Id = x.Id,
            Name = x.Name,
            Description = x.Description,
        }, cancellationToken);

        // Assert
        var order = orders.First(x => x.Name == "Order 1");
        Assert.NotNull(order);
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.Contains(result, x => x.Id == order.Id);
    }

    [Fact]
    public async Task MatchingAsync_CheckFiltersWithProjection_SortAsc()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        var loggerOrderMock = new Mock<ILogger<OrderRepository>>();
        var (client, collection) = GetCollection();
        var serviceProvider = GetServiceProvider(client, collection);

        var criteria = new Core.Abstractions.Models.Criteria.Criteria
        {
            Filters = "name^=Order",
            OrderType = Net.Core.Abstractions.Models.Criteria.OrderTypes.Ascending,
            OrderBy = "Total"
        };

        var repository = new OrderRepository(serviceProvider, this.options, loggerOrderMock.Object);

        var orders = OrderData.GetOrders();

        await repository.CreateRangeAsync(orders, cancellationToken);

        // Act
        var data = await repository.MatchingAsync<Order, Order>(criteria, x => new Order()
        {
            Id = x.Id,
            Name = x.Name,
            Description = x.Description,
            Total = x.Total
        }, cancellationToken);

        // Assert
        var result = data.First();
        var order = orders.First();
        Assert.NotNull(order);
        Assert.NotNull(result);
        Assert.Equal(result.Id, order.Id);
    }

    [Fact]
    public async Task MatchingAsync_CheckFiltersWithProjection_SortDesc()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        var loggerOrderMock = new Mock<ILogger<OrderRepository>>();
        var (client, collection) = GetCollection();
        var serviceProvider = GetServiceProvider(client, collection);

        var criteria = new Core.Abstractions.Models.Criteria.Criteria
        {
            Filters = "name^=Order",
            OrderType = Net.Core.Abstractions.Models.Criteria.OrderTypes.Descending,
            OrderBy = "Total"
        };

        var repository = new OrderRepository(serviceProvider, this.options, loggerOrderMock.Object);

        var orders = OrderData.GetOrders();

        await repository.CreateRangeAsync(orders, cancellationToken);

        // Act
        var data = await repository.MatchingAsync<Order, Order>(criteria, x => new Order()
        {
            Id = x.Id,
            Name = x.Name,
            Description = x.Description,
            Total = x.Total
        }, cancellationToken);

        // Assert
        var result = data.First();
        var order = orders.Last();
        Assert.NotNull(order);
        Assert.NotNull(result);
        Assert.Equal(result.Name, order.Name);
    }


    [Fact]
    public async Task MatchingAsync_CheckFiltersWithSubCollections_Success()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        var loggerOrderMock = new Mock<ILogger<OrderRepository>>();
        var (client, collection) = GetCollection();
        var serviceProvider = GetServiceProvider(client, collection);

        var criteria = new Core.Abstractions.Models.Criteria.Criteria
        {
            Filters = "name=Product 1"
        };

        var repository = new OrderRepository(serviceProvider, this.options, loggerOrderMock.Object);

        var orders = OrderData.GetOrders();
        var order = orders.First(x => x.Name == "Order 1");
        var product = order.Products.First(x => x.Name == "Product 1");

        await repository.CreateRangeAsync(orders, cancellationToken);

        // Act
        var result = await repository.MatchingAsync<Order, Product>(order.Id, criteria, x => x.Products, cancellationToken);

        // Assert
        Assert.NotNull(order);
        Assert.NotNull(product);
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.Contains(result, x => x.Id == product.Id);
    }

    [Fact]
    public void GetPropertyName_InvalidExpression_ThrowsMongoException()
    {
        // Arrange
        var repositoryType = typeof(RepositoryBase);
        var methodInfo = repositoryType
            .GetMethods(BindingFlags.NonPublic | BindingFlags.Static)
            .FirstOrDefault(m => m.Name == "GetPropertyName" && m.IsGenericMethodDefinition);

        if (methodInfo == null)
            throw new InvalidOperationException("No se encontró el método 'GetPropertyName'.");

        // Crear una expresión inválida (una constante en lugar de un MemberExpression)
        Expression<Func<Order, bool>> invalidExpression = x => x.Name == "Test" && x.Description == "Test";

        // Crear un método cerrado (closed method) a partir del método genérico
        var closedMethod = methodInfo.MakeGenericMethod(typeof(Order), typeof(bool));

        // Act
        var exception = Record.Exception(() => closedMethod.Invoke(null, [invalidExpression]));

        // Assert
        Assert.NotNull(exception);
        Assert.IsType<TargetInvocationException>(exception);
        Assert.IsType<Mongo.Exceptions.MongoException>(exception.InnerException);
        Assert.Equal("The expression must be a MemberExpression.", exception.InnerException.Message);
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