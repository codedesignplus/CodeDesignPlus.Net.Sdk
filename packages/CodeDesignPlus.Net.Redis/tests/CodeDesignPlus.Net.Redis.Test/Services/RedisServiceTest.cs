using CodeDesignPlus.Net.Redis.Options;
using CodeDesignPlus.Net.Redis.Services;
using CodeDesignPlus.Net.Redis.Test.Helpers.Server;
using Ductus.FluentDocker.Model.Common;
using Microsoft.Extensions.Logging;
using Moq;
using O = Microsoft.Extensions.Options;

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
        var file = Path.Combine(Directory.GetCurrentDirectory(), "Helpers", "Server", "Certificates", "client.pfx");


    }


    /// <summary>
    /// Should return an <see cref="ArgumentNullException"/> when options is null
    /// </summary>
    [Fact]
    public void Constructor_OptionsIsNull_ArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => new RedisService(null, null));
    }

    /// <summary>
    /// Should return an <see cref="ArgumentNullException"/> when options is null
    /// </summary>
    [Fact]
    public void Constructor_LoggerIsNull_ArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => new RedisService(Mock.Of<IOptions<RedisOptions>>(), null));
    }

    /// <summary>
    /// Should connect with redis server and register events
    /// </summary>
    [Fact]
    public void Initialize_Connection_Success()
    {
        // Arrange
        var logger = Mock.Of<ILogger<RedisService>>();
        var options = CreateOptions("client.pfx", "Temporal1");

        // Act
        var redisService = new RedisService(options, logger);

        redisService.Initialize(options.Value.Instances.FirstOrDefault());

        // Assert
        Assert.True(redisService.IsConnected);
        Assert.NotNull(redisService.Database);
        Assert.NotNull(redisService.Subscriber);
    }

    private static IOptions<RedisOptions> CreateOptions(string certificate, string? password = null)
    {
        var pfx = Path.Combine(Directory.GetCurrentDirectory(),  "Helpers", "Server", "Certificates",certificate);

        if (!File.Exists(pfx))
            throw new InvalidOperationException("Can't run unit test because certificate does not exist");

        var instance = new Instance()
        {
            ConnectionString = "localhost:6380,ssl=true,password=12345678,sslHost=redis-client,resolveDns=false",
            Certificate = pfx,
            PasswordCertificate = password,
        };

        if (!string.IsNullOrEmpty(password))
            instance.PasswordCertificate = password;


        var redisOptions = new RedisOptions();

        redisOptions.Instances.Add(instance);


        return O.Options.Create(redisOptions);
    }
}
