namespace CodeDesignPlus.Net.xUnit.Helpers.VaultContainer;

/// <summary>
/// Represents a Docker container for SQL Server, managed using Docker Compose.
/// </summary>
public class VaultContainer : DockerCompose
{
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
            AlternativeServiceName = "vault_" + Guid.NewGuid().ToString("N"),
        };

        this.EnableGetPort = true;
        this.InternalPort = 8200;
        this.ContainerName = $"{dockerCompose.AlternativeServiceName}-vault";

        var compose = new DockerComposeCompositeService(base.DockerHost, dockerCompose);

        return compose;
    }

    public static VaultCredentials GetCredentials()
    {
        Thread.Sleep(2000);

        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Helpers", "VaultContainer", "shared", "vault-config", "credentials.json");
        
        if (!File.Exists(filePath))
            throw new FileNotFoundException("The file with the credentials was not found.", filePath);

        string json = File.ReadAllText(filePath);
        var credentials = JsonSerializer.Deserialize<VaultCredentials>(json);

        return credentials;
    }
}


public class VaultCredentials
{
    [Newtonsoft.Json.JsonProperty("role_id")]
    public string? RoleId { get; set; }
    [Newtonsoft.Json.JsonProperty("secret_id")]
    public string? SecretId { get; set; }
}

