using CodeDesignPlus.Net.xUnit.Containers.RedisContainer;

namespace CodeDesignPlus.Net.xUnit.Test;


[Collection(RedisCollectionFixture.Collection)]
public class RedisContainerTest(RedisCollectionFixture fixture)
{
    private readonly RedisContainer redisContainer = fixture.Container;

    [Fact]
    public void CheckConnectionServer()
    {
        var server = this.redisContainer.RedisServer;

        Assert.True(server.IsConnected);
    }
}
