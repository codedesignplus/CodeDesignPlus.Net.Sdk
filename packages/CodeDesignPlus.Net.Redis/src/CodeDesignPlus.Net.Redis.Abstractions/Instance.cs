namespace CodeDesignPlus.Net.Redis.Abstractions;

/// <summary>
/// Represents the configuration for a Redis instance, allowing for customization of various settings related to the connection and operation of Redis within the context of a microservice.
/// </summary>
public class Instance
{
    /// <summary>
    /// Gets or sets the connection string used for connecting to the Redis server. 
    /// It can include several configuration parameters like EndPoints, Password, and more, separated by commas.
    /// </summary>
    [Required]
    [RegularExpression(@"^(\w+=\w+)(,\w+=\w+)*$", ErrorMessage = "Invalid connection string format.")]
    public string ConnectionString { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to use ThreadPriority.AboveNormal for SocketManager reader and writer threads.
    /// If false, ThreadPriority.Normal will be used. Default is true.
    /// </summary>
    public bool HighPrioritySocketThreads { get; set; } = true;

    /// <summary>
    /// Gets or sets the file path to the PFX certificate used for SSL connections.
    /// </summary>
    public string Certificate { get; set; }

    /// <summary>
    /// Gets or sets the password for the PFX certificate.
    /// </summary>
    public string PasswordCertificate { get; set; }

    /// <summary>
    /// Creates a new instance of <see cref="ConfigurationOptions"/> based on the current instance properties.
    /// </summary>
    /// <returns>The options relevant to a set of Redis connections.</returns>
    public ConfigurationOptions CreateConfiguration()
    {
        var configuration = ConfigurationOptions.Parse(this.ConnectionString);

        configuration.SocketManager = new SocketManager("RedisInstance", this.HighPrioritySocketThreads);

        return configuration;
    }
}