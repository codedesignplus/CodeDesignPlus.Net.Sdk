namespace CodeDesignPlus.Net.xUnit.Containers.ServiceBusContainer;

/// <summary>
/// Represents a Docker container for the Azure Service Bus emulator, managed using Docker Compose.
/// </summary>
/// <remarks>
/// El emulador necesita dos puertos: el de AMQP para publicar y consumir, y el de gestion para crear topics y
/// suscripciones. La clase base solo resuelve uno, asi que el segundo se resuelve aqui.
/// </remarks>
public class ServiceBusContainer : DockerCompose
{
    /// <summary>
    /// Port the emulator listens on for management operations inside the container.
    /// </summary>
    private const int InternalManagementPort = 5300;

    /// <summary>
    /// Gets the host port mapped to the management endpoint of the emulator.
    /// </summary>
    public int ManagementPort { get; private set; }

    /// <summary>
    /// Gets the connection string used for publishing and consuming messages.
    /// </summary>
    public string ConnectionString => BuildConnectionString(this.Port);

    /// <summary>
    /// Gets the connection string used for creating topics and subscriptions.
    /// </summary>
    /// <remarks>
    /// El emulador atiende el plano de gestion en otro puerto, y el cliente de administracion exige que la
    /// cadena lo lleve explicito.
    /// </remarks>
    public string ManagementConnectionString => BuildConnectionString(this.ManagementPort);

    /// <summary>
    /// Builds the Docker Compose service configuration for the Azure Service Bus emulator.
    /// </summary>
    /// <returns>An <see cref="ICompositeService"/> representing the Docker Compose service.</returns>
    protected override ICompositeService Build()
    {
        var file = Path.Combine(Directory.GetCurrentDirectory(), "Containers", "ServiceBusContainer", "docker-compose.yml");

        var dockerCompose = new DockerComposeConfig
        {
            ComposeFilePath = [file],
            ForceRecreate = true,
            RemoveOrphans = true,
            StopOnDispose = true,
            AlternativeServiceName = "servicebus_" + Guid.NewGuid().ToString("N"),
            ComposeVersion = ComposeVersion.V2,
        };

        this.EnableGetPort = true;
        this.InternalPort = 5672;
        this.ContainerName = "servicebus";

        return new DockerComposeCompositeService(base.DockerHost, dockerCompose);
    }

    /// <summary>
    /// Resolves the host port mapped to the management endpoint once the container is running.
    /// </summary>
    protected override void OnContainerInitialized()
    {
        var container = this.CompositeService.Containers.First(x => x.Name.StartsWith(this.ContainerName));

        this.ManagementPort = container.ToHostExposedEndpoint($"{InternalManagementPort}/tcp").Port;
    }

    /// <summary>
    /// Builds the static connection string the emulator accepts, bound to the specified host port.
    /// </summary>
    /// <param name="port">The host port.</param>
    /// <returns>The connection string.</returns>
    private static string BuildConnectionString(int port)
        => $"Endpoint=sb://localhost:{port};SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=SAS_KEY_VALUE;UseDevelopmentEmulator=true;";
}
