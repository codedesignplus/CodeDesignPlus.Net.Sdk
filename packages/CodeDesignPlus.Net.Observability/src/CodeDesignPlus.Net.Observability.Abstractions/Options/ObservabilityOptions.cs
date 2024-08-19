using System.ComponentModel.DataAnnotations;

namespace CodeDesignPlus.Net.Observability.Abstractions.Options;

/// <summary>
/// Represents the options for configuring observability.
/// </summary>
public class ObservabilityOptions : IValidatableObject
{
    /// <summary>
    /// The name of the section used in the configuration.
    /// </summary>
    public static readonly string Section = "Observability";

    /// <summary>
    /// Gets or sets a value indicating whether observability is enabled.
    /// </summary>
    public bool Enable { get; set; }

    /// <summary>
    /// Gets or sets the OpenTelemetry server URI.
    /// </summary>
    public Uri ServerOtel { get; set; }

    /// <summary>
    /// Gets or sets the metrics options.
    /// </summary>
    public Metrics Metrics { get; set; }

    /// <summary>
    /// Gets or sets the trace options.
    /// </summary>
    public Trace Trace { get; set; }

    /// <summary>
    /// Validates the properties of the ObservabilityOptions.
    /// </summary>
    /// <param name="validationContext">The context information about the validation operation.</param>
    /// <returns>A collection of validation results.</returns>
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var results = new List<ValidationResult>();

        if (Enable && ServerOtel is null)
            results.Add(new ValidationResult("The ServerOtel field is required.", new[] { nameof(ServerOtel) }));

        return results;
    }
}