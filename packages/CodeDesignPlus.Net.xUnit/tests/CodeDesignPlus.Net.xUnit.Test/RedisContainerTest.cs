using CodeDesignPlus.Net.xUnit.Helpers.Server;

namespace CodeDesignPlus.Net.xUnit.Test;

public class RedisContainerTest : IClassFixture<RedisContainer>
{
    private readonly RedisContainer redisContainer;

    public RedisContainerTest(RedisContainer redisContainer)
    {
        this.redisContainer = redisContainer;
    }

    [Fact]
    public void CheckConnectionServer()
    {
        var server = this.redisContainer.RedisServer;

        Assert.True(server.IsConnected);
    }
}
