using CodeDesignPlus.Net.xUnit.Containers.MongoContainer;
using CodeDesignPlus.Net.xUnit.Test.Helpers;
using MongoDB.Driver;

namespace CodeDesignPlus.Net.xUnit.Test;

[Collection(MongoCollectionFixture.Collection)]
public class MongoContainerTest(MongoCollectionFixture mongoCollectionFixture)
{
    private readonly MongoContainer mongoContainer = mongoCollectionFixture.Container;

    [Fact]
    public async Task CheckConnectionServer()
    {
        var host = "localhost";
        var port = this.mongoContainer.Port;
        var databaseName = "dbtestmongo";

        var connectionString = $"mongodb://{host}:{port}";

        var client = new MongoClient(connectionString);
        var database = client.GetDatabase(databaseName);

        var clientEntity = new Client()
        {
            Id = Guid.NewGuid(),
            Name = "Test"
        };

        var collection = database.GetCollection<Client>(typeof(Client).Name);

        await collection.InsertOneAsync(clientEntity);

        var result = await collection.Find(e => e.Id == clientEntity.Id).FirstOrDefaultAsync();

        Assert.NotNull(result);
        Assert.Equal(clientEntity.Id, result.Id);
        Assert.Equal(clientEntity.Name, result.Name);
    }
}
