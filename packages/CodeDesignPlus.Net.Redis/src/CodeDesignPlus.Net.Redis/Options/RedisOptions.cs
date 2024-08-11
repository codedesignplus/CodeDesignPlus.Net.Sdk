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
    public Dictionary<string, Instance> Instances { get; set; } = [];

    /// <summary>
    /// Determines whether the specified object is valid.
    /// </summary>
    /// <param name="validationContext"> The validation context.</param>
    /// <returns>A collection that holds failed-validation information.</returns>
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var result = new List<ValidationResult>();

        if (Instances.Count == 0)
            result.Add(new ValidationResult("The Instances list must not be empty.", [nameof(this.Instances)]));


        foreach (var instance in this.Instances.Where(x => x.Value.CreateConfiguration().Ssl && string.IsNullOrEmpty(x.Value.Certificate)))
        {
            result.Add(new ValidationResult("The Certificate is required.", [nameof(Instance.Certificate)]));
        }

        return result;
    }
}