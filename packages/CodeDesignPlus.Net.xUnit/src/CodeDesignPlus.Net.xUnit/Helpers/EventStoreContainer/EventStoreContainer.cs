namespace CodeDesignPlus.Net.xUnit.Helpers.EventStoreContainer;

/// <summary>
/// Represents a Docker container for EventStore, managed using Docker Compose.
/// </summary>
public class EventStoreContainer : DockerCompose
{
    /// <summary>
    /// Builds the Docker Compose service configuration for the EventStore container.
    /// </summary>
    /// <returns>An <see cref="ICompositeService"/> representing the Docker Compose service.</returns>
    protected override ICompositeService Build()
    {
        // Set the internal port for the EventStore container.
        this.InternalPort = 1113;

        // Define the path to the Docker Compose file.
        var file = Path.Combine(Directory.GetCurrentDirectory(), "Helpers", "EventStoreContainer", (TemplateString)"docker-compose.yml");

        // Configure the Docker Compose settings.
        var dockerCompose = new DockerComposeConfig
        {
            ComposeFilePath = new[] { file },
            ForceRecreate = true,
            RemoveOrphans = true,
            StopOnDispose = true,
            AlternativeServiceName = "eventstore_" + Guid.NewGuid().ToString("N"),
        };

        // Enable port retrieval and set the container name.
        this.EnableGetPort = true;
        this.ContainerName = $"{dockerCompose.AlternativeServiceName}-eventstore.db";

        // Create and return the Docker Compose service.
        var compose = new DockerComposeCompositeService(base.DockerHost, dockerCompose);

        return compose;
    }
}