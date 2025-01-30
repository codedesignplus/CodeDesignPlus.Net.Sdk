using System.ComponentModel.DataAnnotations;

namespace CodeDesignPlus.Net.Redis.Cache.Abstractions.Options;

/// <summary>
/// Options to setting of the Redis.ache
/// </summary>
public class RedisCacheOptions: IValidatableObject
{
    /// <summary>
    /// Name of the setions used in the appsettings
    /// </summary>
    public static readonly string Section = "RedisCache";

    /// <summary>
    /// Get or sets the Enable
    /// </summary>
    public bool Enable { get; set; }
    /// <summary>
    /// Gets or sets the expiration time for the cache.
    /// </summary>
    public TimeSpan Expiration { get; set; }

    /// <summary>
    /// Validate the properties of the class
    /// </summary>
    /// <param name="validationContext">The context of the validation</param>
    /// <returns>The collection of errors</returns>
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Enable && Expiration <= TimeSpan.Zero)
            yield return new ValidationResult("The Expiration field is required.", [nameof(Expiration)]);        
    }

}
