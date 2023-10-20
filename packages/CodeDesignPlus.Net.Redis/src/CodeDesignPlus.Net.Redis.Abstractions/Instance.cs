using System.ComponentModel.DataAnnotations;

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
    /// Use ThreadPriority.AboveNormal for SocketManager reader and writer threads (true by default). If false, ThreadPriority.Normal will be used.
    /// </summary>
    public bool HighPrioritySocketThreads { get; set; } = true;
    /// <summary>
    /// File PFX
    /// </summary>
    public string Certificate { get; set; }
    /// <summary>
    /// Password Certificate
    /// </summary>
    public string PasswordCertificate { get; set; }

    /// <summary>
    /// Create a new instance of <see cref="ConfigurationOptions"/>
    /// </summary>
    /// <returns>The options relevant to a set of redis connections</returns>
    public ConfigurationOptions CreateConfiguration()
    {
        var configuration = ConfigurationOptions.Parse(this.ConnectionString);

        configuration.SocketManager = new SocketManager("RedisInstance", this.HighPrioritySocketThreads);

        return configuration;
    }
}
