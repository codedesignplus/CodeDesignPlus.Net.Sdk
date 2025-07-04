namespace CodeDesignPlus.Net.Logger.Options;

/// <summary>
/// Options for configuring the logger.
/// </summary>
public class LoggerOptions : IValidatableObject
{
    /// <summary>
    /// The name of the section used in the appsettings.
    /// </summary>
    public const string Section = "Logger";

    /// <summary>
    /// Gets or sets a value indicating whether logging is enabled.
    /// </summary>
    public bool Enable { get; set; }

    /// <summary>
    /// Gets or sets the OpenTelemetry endpoint.
    /// </summary>
    [Url]
    public string OTelEndpoint { get; set; }

    /// <summary>
    /// Gets or sets the log level for the logger.
    /// </summary>
    public string Level { get; set; } = null!;

    /// <summary>
    /// Validates the properties of the LoggerOptions.
    /// </summary>
    /// <param name="validationContext">The context information about the validation operation.</param>
    /// <returns>A collection of validation results.</returns>
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var results = new List<ValidationResult>();

        if (Enable)
            Validator.TryValidateProperty(OTelEndpoint, new ValidationContext(this) { MemberName = nameof(OTelEndpoint) }, results);

        return results;
    }
}