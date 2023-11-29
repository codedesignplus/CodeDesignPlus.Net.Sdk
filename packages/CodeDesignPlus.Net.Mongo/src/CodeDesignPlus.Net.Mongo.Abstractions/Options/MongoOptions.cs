using System.ComponentModel.DataAnnotations;

namespace CodeDesignPlus.Net.Mongo.Abstractions.Options;

/// <summary>
/// Options to setting of the Mongo
/// </summary>
public class MongoOptions : IValidatableObject
{
    /// <summary>
    /// Name of the setions used in the appsettings
    /// </summary>
    public static readonly string Section = "Mongo";

    /// <summary>
    /// Get or sets the Enable
    /// </summary>
    public bool Enable { get; set; }
    /// <summary>
    /// Gets or sets the name
    /// </summary>
    [Required]
    public string ConnectionString { get; set; }
    /// <summary>
    /// Gets or sets the name
    /// </summary>
    [Required]
    public string Database { get; set; }

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
            if (string.IsNullOrEmpty(Database))
                results.Add(new ValidationResult($"The {nameof(this.Database)} field is required."));

            Validator.TryValidateProperty(
                this.Database,
                new ValidationContext(this, null, null) { MemberName = nameof(this.Database) },
                results
            );
        }

        return results;
    }
}
