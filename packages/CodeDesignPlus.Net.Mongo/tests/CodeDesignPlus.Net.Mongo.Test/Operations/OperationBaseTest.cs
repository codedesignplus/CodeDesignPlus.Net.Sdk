﻿using CodeDesignPlus.Net.Mongo.Test.Helpers.Models;
using CodeDesignPlus.Net.Security.Abstractions;
using CodeDesignPlus.Net.xUnit.Helpers.MongoContainer;
using MongoDB.Driver;
using Moq;

namespace CodeDesignPlus.Net.Mongo.Test.Operations;

public class OperationBaseTest : IClassFixture<MongoContainer>
{
    private readonly Mock<ILogger<ProductRepository>> loggerMock;
    private readonly MongoContainer mongoContainer;
    private readonly IOptions<MongoOptions> options;

    public OperationBaseTest(MongoContainer mongoContainer)
    {
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
        var date = DateTime.UtcNow;
        var product = new Product
        {
            IsActive = true,
            Id = Guid.NewGuid(),
            CreatedAt = date,
            IdUserCreator = idUser
        };

        var userContextMock = new Mock<IUserContext<Guid>>();

        userContextMock.SetupGet(x => x.IdUser).Returns(idUser);

        var repository = new ProductRepository(userContextMock.Object, serviceProvider, this.options, this.loggerMock.Object);

        // Act
        _ = await repository.CreateAsync(product);

        // Assert
        var result = await collection.Find(x => x.Id == product.Id).FirstOrDefaultAsync(cancellationToken);

        Assert.NotNull(result);
        Assert.Equal(product.Id, result.Id);
        Assert.Equal(product.IsActive, result.IsActive);
        Assert.True(date < result.CreatedAt);
        Assert.Equal(idUser, result.IdUserCreator);
    }

    [Fact]
    public async Task DeleteAsync_WhenEntityIsValid_ThenReturnTrue()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        var (client, collection) = GetCollection();
        var serviceProvider = GetServiceProvider(client, collection);

        var idUser = Guid.NewGuid();
        var date = DateTime.UtcNow;
        var product = new Product
        {
            IsActive = true,
            Id = Guid.NewGuid(),
            CreatedAt = date,
            IdUserCreator = idUser
        };

        var userContextMock = new Mock<IUserContext<Guid>>();

        userContextMock.SetupGet(x => x.IdUser).Returns(idUser);

        var repository = new ProductRepository(userContextMock.Object, serviceProvider, this.options, this.loggerMock.Object);

        _ = await repository.CreateAsync(product);

        // Act
        var isSuccess = await repository.DeleteAsync(product.Id);

        // Assert
        var result = await collection.Find(x => x.Id == product.Id).FirstOrDefaultAsync(cancellationToken);

        Assert.True(isSuccess);
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
        var date = DateTime.UtcNow;
        var product = new Product
        {
            IsActive = true,
            Id = Guid.NewGuid(),
            CreatedAt = date,
            IdUserCreator = idUser
        };

        var userContextMock = new Mock<IUserContext<Guid>>();

        userContextMock.SetupGet(x => x.IdUser).Returns(idUser);

        var repository = new ProductRepository(userContextMock.Object, serviceProvider, this.options, this.loggerMock.Object);

        _ = await repository.CreateAsync(product);

        // Act
        product.IsActive = false;
        var isSuccess = await repository.UpdateAsync(product.Id, product);

        // Assert
        var result = await collection.Find(x => x.Id == product.Id).FirstOrDefaultAsync(cancellationToken);

        Assert.True(isSuccess);
        Assert.NotNull(result);
        Assert.Equal(product.Id, result.Id);
        Assert.Equal(product.IsActive, result.IsActive);
        Assert.True(date < result.CreatedAt);
        Assert.Equal(product.IdUserCreator, result.IdUserCreator);
    }

    private static ServiceProvider GetServiceProvider(IMongoClient mongoClient, IMongoCollection<Product> collection)
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddSingleton(mongoClient);
        serviceCollection.AddSingleton(collection);
        var serviceProvider = serviceCollection.BuildServiceProvider();
        return serviceProvider;
    }

    private (IMongoClient, IMongoCollection<Product>) GetCollection()
    {
        var connectionString = OptionsUtil.GetOptions(this.mongoContainer.Port).ConnectionString;

        var client = new MongoClient(connectionString);
        var database = client.GetDatabase("dbtestmongo");
        var collection = database.GetCollection<Product>(typeof(Product).Name);
        return (client, collection);
    }
}