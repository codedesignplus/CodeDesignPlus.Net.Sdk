using System.ComponentModel.DataAnnotations;

namespace CodeDesignPlus.Net.Redis.Event.Bus.Options;

/// <summary>
/// Options to setting of the Redis.Event.Bus
/// </summary>
public class RedisEventBusOptions
{
    /// <summary>
    /// Name of the setions used in the appsettings
    /// </summary>
    public static readonly string Section = "RedisEventBus";

    /// <summary>
    /// Get or sets the Enable
    /// </summary>
    public bool Enable { get; set; }
    /// <summary>
    /// Gets or sets the name the instance of the Redis to publish and subcribe events
    /// </summary>
    [Required]
    public string Name { get; set; }
}
