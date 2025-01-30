namespace CodeDesignPlus.Net.xUnit.Containers.KafkaContainer;

/// <summary>
/// Represents a Docker container for Kafka, managed using Docker Compose.
/// </summary>
public class KafkaContainer : DockerCompose
{
    /// <summary>
    /// Gets the broker list for the Kafka container.
    /// </summary>
    public string BrokerList { get; private set; }

    /// <summary>
    /// Builds the Docker Compose service configuration for the Kafka container.
    /// </summary>
    /// <returns>An <see cref="ICompositeService"/> representing the Docker Compose service.</returns>
    protected override ICompositeService Build()
    {
        // Define the path to the Docker Compose file.
        var file = Path.Combine(Directory.GetCurrentDirectory(), "Containers", "KafkaContainer", (TemplateString)"docker-compose.yml");

        // Generate a random port for the Kafka broker.
        var random = new Random();
        var hostPort = random.Next(29000, 29999);

        // Set the broker list for the Kafka container.
        this.BrokerList = $"localhost:{hostPort}";

        // Configure the Docker Compose settings.
        var compose = new DockerComposeConfig
        {
            ComposeFilePath = new[] { file },
            ForceRecreate = true,
            RemoveOrphans = true,
            StopOnDispose = true,
            AlternativeServiceName = "kafka_" + Guid.NewGuid().ToString("N"),
            EnvironmentNameValue = new Dictionary<string, string>
            {
                { "KAFKA_PORT" , random.Next(9000, 9999).ToString() },
                { "KAFKA_HOST_PORT", hostPort.ToString() }
            }
        };

        // Disable port retrieval and set the container name.
        this.EnableGetPort = false;
        this.ContainerName = $"{compose.AlternativeServiceName}-kafka";

        // Create and return the Docker Compose service.
        return new DockerComposeCompositeService(DockerHost, compose);
    }
}