namespace CodeDesignPlus.Net.xUnit.Helpers.VaultContainer;

/// <summary>
/// Represents a Docker container for SQL Server, managed using Docker Compose.
/// </summary>
public class VaultContainer : DockerCompose
{
    public VaultCredentials Credentials { get; private set; }

    private readonly string id = Guid.NewGuid().ToString("N");

    public VaultContainer() : base()
    {

    }

    /// <summary>
    /// Builds the Docker Compose service configuration for the SQL Server container.
    /// </summary>
    /// <returns>An <see cref="ICompositeService"/> representing the Docker Compose service.</returns>
    protected override ICompositeService Build()
    {
        var file = Path.Combine(Directory.GetCurrentDirectory(), "Helpers", "VaultContainer", "docker-compose.yml");

        var dockerCompose = new DockerComposeConfig
        {
            ComposeFilePath = [file],
            ForceRecreate = true,
            RemoveOrphans = true,
            StopOnDispose = true,
            AlternativeServiceName = "vault_" + this.id,
            EnvironmentNameValue = new Dictionary<string, string>
            {
                { "FILE_CREDENTIAL", this.id },
            }
        };

        this.EnableGetPort = true;
        this.InternalPort = 8200;
        this.ContainerName = $"{dockerCompose.AlternativeServiceName}-vault";

        var compose = new DockerComposeCompositeService(base.DockerHost, dockerCompose);

        return compose;
    }

    protected override void OnContainerInitialized()
    {
        // Wait for the vault container to be ready (RabbitMQ)
        Thread.Sleep(30000);

        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Helpers", "VaultContainer", "shared", "vault-config", $"{this.id}-credentials.json");

        if (!File.Exists(filePath))
            throw new FileNotFoundException("The file with the credentials was not found.", filePath);

        string json = File.ReadAllText(filePath);
        this.Credentials = JsonSerializer.Deserialize<VaultCredentials>(json);
    }
}


public class VaultCredentials
{
    [Newtonsoft.Json.JsonProperty("role_id")]
    public string RoleId { get; set; }
    [Newtonsoft.Json.JsonProperty("secret_id")]
    public string SecretId { get; set; }
}

