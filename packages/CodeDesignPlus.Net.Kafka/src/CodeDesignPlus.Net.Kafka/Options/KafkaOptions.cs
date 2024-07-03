using Confluent.Kafka;
using System.ComponentModel.DataAnnotations;

namespace CodeDesignPlus.Net.Kafka.Options;

/// <summary>
/// Options to setting of the Kafka
/// </summary>
public class KafkaOptions
{
    /// <summary>
    /// Name of the setions used in the appsettings
    /// </summary>
    public static readonly string Section = "Kafka";

    /// <summary>
    /// Get or sets the Enable
    /// </summary>
    public bool Enable { get; set; }

    public string BootstrapServers { get; set; }
    public string Acks { get; set; }
    public int BatchSize { get; set; }
    public int LingerMs { get; set; }
    public string CompressionType { get; set; }
    [Required]
    public string NameMicroservice { get; set; }

    public ProducerConfig ProducerConfig => new()
    {
        BootstrapServers = this.BootstrapServers,
        //Acks = this.Acks,
        // BatchSize = this.BatchSize,
        // LingerMs = this.LingerMs,
        //CompressionType = this.CompressionType
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
