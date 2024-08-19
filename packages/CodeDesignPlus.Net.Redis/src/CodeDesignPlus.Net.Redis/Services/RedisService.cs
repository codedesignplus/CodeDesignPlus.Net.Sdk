namespace CodeDesignPlus.Net.Redis.Services;

/// <summary>
/// Provides Redis services.
/// </summary>
public class RedisService : IRedisService
{
    private readonly ILogger<RedisService> logger;

    /// <summary>
    /// Gets the Redis connection multiplexer.
    /// </summary>
    public IConnectionMultiplexer Connection { get; private set; }

    /// <summary>
    /// Gets the Redis database.
    /// </summary>
    public IDatabaseAsync Database { get; private set; }

    /// <summary>
    /// Gets the Redis subscriber.
    /// </summary>
    public ISubscriber Subscriber { get; private set; }

    /// <summary>
    /// Gets a value indicating whether the Redis connection is connected.
    /// </summary>
    public bool IsConnected => this.Connection.IsConnected;

    /// <summary>
    /// Initializes a new instance of the <see cref="RedisService"/> class.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="logger"/> is null.</exception>
    public RedisService(ILogger<RedisService> logger)
    {
        ArgumentNullException.ThrowIfNull(logger);
        this.logger = logger;
    }

    /// <summary>
    /// Initializes the Redis service with the specified instance.
    /// </summary>
    /// <param name="instance">The Redis instance configuration.</param>
    public void Initialize(Instance instance)
    {
        var configuration = instance.CreateConfiguration();
        ConfigureSslIfRequired(instance, configuration);

        this.Connection = ConnectionMultiplexer.Connect(configuration);

        if (this.Connection.IsConnected)
        {
            this.RegisterEvents();
            this.Subscriber = this.Connection.GetSubscriber();
            this.Database = this.Connection.GetDatabase();
        }
    }

    /// <summary>
    /// Configures SSL if required for the Redis connection.
    /// </summary>
    /// <param name="instance">The Redis instance configuration.</param>
    /// <param name="configuration">The Redis configuration options.</param>
    private static void ConfigureSslIfRequired(Instance instance, ConfigurationOptions configuration)
    {
        if (configuration.Ssl)
        {
            configuration.CertificateSelection += (_, _, _, _, _) => CertificateSelection(
                instance.PasswordCertificate,
                instance.Certificate
            );

            configuration.CertificateValidation += (_, _, chain, sslPolicyErrors) => CertificateValidation(
                chain,
                sslPolicyErrors,
                instance.PasswordCertificate,
                instance.Certificate
            );
        }
    }

    /// <summary>
    /// Selects the certificate for SSL.
    /// </summary>
    /// <param name="passwordCertificate">The password for the certificate.</param>
    /// <param name="certificate">The certificate path.</param>
    /// <returns>The selected certificate.</returns>
    private static X509Certificate2 CertificateSelection(string passwordCertificate, string certificate)
    {
        if (!string.IsNullOrEmpty(passwordCertificate))
            return new X509Certificate2(certificate, passwordCertificate);
        else
            return new X509Certificate2(certificate);
    }

    /// <summary>
    /// Validates the SSL certificate.
    /// </summary>
    /// <param name="chain">The X509 chain.</param>
    /// <param name="sslPolicyErrors">The SSL policy errors.</param>
    /// <param name="passwordCertificate">The password for the certificate.</param>
    /// <param name="certificate">The certificate path.</param>
    /// <returns>True if the certificate is valid; otherwise, false.</returns>
    private static bool CertificateValidation(X509Chain chain, SslPolicyErrors sslPolicyErrors, string passwordCertificate, string certificate)
    {
        if (sslPolicyErrors == SslPolicyErrors.None)
            return true;

        var serverCACertThumbprint = chain.ChainElements[^1].Certificate.Thumbprint;

        var clientCertCollection = new X509Certificate2Collection();
        clientCertCollection.Import(certificate, passwordCertificate, X509KeyStorageFlags.DefaultKeySet);

        var clientCert = clientCertCollection.OfType<X509Certificate2>().FirstOrDefault(cert => cert.HasPrivateKey);

        var clientCertChain = new X509Chain();
        var chainPolicy = new X509ChainPolicy
        {
            RevocationMode = X509RevocationMode.NoCheck
        };
        chainPolicy.ExtraStore.AddRange(clientCertCollection);

        clientCertChain.ChainPolicy = chainPolicy;
        clientCertChain.Build(clientCert);

        var clientCACertThumbprint = clientCertChain.ChainElements[^1].Certificate.Thumbprint;

        return serverCACertThumbprint == clientCACertThumbprint;
    }

    /// <summary>
    /// Registers the Redis connection events.
    /// </summary>
    private void RegisterEvents()
    {
        this.Connection.ConfigurationChanged += this.ConfigurationChanged;
        this.Connection.ConfigurationChangedBroadcast += this.ConfigurationChangedBroadcast;
        this.Connection.ConnectionFailed += this.ConnectionFailed;
        this.Connection.ConnectionRestored += this.ConnectionRestored;
        this.Connection.ErrorMessage += this.ErrorMessage;
        this.Connection.HashSlotMoved += this.HashSlotMoved;
        this.Connection.InternalError += this.InternalError;
    }

    /// <summary>
    /// Handles the internal error event.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="args">The event arguments.</param>
    private void InternalError(object sender, InternalErrorEventArgs args)
    {
        var data = new
        {
            args.ConnectionType,
            EndPoint = args.EndPoint.ToString(),
            args.Origin
        };

        this.logger.LogCritical(args.Exception, "Internal Error - Data: {data}", JsonSerializer.Serialize(data));
    }

    /// <summary>
    /// Handles the hash slot moved event.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="args">The event arguments.</param>
    private void HashSlotMoved(object sender, HashSlotMovedEventArgs args)
    {
        var data = new
        {
            args.HashSlot,
            OldEndPoint = args.OldEndPoint.ToString(),
            NewEndPoint = args.NewEndPoint.ToString()
        };

        this.logger.LogWarning("Hash Slot Moved - Data: {data}", JsonSerializer.Serialize(data));
    }

    /// <summary>
    /// Handles the error message event.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="args">The event arguments.</param>
    private void ErrorMessage(object sender, RedisErrorEventArgs args)
    {
        var data = new
        {
            EndPoint = args.EndPoint.ToString(),
            args.Message
        };

        this.logger.LogError("Error Message - Data: {data}", JsonSerializer.Serialize(data));
    }

    /// <summary>
    /// Handles the connection restored event.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="args">The event arguments.</param>
    private void ConnectionRestored(object sender, ConnectionFailedEventArgs args)
    {
        var data = new
        {
            args.ConnectionType,
            EndPoint = args.EndPoint.ToString(),
            args.FailureType,
            physicalNameConnection = args.ToString()
        };

        this.logger.LogInformation(args.Exception, "Connection Restored - Data: {data}", JsonSerializer.Serialize(data));
    }

    /// <summary>
    /// Handles the connection failed event.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="args">The event arguments.</param>
    private void ConnectionFailed(object sender, ConnectionFailedEventArgs args)
    {
        var data = new
        {
            args.ConnectionType,
            EndPoint = args.EndPoint.ToString(),
            args.FailureType,
            physicalNameConnection = args.ToString()
        };

        this.logger.LogInformation(args.Exception, "Connection Failed - Data: {data}", JsonSerializer.Serialize(data));
    }

    /// <summary>
    /// Handles the configuration changed broadcast event.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="args">The event arguments.</param>
    private void ConfigurationChangedBroadcast(object sender, EndPointEventArgs args)
    {
        var data = new
        {
            EndPoint = args.EndPoint.ToString(),
        };

        this.logger.LogInformation("Configuration Changed Broadcast - Data: {data}", JsonSerializer.Serialize(data));
    }

    /// <summary>
    /// Handles the configuration changed event.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="args">The event arguments.</param>
    private void ConfigurationChanged(object sender, EndPointEventArgs args)
    {
        var data = new
        {
            EndPoint = args.EndPoint.ToString(),
        };

        this.logger.LogInformation("Configuration Changed - Data: {data}", JsonSerializer.Serialize(data));
    }
}