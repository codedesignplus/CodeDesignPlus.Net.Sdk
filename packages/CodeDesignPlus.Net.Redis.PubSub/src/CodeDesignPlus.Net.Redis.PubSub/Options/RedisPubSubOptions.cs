using System.ComponentModel.DataAnnotations;

namespace CodeDesignPlus.Net.Redis.PubSub.Options;

/// <summary>
/// Options to setting of the Redis.PubSub
/// </summary>
public class RedisPubSubOptions
{
    /// <summary>
    /// Name of the setions used in the appsettings
    /// </summary>
    public static readonly string Section = "RedisPubSub";

    /// <summary>
    /// Get or sets the Enable
    /// </summary>
    public bool Enable { get; set; }
    /// <summary>
    /// Gets or sets the name the instance of the Redis to publish and subcribe events
    /// </summary>
    [Required]
    public string Name { get; set; }

    /// <summary>
    /// True if the service is listening for events
    /// </summary>
    public bool ListenerEvents { get; set; }
}
