using System.ComponentModel.DataAnnotations;

namespace CodeDesignPlus.Net.Security.Options;

/// <summary>
/// Options to setting of the Security
/// </summary>
public class SecurityOptions : IValidatableObject
{
    /// <summary>
    /// Name of the setions used in the appsettings
    /// </summary>
    public static readonly string Section = "Security";

    /// <summary>
    /// Get or sets the Enable
    /// </summary>
    public bool Enable { get; set; }
    /// <summary>
    /// Gets or sets the name
    /// </summary>
    [Required]
    public string Name { get; set; }
    /// <summary>
    /// Gets or sets the name
    /// </summary>
    [EmailAddress]
    public string Email { get; set; }

    /// <summary>
    /// Determines whether the specified object is valid.
    /// </summary>
    /// <param name="validationContext">The validation context.</param>
    /// <returns>A collection that holds failed-validation information.</returns>
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var results = new List<ValidationResult>();

        if (this.Enable)
        {
            if (string.IsNullOrEmpty(Email))
                results.Add(new ValidationResult($"The {nameof(this.Email)} field is required."));

            Validator.TryValidateProperty(
                this.Email,
                new ValidationContext(this, null, null) { MemberName = nameof(this.Email) },
                results
            );
        }

        return results;
    }
}
