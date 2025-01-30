namespace CodeDesignPlus.Net.xUnit.Containers.MongoContainer;

/// <summary>
/// Represents a Docker container for MongoDB, managed using Docker Compose.
/// </summary>
public class MongoContainer : DockerCompose
{
    /// <summary>
    /// Builds the Docker Compose service configuration for the MongoDB container.
    /// </summary>
    /// <returns>An <see cref="ICompositeService"/> representing the Docker Compose service.</returns>
    protected override ICompositeService Build()
    {
        var port = new Random().Next(27017, 28000);
        // Define the path to the Docker Compose file.
        var file = Path.Combine(Directory.GetCurrentDirectory(), "Containers", "MongoContainer", "docker-compose.yml");

        // Configure the Docker Compose settings.
        var dockerCompose = new DockerComposeConfig
        {
            ComposeFilePath = new[] { file },
            ForceRecreate = true,
            RemoveOrphans = true,
            StopOnDispose = true,
            AlternativeServiceName = "mongo_" + Guid.NewGuid().ToString("N"),
            EnvironmentNameValue = new Dictionary<string, string>
            {
                { "PORT_CUSTOM", port.ToString() },
            }
        };

        // Enable port retrieval and set the internal port and container name.
        this.EnableGetPort = true;
        this.InternalPort = port;
        this.ContainerName = $"{dockerCompose.AlternativeServiceName}-mongo";

        // Create and return the Docker Compose service.
        var compose = new DockerComposeCompositeService(base.DockerHost, dockerCompose);

        return compose;
    }
}