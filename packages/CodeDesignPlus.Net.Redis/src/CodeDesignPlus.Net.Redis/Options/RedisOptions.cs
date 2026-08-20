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
    /// <remarks>
    /// TLS does not require a client certificate. Managed services such as Azure Managed Redis
    /// authenticate with an access key over standard TLS, validating the server against the system
    /// certificate authorities. A client certificate is only needed for mutual TLS, so it stays
    /// optional and is only demanded when a password for it has been supplied.
    /// </remarks>
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var result = new List<ValidationResult>();

        if (this.Instances.Count == 0)
        {
            result.Add(new ValidationResult("The Instances list must not be empty.", [nameof(this.Instances)]));
        }

        foreach (var instance in this.Instances.Values)
        {
            ValidateInstance(instance, result);
        }

        return result;
    }

    /// <summary>
    /// Validates a single Redis instance.
    /// </summary>
    /// <param name="instance">The instance to validate.</param>
    /// <param name="result">The collection where the validation errors are accumulated.</param>
    private static void ValidateInstance(Instance instance, List<ValidationResult> result)
    {
        // The nested instances are not reached by ValidateDataAnnotations, which does not recurse
        // into the values of a dictionary. Validating them here is what makes the rules effective
        // at runtime instead of only in unit tests.
        if (string.IsNullOrWhiteSpace(instance.ConnectionString))
        {
            result.Add(new ValidationResult("The ConnectionString field is required.", [nameof(Instance.ConnectionString)]));

            return;
        }

        // StackExchange.Redis is lenient about host names, since any token without an equals sign
        // is taken as an endpoint, but strict about keywords: an unknown one raises ArgumentException
        // and a malformed value raises ArgumentOutOfRangeException, which derives from it.
        try
        {
            var configuration = ConfigurationOptions.Parse(instance.ConnectionString);

            // A connection string made only of keywords parses without complaint and leaves nothing
            // to connect to. "ssl=true" is the textbook case.
            if (configuration.EndPoints.Count == 0)
            {
                result.Add(new ValidationResult("The ConnectionString must contain at least one endpoint.", [nameof(Instance.ConnectionString)]));
            }
        }
        catch (ArgumentException exception)
        {
            result.Add(new ValidationResult($"Invalid connection string format. {exception.Message}", [nameof(Instance.ConnectionString)]));
        }

        if (!string.IsNullOrEmpty(instance.PasswordCertificate) && string.IsNullOrEmpty(instance.Certificate))
        {
            result.Add(new ValidationResult("The Certificate is required when the PasswordCertificate is set.", [nameof(Instance.Certificate)]));
        }
    }
}