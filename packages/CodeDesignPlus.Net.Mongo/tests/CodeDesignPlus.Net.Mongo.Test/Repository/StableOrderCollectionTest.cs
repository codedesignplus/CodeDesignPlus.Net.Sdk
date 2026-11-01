using CodeDesignPlus.Net.Mongo.Repository;
using CodeDesignPlus.Net.Mongo.Test.Helpers.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using Moq;

namespace CodeDesignPlus.Net.Mongo.Test.Repository;

public class StableOrderCollectionTest
{
    private static readonly FilterDefinition<Client> Filter = Builders<Client>.Filter.Empty;

    private readonly Mock<IMongoCollection<Client>> inner = new();

    private readonly StableOrderCollection<Client> collection;

    public StableOrderCollectionTest()
    {
        // Rendering a sort freezes the class map of Client, so the Guid representation must be registered first.
        try
        {
            BsonSerializer.TryRegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));
        }
        catch { }

        this.collection = new StableOrderCollection<Client>(this.inner.Object);
    }

    internal static BsonDocument? Render<T>(SortDefinition<T>? sort)
    {
        return sort?.Render(new RenderArgs<T>(BsonSerializer.LookupSerializer<T>(), BsonSerializer.SerializerRegistry));
    }

    [Fact]
    public async Task FindAsync_OptionsWithoutSort_SortsByIdAscending()
    {
        // Arrange
        FindOptions<Client, Client>? sent = null;

        this.inner
            .Setup(x => x.FindAsync(Filter, It.IsAny<FindOptions<Client, Client>>(), It.IsAny<CancellationToken>()))
            .Callback<FilterDefinition<Client>, FindOptions<Client, Client>, CancellationToken>((_, options, _) => sent = options)
            .ReturnsAsync(Mock.Of<IAsyncCursor<Client>>());

        // Act
        await this.collection.FindAsync(Filter, new FindOptions<Client, Client> { Limit = 5 }, CancellationToken.None);

        // Assert
        Assert.Equal(new BsonDocument("_id", 1), Render(sent!.Sort));
    }

    [Fact]
    public async Task FindAsync_NullOptions_SortsByIdAscending()
    {
        // Arrange
        FindOptions<Client, Client>? sent = null;

        this.inner
            .Setup(x => x.FindAsync(Filter, It.IsAny<FindOptions<Client, Client>>(), It.IsAny<CancellationToken>()))
            .Callback<FilterDefinition<Client>, FindOptions<Client, Client>, CancellationToken>((_, options, _) => sent = options)
            .ReturnsAsync(Mock.Of<IAsyncCursor<Client>>());

        // Act
        await this.collection.FindAsync<Client>(Filter, null, CancellationToken.None);

        // Assert
        Assert.Equal(new BsonDocument("_id", 1), Render(sent!.Sort));
    }

    [Fact]
    public async Task FindAsync_OptionsWithSort_KeepsTheCallerSort()
    {
        // Arrange
        FindOptions<Client, Client>? sent = null;

        this.inner
            .Setup(x => x.FindAsync(Filter, It.IsAny<FindOptions<Client, Client>>(), It.IsAny<CancellationToken>()))
            .Callback<FilterDefinition<Client>, FindOptions<Client, Client>, CancellationToken>((_, options, _) => sent = options)
            .ReturnsAsync(Mock.Of<IAsyncCursor<Client>>());

        var options = new FindOptions<Client, Client> { Sort = Builders<Client>.Sort.Descending(x => x.Name) };

        // Act
        await this.collection.FindAsync(Filter, options, CancellationToken.None);

        // Assert
        Assert.Equal(new BsonDocument("Name", -1), Render(sent!.Sort));
    }

    [Fact]
    public async Task FindAsync_WithSessionAndNoSort_SortsByIdAscending()
    {
        // Arrange
        var session = Mock.Of<IClientSessionHandle>();
        FindOptions<Client, Client>? sent = null;

        this.inner
            .Setup(x => x.FindAsync(session, Filter, It.IsAny<FindOptions<Client, Client>>(), It.IsAny<CancellationToken>()))
            .Callback<IClientSessionHandle, FilterDefinition<Client>, FindOptions<Client, Client>, CancellationToken>((_, _, options, _) => sent = options)
            .ReturnsAsync(Mock.Of<IAsyncCursor<Client>>());

        // Act
        await this.collection.FindAsync<Client>(session, Filter, null, CancellationToken.None);

        // Assert
        Assert.Equal(new BsonDocument("_id", 1), Render(sent!.Sort));
    }

    [Fact]
    public void FindSync_NoSort_SortsByIdAscending()
    {
        // Arrange
        FindOptions<Client, Client>? sent = null;

        this.inner
            .Setup(x => x.FindSync(Filter, It.IsAny<FindOptions<Client, Client>>(), It.IsAny<CancellationToken>()))
            .Callback<FilterDefinition<Client>, FindOptions<Client, Client>, CancellationToken>((_, options, _) => sent = options)
            .Returns(Mock.Of<IAsyncCursor<Client>>());

        // Act
        this.collection.FindSync<Client>(Filter, null, CancellationToken.None);

        // Assert
        Assert.Equal(new BsonDocument("_id", 1), Render(sent!.Sort));
    }

    [Fact]
    public void FindSync_WithSessionAndNoSort_SortsByIdAscending()
    {
        // Arrange
        var session = Mock.Of<IClientSessionHandle>();
        FindOptions<Client, Client>? sent = null;

        this.inner
            .Setup(x => x.FindSync(session, Filter, It.IsAny<FindOptions<Client, Client>>(), It.IsAny<CancellationToken>()))
            .Callback<IClientSessionHandle, FilterDefinition<Client>, FindOptions<Client, Client>, CancellationToken>((_, _, options, _) => sent = options)
            .Returns(Mock.Of<IAsyncCursor<Client>>());

        // Act
        this.collection.FindSync<Client>(session, Filter, null, CancellationToken.None);

        // Assert
        Assert.Equal(new BsonDocument("_id", 1), Render(sent!.Sort));
    }

    [Fact]
    public async Task InsertOneAsync_Document_DelegatesToTheInnerCollection()
    {
        // Arrange
        var document = new Client { Id = Guid.NewGuid() };
        var options = new InsertOneOptions();

        // Act
        await this.collection.InsertOneAsync(document, options, CancellationToken.None);

        // Assert
        this.inner.Verify(x => x.InsertOneAsync(document, options, CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task ReplaceOneAsync_Document_DelegatesToTheInnerCollection()
    {
        // Arrange
        var document = new Client { Id = Guid.NewGuid() };
        var options = new ReplaceOptions { IsUpsert = true };

        // Act
        await this.collection.ReplaceOneAsync(Filter, document, options, CancellationToken.None);

        // Assert
        this.inner.Verify(x => x.ReplaceOneAsync(Filter, document, options, CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task UpdateOneAsync_Update_DelegatesToTheInnerCollection()
    {
        // Arrange
        var update = Builders<Client>.Update.Set(x => x.Name, "Name");

        // Act
        await this.collection.UpdateOneAsync(Filter, update, null, CancellationToken.None);

        // Assert
        this.inner.Verify(x => x.UpdateOneAsync(Filter, update, null, CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task CountDocumentsAsync_Filter_ReturnsTheInnerCount()
    {
        // Arrange
        this.inner.Setup(x => x.CountDocumentsAsync(Filter, null, CancellationToken.None)).ReturnsAsync(42);

        // Act
        var count = await this.collection.CountDocumentsAsync(Filter, null, CancellationToken.None);

        // Assert
        Assert.Equal(42, count);
    }

    [Fact]
    public async Task AggregateAsync_Pipeline_ReachesTheInnerCollectionUntouched()
    {
        // Arrange
        var pipeline = new EmptyPipelineDefinition<Client>().Match(Filter);

        // Act
        await this.collection.AggregateAsync(pipeline, null, CancellationToken.None);

        // Assert
        this.inner.Verify(x => x.AggregateAsync(pipeline, null, CancellationToken.None), Times.Once);
    }

    [Fact]
    public void WithReadPreference_ReadPreference_KeepsTheStableOrder()
    {
        // Arrange
        this.inner.Setup(x => x.WithReadPreference(ReadPreference.Secondary)).Returns(Mock.Of<IMongoCollection<Client>>());

        // Act
        var result = this.collection.WithReadPreference(ReadPreference.Secondary);

        // Assert
        Assert.IsType<StableOrderCollection<Client>>(result);
    }
}
