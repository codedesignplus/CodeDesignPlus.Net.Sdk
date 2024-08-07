using System.ComponentModel.DataAnnotations;

namespace CodeDesignPlus.Net.Observability.Abstractions.Options;


/// <summary>
/// Options to setting of the Observability
/// </summary>
public class ObservabilityOptions : IValidatableObject
{
    /// <summary>
    /// Name of the setions used in the appsettings
    /// </summary>
    public static readonly string Section = "Observability";

    /// <summary>
    /// Get or sets the Enable
    /// </summary>
    public bool Enable { get; set; }
    public Uri ServerOtel { get; set; }
    public Metrics Metrics { get; set; }
    public Trace Trace { get; set; }

    /// <summary>
    /// Determines whether the specified object is valid.
    /// </summary>
    /// <param name="validationContext">The validation context.</param>
    /// <returns>A collection that holds failed-validation information.</returns>
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var results = new List<ValidationResult>();

        if (Enable && ServerOtel is null)
            results.Add(new ValidationResult("The ServerOtel field is required.", [nameof(ServerOtel)]));

        return results;
    }
}
