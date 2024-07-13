using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;

namespace CodeDesignPlus.Net.Redis.Services;

/// <summary>
/// The default IServiceProvider. 
/// </summary>
public class RedisService : IRedisService
{
    /// <summary>
    /// Options to control serialization behavior.
    /// </summary>
    private readonly JsonSerializerOptions jsonSerializerOptions = new() { WriteIndented = true };
    /// <summary>
    /// A generic interface for logging
    /// </summary>
    private readonly ILogger<RedisService> logger;
    /// <summary>
    /// Represents the abstract multiplexer API
    /// </summary>
    public IConnectionMultiplexer Connection { get; private set; }
    /// <summary>
    /// Describes functionality that is common to both standalone redis servers and redis clusters
    /// </summary>
    public IDatabaseAsync Database { get; private set; }
    /// <summary>
    /// A redis connection used as the subscriber in a pub/sub scenario
    /// </summary>
    public ISubscriber Subscriber { get; private set; }
    /// <summary>
    /// Indicates whether any servers are connected
    /// </summary>
    public bool IsConnected { get => this.Connection.IsConnected; }

    /// <summary>
    /// Initializes a new instance of the <see cref="RedisService"/>
    /// </summary>
    /// <param name="logger">A generic interface for logging</param>
    /// <exception cref="ArgumentNullException">options is null</exception>
    /// <exception cref="ArgumentNullException">logger is null</exception>
    public RedisService(ILogger<RedisService> logger)
    {
        ArgumentNullException.ThrowIfNull(logger);
        
        this.logger = logger;
    }

    /// <summary>
    /// Initiates a connection to the provided Redis instance.
    /// </summary>
    /// <param name="instance">Configuration details of the Redis instance to connect to.</param>
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
    /// Configures the SSL settings if required based on the provided instance and configuration options.
    /// </summary>
    /// <param name="instance">The instance whose SSL settings need to be configured.</param>
    /// <param name="configuration">The configuration options associated with the provided instance.</param>

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
    /// Selects the appropriate certificate based on provided details.
    /// </summary>
    /// <param name="passwordCertificate">The password for the certificate.</param>
    /// <param name="certificate">The certificate details.</param>
    /// <returns>A <see cref="X509Certificate2"/> instance constructed based on provided details.</returns>   
    private static X509Certificate2 CertificateSelection(string passwordCertificate, string certificate)
    {
        if (!string.IsNullOrEmpty(passwordCertificate))
            return new X509Certificate2(certificate, passwordCertificate);
        else
            return new X509Certificate2(certificate);
    }

    /// <summary>
    /// A RemoteCertificateValidationCallback delegate responsible for validating the
    /// certificate supplied by the remote party; note that this cannot be specified
    /// in the configuration-string.
    /// </summary>
    /// <param name="chain">The chain of certificate authorities associated with the remote certificate.</param>
    /// <param name="sslPolicyErrors">One or more errors associated with the remote certificate.</param>    
    /// <param name="passwordCertificate">The password for the certificate.</param>
    /// <param name="certificate">The certificate details.</param>
    /// <returns>A System.Boolean value that determines whether the specified certificate is accepted for authentication.</returns>
    private static bool CertificateValidation(X509Chain chain, SslPolicyErrors sslPolicyErrors, string passwordCertificate, string certificate)
    {
        if (sslPolicyErrors == SslPolicyErrors.None)
            return true;

        // Extraer el CA del certificado del servidor
        var serverCACertThumbprint = chain.ChainElements[^1].Certificate.Thumbprint;

        // Carga todos los certificados del archivo PFX
        var clientCertCollection = new X509Certificate2Collection();
        clientCertCollection.Import(certificate, passwordCertificate, X509KeyStorageFlags.DefaultKeySet);

        // Encuentra el certificado principal (el que tiene la clave privada, por lo general es el certificado del cliente)
        var clientCert = clientCertCollection.OfType<X509Certificate2>().FirstOrDefault(cert => cert.HasPrivateKey);

        // Construye una cadena para el certificado del cliente utilizando un X509ChainPolicy para considerar todos los certificados en el archivo PFX
        var clientCertChain = new X509Chain();
        var chainPolicy = new X509ChainPolicy
        {
            RevocationMode = X509RevocationMode.NoCheck
        };
        chainPolicy.ExtraStore.AddRange(clientCertCollection);

        clientCertChain.ChainPolicy = chainPolicy;
        clientCertChain.Build(clientCert);

        // Obten el thumbprint del CA del certificado del cliente
        var clientCACertThumbprint = clientCertChain.ChainElements[^1].Certificate.Thumbprint;

        // Compara ambos thumbprints de los CA
        return serverCACertThumbprint == clientCACertThumbprint;
    }

    /// <summary>
    /// Register event handlers 
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
    /// Raised whenever an internal error occurs (this is primarily for debugging)
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="args">Event information</param>
    private void InternalError(object sender, InternalErrorEventArgs args)
    {
        var data = new
        {
            args.ConnectionType,
            EndPoint = args.EndPoint.ToString(),
            args.Origin
        };

        this.logger.LogCritical(args.Exception, "Internal Error - Data: {data}", JsonSerializer.Serialize(data, this.jsonSerializerOptions));
    }

    /// <summary>
    /// Raised when a hash-slot has been relocated
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="args">Event information</param>
    private void HashSlotMoved(object sender, HashSlotMovedEventArgs args)
    {
        var data = new
        {
            args.HashSlot,
            OldEndPoint = args.OldEndPoint.ToString(),
            NewEndPoint = args.NewEndPoint.ToString()
        };

        this.logger.LogWarning("Hash Slot Moved - Data: {data}", JsonSerializer.Serialize(data, this.jsonSerializerOptions));
    }

    /// <summary>
    /// A server replied with an error message;
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="args">Event information</param>
    private void ErrorMessage(object sender, RedisErrorEventArgs args)
    {
        var data = new
        {
            EndPoint = args.EndPoint.ToString(),
            args.Message
        };

        this.logger.LogError("Error Message - Data: {data}", JsonSerializer.Serialize(data, this.jsonSerializerOptions));
    }

    /// <summary>
    /// Raised whenever a physical connection is established
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="args">Event information</param>
    private void ConnectionRestored(object sender, ConnectionFailedEventArgs args)
    {
        var data = new
        {
            args.ConnectionType,
            EndPoint = args.EndPoint.ToString(),
            args.FailureType,
            physicalNameConnection = args.ToString()
        };

        this.logger.LogInformation(args.Exception, "Connection Restored - Data: {data}", JsonSerializer.Serialize(data, this.jsonSerializerOptions));
    }

    /// <summary>
    /// Raised whenever a physical connection fails
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="args">Event information</param>
    private void ConnectionFailed(object sender, ConnectionFailedEventArgs args)
    {
        var data = new
        {
            args.ConnectionType,
            EndPoint = args.EndPoint.ToString(),
            args.FailureType,
            physicalNameConnection = args.ToString()
        };

        this.logger.LogInformation(args.Exception, "Connection Failed - Data: {data}", JsonSerializer.Serialize(data, this.jsonSerializerOptions));
    }

    /// <summary>
    /// Raised when nodes are explicitly requested to reconfigure via broadcast; this usually means master/replica changes
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="args">Event information</param>
    private void ConfigurationChangedBroadcast(object sender, EndPointEventArgs args)
    {
        var data = new
        {
            EndPoint = args.EndPoint.ToString(),
        };

        this.logger.LogInformation("Configuration Changed Broadcast - Data: {data}", JsonSerializer.Serialize(data, this.jsonSerializerOptions));
    }

    /// <summary>
    /// Raised when configuration changes are detected
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="args">Event information</param>
    private void ConfigurationChanged(object sender, EndPointEventArgs args)
    {
        var data = new
        {
            EndPoint = args.EndPoint.ToString(),
        };

        this.logger.LogInformation("Configuration Changed - Data: {data}", JsonSerializer.Serialize(data, this.jsonSerializerOptions));
    }
}