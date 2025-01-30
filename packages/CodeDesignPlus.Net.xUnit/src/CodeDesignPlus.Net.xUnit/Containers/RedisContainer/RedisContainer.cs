using CodeDesignPlus.Net.Redis.Abstractions;
using CodeDesignPlus.Net.Redis.Options;
using CodeDesignPlus.Net.Redis.Services;

namespace CodeDesignPlus.Net.xUnit.Containers.RedisContainer;

/// <summary>
/// Represents a Docker container for Redis, managed using Docker Compose.
/// </summary>
public class RedisContainer : DockerCompose
{
    /// <summary>
    /// Gets the mock logger for the Redis service.
    /// </summary>
    public Mock<ILogger<RedisService>> Logger { get; private set; } = new();

    /// <summary>
    /// Gets the Redis service instance with a PFX password.
    /// </summary>
    public RedisService RedisServer { get; private set; }

    /// <summary>
    /// Gets the Redis service instance without a PFX password.
    /// </summary>
    public RedisService RedisServerWithoutPfxPassword { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="RedisContainer"/> class.
    /// </summary>
    public RedisContainer()
    {
        RedisServerWithoutPfxPassword = StartSecondRedisServer();
        RedisServer = StartFirstRedisServer();
    }

    /// <summary>
    /// Starts the first Redis server with a PFX password.
    /// </summary>
    /// <returns>A configured <see cref="RedisService"/> instance.</returns>
    private RedisService StartFirstRedisServer()
    {
        var options = CreateOptions("client.pfx", "Temporal1");

        var redisService = new RedisService(Logger.Object);

        redisService.Initialize(options.Value.Instances["test"]);

        return redisService;
    }

    /// <summary>
    /// Starts the second Redis server without a PFX password.
    /// </summary>
    /// <returns>A configured <see cref="RedisService"/> instance.</returns>
    private RedisService StartSecondRedisServer()
    {
        var options = CreateOptions("client-without-pass.pfx");

        var redisService = new RedisService(Logger.Object);

        redisService.Initialize(options.Value.Instances["test"]);

        return redisService;
    }

    /// <summary>
    /// Builds the Docker Compose service configuration for the Redis container.
    /// </summary>
    /// <returns>An <see cref="ICompositeService"/> representing the Docker Compose service.</returns>
    protected override ICompositeService Build()
    {
        var file = Path.Combine(Directory.GetCurrentDirectory(), "Containers", "RedisContainer", (TemplateString)"docker-compose.standalone.yml");

        var dockerCompose = new DockerComposeConfig
        {
            ComposeFilePath = [file],
            ForceRecreate = true,
            RemoveOrphans = true,
            StopOnDispose = true,
            AlternativeServiceName = "redis_" + Guid.NewGuid().ToString("N"),
        };

        EnableGetPort = true;
        InternalPort = 6380;
        ContainerName = $"{dockerCompose.AlternativeServiceName}-redis";

        var compose = new DockerComposeCompositeService(DockerHost, dockerCompose);

        return compose;
    }

    /// <summary>
    /// Creates Redis options for the specified certificate and password.
    /// </summary>
    /// <param name="certificate">The certificate file name.</param>
    /// <param name="password">The password for the certificate. Default is null.</param>
    /// <param name="instanceName">The name of the Redis instance. Default is "test".</param>
    /// <returns>A configured <see cref="RedisOptions"/> instance.</returns>
    public RedisOptions RedisOptions(string certificate, string password = null, string instanceName = "test")
    {
        var pfx = Path.Combine(Directory.GetCurrentDirectory(), "Containers", "RedisContainer", "Certificates", certificate);

        if (!File.Exists(pfx))
            throw new InvalidOperationException("Can't run unit test because certificate does not exist");

        var instance = new Instance()
        {
            ConnectionString = $"localhost:{Port},ssl=true,password=12345678,resolveDns=false,sslprotocols=tls12|tls13",
            Certificate = pfx,
            PasswordCertificate = password,
        };

        if (!string.IsNullOrEmpty(password))
            instance.PasswordCertificate = password;

        var redisOptions = new RedisOptions();

        redisOptions.Instances.Add(instanceName, instance);

        return redisOptions;
    }

    /// <summary>
    /// Creates options for the Redis service.
    /// </summary>
    /// <param name="certificate">The certificate file name.</param>
    /// <param name="password">The password for the certificate. Default is null.</param>
    /// <returns>A configured <see cref="IOptions{RedisOptions}"/> instance.</returns>
    private IOptions<RedisOptions> CreateOptions(string certificate, string password = null)
    {
        return Options.Create(RedisOptions(certificate, password));
    }
}