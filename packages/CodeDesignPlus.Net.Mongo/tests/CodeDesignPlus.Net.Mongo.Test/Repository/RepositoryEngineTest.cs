using System.Reflection;
using CodeDesignPlus.Net.Mongo.Repository;
using CodeDesignPlus.Net.Mongo.Test.Helpers.Models;
using CodeDesignPlus.Net.xUnit.Containers.MongoContainer;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using Moq;
using CR = CodeDesignPlus.Net.Core.Abstractions.Models.Criteria;

namespace CodeDesignPlus.Net.Mongo.Test.Repository;

/// <summary>
/// Kappali pendings/621: with the Azure DocumentDB engine every find is sorted; with MongoDB nothing changes.
/// </summary>
[Collection(MongoCollectionFixture.Collection)]
public class RepositoryEngineTest
{
    private readonly MongoContainer mongoContainer;

    private readonly IServiceProvider serviceProvider;

    public RepositoryEngineTest(MongoCollectionFixture fixture)
    {
        try
        {
            BsonSerializer.TryRegisterSerializer<Guid>(new GuidSerializer(GuidRepresentation.Standard));
        }
        catch { }

        this.mongoContainer = fixture.Container;

        var client = new MongoClient(OptionsUtil.GetOptions(this.mongoContainer.Port).ConnectionString);

        this.serviceProvider = new ServiceCollection().AddSingleton<IMongoClient>(client).BuildServiceProvider();
    }

    private ClientRepository CreateRepository(MongoEngine engine)
    {
        var options = OptionsUtil.GetOptions(this.mongoContainer.Port);

        options.Engine = engine;

        return new ClientRepository(this.serviceProvider, Microsoft.Extensions.Options.Options.Create(options), Mock.Of<ILogger<ClientRepository>>());
    }

    private static IFindFluent<Client, Client> Query(ClientRepository repository, CR.Criteria criteria)
    {
        var method = typeof(RepositoryBase).GetMethod("Query", BindingFlags.NonPublic | BindingFlags.Instance)!.MakeGenericMethod(typeof(Client));

        return (IFindFluent<Client, Client>)method.Invoke(repository, [criteria, Guid.Empty])!;
    }

    [Fact]
    public void GetCollection_AzureDocumentDb_ReturnsTheStableOrderCollection()
    {
        // Arrange
        var repository = this.CreateRepository(MongoEngine.AzureDocumentDb);

        // Act
        var collection = repository.GetCollection<Client>();

        // Assert
        Assert.IsType<StableOrderCollection<Client>>(collection);
    }

    [Fact]
    public void GetCollection_MongoDb_ReturnsTheDriverCollection()
    {
        // Arrange
        var repository = this.CreateRepository(MongoEngine.MongoDb);

        // Act
        var collection = repository.GetCollection<Client>();

        // Assert
        Assert.IsNotType<StableOrderCollection<Client>>(collection);
    }

    [Fact]
    public void Query_AzureDocumentDbWithoutSortBy_SortsById()
    {
        // Arrange
        var repository = this.CreateRepository(MongoEngine.AzureDocumentDb);

        // Act
        var query = Query(repository, new CR.Criteria());

        // Assert
        Assert.Equal(new BsonDocument("_id", 1), StableOrderCollectionTest.Render(query.Options.Sort));
    }

    [Fact]
    public void Query_AzureDocumentDbWithSortBy_AddsIdAsTieBreaker()
    {
        // Arrange
        var repository = this.CreateRepository(MongoEngine.AzureDocumentDb);
        var criteria = new CR.Criteria { OrderBy = "Name", OrderType = CR.OrderTypes.Descending };

        // Act
        var query = Query(repository, criteria);

        // Assert
        Assert.Equal(new BsonDocument { { "Name", -1 }, { "_id", 1 } }, StableOrderCollectionTest.Render(query.Options.Sort));
    }

    [Fact]
    public void Query_AzureDocumentDbSortedById_KeepsTheRequestedDirection()
    {
        // Arrange
        var repository = this.CreateRepository(MongoEngine.AzureDocumentDb);
        var criteria = new CR.Criteria { OrderBy = "Id", OrderType = CR.OrderTypes.Descending };

        // Act
        var query = Query(repository, criteria);

        // Assert
        Assert.Equal(new BsonDocument("_id", -1), StableOrderCollectionTest.Render(query.Options.Sort));
    }

    [Fact]
    public void Query_MongoDbWithoutSortBy_HasNoSort()
    {
        // Arrange
        var repository = this.CreateRepository(MongoEngine.MongoDb);

        // Act
        var query = Query(repository, new CR.Criteria());

        // Assert
        Assert.Null(query.Options.Sort);
    }

    [Fact]
    public void Query_MongoDbWithSortBy_SortsOnlyByTheRequestedField()
    {
        // Arrange
        var repository = this.CreateRepository(MongoEngine.MongoDb);
        var criteria = new CR.Criteria { OrderBy = "Name", OrderType = CR.OrderTypes.Ascending };

        // Act
        var query = Query(repository, criteria);

        // Assert
        Assert.Equal(new BsonDocument("Name", 1), StableOrderCollectionTest.Render(query.Options.Sort));
    }

    [Fact]
    public async Task Find_AzureDocumentDbWithoutSort_ReturnsDocumentsInIdOrder()
    {
        // Arrange
        var repository = this.CreateRepository(MongoEngine.AzureDocumentDb);
        var name = Guid.NewGuid().ToString();
        var prefix = Guid.NewGuid().ToString("N")[..8];
        var ids = new[] { "3", "2", "1" }.Select(x => Guid.Parse($"{prefix}-0000-0000-0000-00000000000{x}")).ToList();

        // Inserted in descending order: the natural order of MongoDB returns them as they were inserted.
        await repository.CreateRangeAsync(ids.Select(id => new Client { Id = id, Name = name, IsActive = true }).ToList(), CancellationToken.None);

        // Act
        var result = await repository.GetCollection<Client>().Find(x => x.Name == name).ToListAsync();

        // Assert
        Assert.Equal(ids.OrderBy(x => x.ToString()).ToList(), result.Select(x => x.Id).ToList());
    }

    [Fact]
    public async Task MatchingAsync_AzureDocumentDbSortedByName_BreaksTiesById()
    {
        // Arrange
        var repository = this.CreateRepository(MongoEngine.AzureDocumentDb);
        var name = Guid.NewGuid().ToString();
        var prefix = Guid.NewGuid().ToString("N")[..8];
        var ids = new[] { "3", "2", "1" }.Select(x => Guid.Parse($"{prefix}-0000-0000-0000-00000000000{x}")).ToList();

        await repository.CreateRangeAsync(ids.Select(id => new Client { Id = id, Name = name, IsActive = true }).ToList(), CancellationToken.None);

        var criteria = new CR.Criteria { Filters = $"name={name}", OrderBy = "Name", OrderType = CR.OrderTypes.Ascending };

        // Act
        var result = await repository.MatchingAsync<Client>(criteria, CancellationToken.None);

        // Assert
        Assert.Equal(ids.OrderBy(x => x.ToString()).ToList(), result.Data.Select(x => x.Id).ToList());
    }
}
