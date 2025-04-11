namespace CodeDesignPlus.Net.Redis.Options;

/// <summary>
/// Represents the configuration options for Redis.
/// </summary>
public class RedisOptions : IValidatableObject
{
    /// <summary>
    /// The configuration section name for Redis options.
    /// </summary>
    public const string Section = "Redis";
    /// <summary>
    /// Gets or sets the connection timeout in milliseconds.
    /// </summary>
    public bool RegisterHealthCheck { get; set; } = true;
    /// <summary>
    /// Gets or sets the dictionary of Redis instances.
    /// </summary>
    public Dictionary<string, Instance> Instances { get; set; } = [];

    /// <summary>
    /// Validates the properties of the <see cref="RedisOptions"/> instance.
    /// </summary>
    /// <param name="validationContext">The context information about the validation operation.</param>
    /// <returns>A collection of validation results.</returns>
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var result = new List<ValidationResult>();

        if (Instances.Count == 0)
            result.Add(new ValidationResult("The Instances list must not be empty.", new[] { nameof(this.Instances) }));

        foreach (var instance in this.Instances.Where(x => x.Value.CreateConfiguration().Ssl && string.IsNullOrEmpty(x.Value.Certificate)))
        {
            result.Add(new ValidationResult("The Certificate is required.", new[] { nameof(Instance.Certificate) }));
        }

        return result;
    }
}