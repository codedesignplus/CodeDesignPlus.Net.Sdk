using CodeDesignPlus.Net.Redis.Abstractions;
using CodeDesignPlus.Net.Redis.Options;
using CodeDesignPlus.Net.Redis.Services;
using Ductus.FluentDocker.Model.Common;
using Ductus.FluentDocker.Model.Compose;
using Ductus.FluentDocker.Services;
using Ductus.FluentDocker.Services.Impl;
using Moq;
using O = Microsoft.Extensions.Options;

namespace CodeDesignPlus.Net.xUnit.Helpers.Server;

public class RedisContainer : DockerCompose
{
    public Mock<ILogger<RedisService>> Logger { get; private set; } = new();
    public RedisService RedisServer { get; private set; }
    public RedisService RedisServerWithoutPfxPassword { get; private set; }

    public RedisContainer()
    {
        this.RedisServerWithoutPfxPassword = this.StartSecondRedisServer();
        this.RedisServer = this.StartFirtsRedisServer();
    }

    private RedisService StartFirtsRedisServer()
    {
        var options = CreateOptions("client.pfx", "Temporal1");

        var redisService = new RedisService(this.Logger.Object);

        redisService.Initialize(options.Value.Instances["test"]);

        return redisService;
    }

    private RedisService StartSecondRedisServer()
    {
        var options = CreateOptions("client-without-pass.pfx");

        var redisService = new RedisService(this.Logger.Object);

        redisService.Initialize(options.Value.Instances["test"]);

        return redisService;
    }

    protected override ICompositeService Build()
    {
        var file = Path.Combine(Directory.GetCurrentDirectory(), "Helpers", "RedisContainer", (TemplateString)"docker-compose.standalone.yml");

        var compose = new DockerComposeCompositeService(
            base.DockerHost,
            new DockerComposeConfig
            {
                ComposeFilePath = new List<string> { file },
                ForceRecreate = true,
                RemoveOrphans = true,
                StopOnDispose = true
            });

        return compose;
    }


    private static IOptions<RedisOptions> CreateOptions(string certificate, string password = null)
    {
        var pfx = Path.Combine(Directory.GetCurrentDirectory(), "Helpers", "RedisContainer", "Certificates", certificate);

        if (!File.Exists(pfx))
            throw new InvalidOperationException("Can't run unit test because certificate does not exist");

        var instance = new Instance()
        {
            ConnectionString = "localhost:6380,ssl=true,password=12345678,resolveDns=false,sslprotocols=tls12|tls13",
            Certificate = pfx,
            PasswordCertificate = password,
        };

        if (!string.IsNullOrEmpty(password))
            instance.PasswordCertificate = password;

        var redisOptions = new RedisOptions();

        redisOptions.Instances.Add("test", instance);

        return O.Options.Create(redisOptions);
    }
}
