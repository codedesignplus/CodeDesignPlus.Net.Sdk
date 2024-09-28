using Ductus.FluentDocker.Model.Compose;
using Ductus.FluentDocker.Services;
using Ductus.FluentDocker.Services.Extensions;
using Ductus.FluentDocker.Services.Impl;

namespace CodeDesignPlus.Net.xUnit.Microservice.Server.Services;

/// <summary>
/// Represents a Docker Compose service.
/// </summary>
public class ServerCompose : DockerCompose
{
    private readonly string containerName = "server_" + Guid.NewGuid().ToString("N");

    public (string, int) Redis;
    public (string, int) RabbitMQ;
    public (string, int) Mongo;
    public (string, int) Otel;

    /// <summary>
    /// Initializes a new instance of the <see cref="ServerCompose"/> class.
    /// </summary>
    /// <returns>A new instance of the <see cref="ServerCompose"/> class.</returns>
    protected override ICompositeService Build()
    {
        var file = Path.Combine(Directory.GetCurrentDirectory(), "Server", "Services", "docker-compose.yml");

        var dockerCompose = new DockerComposeConfig
        {
            ComposeFilePath = [file],
            ForceRecreate = true,
            RemoveOrphans = true,
            StopOnDispose = true,
            AlternativeServiceName = containerName,
        };

        var compose = new DockerComposeCompositeService(DockerHost, dockerCompose);

        return compose;
    }

    /// <summary>
    /// Called when the Docker container is initialized.
    /// </summary>
    protected override void OnContainerInitialized()
    {
        // Get the port for the service.
        Redis = GetPort("redis", 6379);
        RabbitMQ = GetPort("rabbitmq", 5672);
        Mongo = GetPort("mongo", 27017);
        Otel = GetPort("otel-collector", 4317);

    }

    /// <summary>
    /// Get the host and port for the service.
    /// </summary>
    /// <param name="service">The name of the service.</param>
    /// <param name="InternalPort">The internal port of the service.</param>
    /// <returns>A tuple containing the host and port for the service.</returns>
    /// <exception cref="XunitException">The container was not found.</exception>
    public (string, int) GetPort(string service, int InternalPort)
    {
        var name = $"{containerName}-{service}";
        var container = CompositeService.Containers.FirstOrDefault(x => x.Name.StartsWith(name));

        if (container == null)
            throw new Exception($"The container {name} was not found.");

        var endpoint = container.ToHostExposedEndpoint($"{InternalPort}/tcp");

        return ("localhost", endpoint.Port);
    }

}
