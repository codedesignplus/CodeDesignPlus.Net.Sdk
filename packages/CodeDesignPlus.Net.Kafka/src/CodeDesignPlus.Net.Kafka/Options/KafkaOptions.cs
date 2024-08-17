namespace CodeDesignPlus.Net.Kafka.Options;

/// <summary>
/// Options to setting of the Kafka
/// </summary>
public class KafkaOptions: PubSubOptions
{
    /// <summary>
    /// Name of the setions used in the appsettings
    /// </summary>
    public static new readonly string Section = "Kafka";

    /// <summary>
    /// Get or sets the Enable
    /// </summary>
    public bool Enable { get; set; }

    public string BootstrapServers { get; set; }
    public Acks Acks { get; set; } = Acks.All;
    public int BatchSize { get; set; }
    public int LingerMs { get; set; }
    public CompressionType CompressionType { get; set; } = CompressionType.Snappy;
    [Required]
    public string NameMicroservice { get; set; }

    public SecurityProtocol SecurityProtocol { get; set; } = SecurityProtocol.Plaintext;

    public ProducerConfig ProducerConfig => new()
    {
        BootstrapServers = this.BootstrapServers,
        Acks = this.Acks,
        BatchSize = this.BatchSize,
        LingerMs = this.LingerMs,
        CompressionType = this.CompressionType,
        SecurityProtocol = SecurityProtocol
    };

    public ConsumerConfig ConsumerConfig => new()
    {
        BootstrapServers = this.BootstrapServers,
        GroupId = this.NameMicroservice,
        AutoOffsetReset = AutoOffsetReset.Earliest
    };

    public AdminClientConfig AdminClientConfig => new()
    {
        BootstrapServers = this.BootstrapServers
    };

    public int MaxAttempts { get; set; } = 60;
}
