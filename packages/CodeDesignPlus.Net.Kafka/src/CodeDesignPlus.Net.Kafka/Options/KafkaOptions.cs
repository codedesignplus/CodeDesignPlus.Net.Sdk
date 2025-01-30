namespace CodeDesignPlus.Net.Kafka.Options;

/// <summary>
/// Options for configuring Kafka.
/// </summary>
public class KafkaOptions : PubSubOptions
{
    /// <summary>
    /// The name of the section used in the appsettings.
    /// </summary>
    public static new readonly string Section = "Kafka";

    /// <summary>
    /// Gets or sets a value indicating whether Kafka is enabled.
    /// </summary>
    public bool Enable { get; set; }

    /// <summary>
    /// Gets or sets the bootstrap servers.
    /// </summary>
    public string BootstrapServers { get; set; }

    /// <summary>
    /// Gets or sets the acknowledgment level.
    /// </summary>
    public Acks Acks { get; set; } = Acks.All;

    /// <summary>
    /// Gets or sets the batch size.
    /// </summary>
    public int BatchSize { get; set; }

    /// <summary>
    /// Gets or sets the linger time in milliseconds.
    /// </summary>
    public int LingerMs { get; set; }

    /// <summary>
    /// Gets or sets the compression type.
    /// </summary>
    public CompressionType CompressionType { get; set; } = CompressionType.Snappy;

    /// <summary>
    /// Gets or sets the security protocol.
    /// </summary>
    public SecurityProtocol SecurityProtocol { get; set; } = SecurityProtocol.Plaintext;

    /// <summary>
    /// Gets the producer configuration.
    /// </summary>
    public ProducerConfig ProducerConfig => new()
    {
        BootstrapServers = this.BootstrapServers,
        Acks = this.Acks,
        BatchSize = this.BatchSize,
        LingerMs = this.LingerMs,
        CompressionType = this.CompressionType,
        SecurityProtocol = this.SecurityProtocol
    };

    /// <summary>
    /// Gets the consumer configuration.
    /// </summary>
    public ConsumerConfig ConsumerConfig => new()
    {
        BootstrapServers = this.BootstrapServers,
        AutoOffsetReset = AutoOffsetReset.Earliest
    };

    /// <summary>
    /// Gets the admin client configuration.
    /// </summary>
    public AdminClientConfig AdminClientConfig => new()
    {
        BootstrapServers = this.BootstrapServers
    };

    /// <summary>
    /// Gets or sets the maximum number of attempts.
    /// </summary>
    public int MaxAttempts { get; set; } = 60;
}