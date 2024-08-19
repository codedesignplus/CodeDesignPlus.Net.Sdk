namespace CodeDesignPlus.Net.xUnit.Helpers.OpenTelemetry;

/// <summary>
/// Represents a Docker container for OpenTelemetry Collector, managed using Docker Compose.
/// </summary>
public class OpenTelemetryContainer : DockerCompose
{
    /// <summary>
    /// Builds the Docker Compose service configuration for the OpenTelemetry Collector container.
    /// </summary>
    /// <returns>An <see cref="ICompositeService"/> representing the Docker Compose service.</returns>
    protected override ICompositeService Build()
    {
        // Define the path to the Docker Compose file.
        var file = Path.Combine(Directory.GetCurrentDirectory(), "Helpers", "OpenTelemetry", "docker-compose.yml");

        // Configure the Docker Compose settings.
        var dockerCompose = new DockerComposeConfig
        {
            ComposeFilePath = new[] { file },
            ForceRecreate = true,
            RemoveOrphans = true,
            StopOnDispose = true,
            AlternativeServiceName = "otel_" + Guid.NewGuid().ToString("N"),
        };

        // Enable port retrieval and set the internal port and container name.
        this.EnableGetPort = true;
        this.InternalPort = 4317;
        this.ContainerName = $"{dockerCompose.AlternativeServiceName}-otel-collector";

        // Create and return the Docker Compose service.
        var compose = new DockerComposeCompositeService(base.DockerHost, dockerCompose);

        return compose;
    }
}