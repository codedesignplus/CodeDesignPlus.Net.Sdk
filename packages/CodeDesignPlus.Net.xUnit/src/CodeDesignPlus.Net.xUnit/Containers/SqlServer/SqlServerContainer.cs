namespace CodeDesignPlus.Net.xUnit.Containers.SqlServer;

/// <summary>
/// Represents a Docker container for SQL Server, managed using Docker Compose.
/// </summary>
public class SqlServerContainer : DockerCompose
{
    public string Password { get; private set; } = GeneratePassword();
    /// <summary>
    /// Builds the Docker Compose service configuration for the SQL Server container.
    /// </summary>
    /// <returns>An <see cref="ICompositeService"/> representing the Docker Compose service.</returns>
    protected override ICompositeService Build()
    {
        // Define the path to the Docker Compose file.
        var file = Path.Combine(Directory.GetCurrentDirectory(), "Containers", "SqlServer", "docker-compose.yml");

        // Configure the Docker Compose settings.
        var dockerCompose = new DockerComposeConfig
        {
            ComposeFilePath = [file],
            ForceRecreate = true,
            RemoveOrphans = true,
            StopOnDispose = true,
            AlternativeServiceName = "sql_" + Guid.NewGuid().ToString("N"),
            EnvironmentNameValue = new Dictionary<string, string>
            {
                { "SA_PASSWORD", this.Password },
            },
            ComposeVersion = ComposeVersion.V2,
        };
 
        // Enable port retrieval and set the internal port and container name.
        this.EnableGetPort = true;
        this.InternalPort = 1433;
        this.ContainerName = $"sqlserver";

        // Create and return the Docker Compose service.
        var compose = new DockerComposeCompositeService(base.DockerHost, dockerCompose);

        return compose;
    }

    public static string GeneratePassword()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()_+";
        var random = new Random();
        var password = new char[16];
        for (int i = 0; i < password.Length; i++)
        {
            password[i] = chars[random.Next(chars.Length)];
        }
        return new string(password);
    }
}