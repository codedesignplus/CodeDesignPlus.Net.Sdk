using System.ComponentModel.DataAnnotations;

namespace CodeDesignPlus.Net.Redis.Options;

/// <summary>
/// Configuration options for the Redis service 
/// </summary>
public class RedisOptions : IValidatableObject
{
    /// <summary>
    /// Section Name
    /// </summary>
    public const string Section = "Redis";
    /// <summary>
    /// Gets or sets the Instances
    /// </summary>
    public List<Instance> Instances { get; set; } = new();

    /// <summary>
    /// Determines whether the specified object is valid.
    /// </summary>
    /// <param name="validationContext"> The validation context.</param>
    /// <returns>A collection that holds failed-validation information.</returns>
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var result = new List<ValidationResult>();

        if (!this.Instances.Any())
            result.Add(new ValidationResult("The Instances list must not be empty.", new string[] { nameof(this.Instances) }));


        foreach (var instance in this.Instances.Where(x => x.CreateConfiguration().Ssl))
        {
            if (string.IsNullOrEmpty(instance.Certificate))
                result.Add(new ValidationResult("The Certificate is required.", new string[] { nameof(Instance.Certificate) }));
        }

        return result;
    }
}