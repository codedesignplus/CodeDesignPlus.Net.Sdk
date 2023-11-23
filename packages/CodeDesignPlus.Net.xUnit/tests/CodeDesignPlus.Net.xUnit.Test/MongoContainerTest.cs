using CodeDesignPlus.Net.xUnit.Helpers.MongoContainer;
using CodeDesignPlus.Net.xUnit.Test.Helpers;
using MongoDB.Driver;

namespace CodeDesignPlus.Net.xUnit.Test;

public class MongoContainerTest : IClassFixture<MongoContainer>
{
    private readonly MongoContainer mongoContainer;

    public MongoContainerTest(MongoContainer container)
    {
        this.mongoContainer = container;
    }

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

        var result = collection.Find(e => e.Id == clientEntity.Id).FirstOrDefault();

        Assert.NotNull(result);
        Assert.Equal(clientEntity.Id, result.Id);
        Assert.Equal(clientEntity.Name, result.Name);
    }
}
