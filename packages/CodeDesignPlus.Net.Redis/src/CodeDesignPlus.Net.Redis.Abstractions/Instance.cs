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
    /// <remarks>
    /// The format is validated by <see cref="RedisOptions"/>, which parses it with StackExchange.Redis.
    /// A regular expression cannot express this format: a connection string mixes bare endpoints
    /// (host:port) with key=value pairs, and endpoints may contain dots, hyphens and colons.
    /// </remarks>
    [Required]
    public string ConnectionString { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to use ThreadPriority.AboveNormal for SocketManager reader and writer threads.
    /// If false, ThreadPriority.Normal will be used. Default is true.
    /// </summary>
    public bool HighPrioritySocketThreads { get; set; } = true;

    /// <summary>
    /// Gets or sets the file path to the PFX certificate used for mutual TLS.
    /// </summary>
    /// <remarks>
    /// Optional. It is only needed when the server requires a client certificate, which is the case
    /// of a self-hosted Redis behind a private CA. Managed services such as Azure Managed Redis
    /// authenticate with an access key over standard TLS and must leave this empty.
    /// </remarks>
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

    /// <summary>
    /// Determines whether the connection string enables TLS.
    /// </summary>
    /// <returns><see langword="true"/> when the connection string enables TLS; otherwise, <see langword="false"/>.</returns>
    /// <remarks>
    /// This does not call <see cref="CreateConfiguration"/> on purpose: that method allocates a
    /// <see cref="SocketManager"/>, which owns a dedicated thread pool. Validation runs on every
    /// options read, so building one there would leak threads for a connection that is never opened.
    /// </remarks>
    public bool UseSsl()
    {
        if (string.IsNullOrWhiteSpace(this.ConnectionString))
        {
            return false;
        }

        try
        {
            return ConfigurationOptions.Parse(this.ConnectionString).Ssl;
        }
        catch (ArgumentException)
        {
            return false;
        }
    }
}