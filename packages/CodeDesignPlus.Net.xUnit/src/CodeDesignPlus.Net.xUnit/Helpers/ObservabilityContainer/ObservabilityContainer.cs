namespace CodeDesignPlus.Net.xUnit.Helpers.ObservabilityContainer;

/// <summary>
/// Represents a Docker container for Observability services, managed using Docker Compose.
/// </summary>
public class ObservabilityContainer : DockerCompose
{
    /// <summary>
    /// Builds the Docker Compose service configuration for the Observability container.
    /// </summary>
    /// <returns>An <see cref="ICompositeService"/> representing the Docker Compose service.</returns>
    protected override ICompositeService Build()
    {
        // Define the path to the Docker Compose file.
        var file = Path.Combine(Directory.GetCurrentDirectory(), "Helpers", "ObservabilityContainer", "docker-compose.yml");

        // Configure the Docker Compose settings.
        var dockerCompose = new DockerComposeConfig
        {
            ComposeFilePath = new[] { file },
            ForceRecreate = true,
            RemoveOrphans = true,
            StopOnDispose = true,
            AlternativeServiceName = "observability_" + Guid.NewGuid().ToString("N"),
        };

        // Create and return the Docker Compose service.
        var compose = new DockerComposeCompositeService(base.DockerHost, dockerCompose);

        return compose;
    }
}