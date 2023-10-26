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
}
