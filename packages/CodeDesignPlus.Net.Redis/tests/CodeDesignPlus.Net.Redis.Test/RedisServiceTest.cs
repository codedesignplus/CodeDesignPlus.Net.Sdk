using CodeDesignPlus.Net.Redis.Options;
using CodeDesignPlus.Net.Redis.Services;
using CodeDesignPlus.Net.Redis.Test.Helpers.Server;
using Ductus.FluentDocker.Model.Common;
using Microsoft.Extensions.Logging;
using Moq;

namespace CodeDesignPlus.Net.Redis.Test;

public class RedisServiceTest : IClassFixture<Server>
{
    private readonly Server fixture;

    public RedisServiceTest(Server fixture)
    {
        this.fixture = fixture;
    }

    [Fact]
    public void Test()
    {
        var file = Path.Combine(Directory.GetCurrentDirectory(), "Helpers", "Server", (TemplateString)"docker-compose.yml");
    }
}
