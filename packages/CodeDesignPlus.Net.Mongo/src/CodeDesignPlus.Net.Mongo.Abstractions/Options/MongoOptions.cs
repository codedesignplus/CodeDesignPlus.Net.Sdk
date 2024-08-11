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
    /// Gets or sets the Diagnostic
    /// </summary>
    public MongoDiagnosticsOptions Diagnostic { get; set; } = new();
    /// <summary>
    /// Gets or sets the RegisterAutomaticRepositories
    /// </summary>
    public bool RegisterAutomaticRepositories { get; set; } = true;


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
            Validator.TryValidateProperty(
                this.Database,
                new ValidationContext(this, null, null) { MemberName = nameof(this.Database) },
                results
            );

            Validator.TryValidateProperty(
                this.ConnectionString,
                new ValidationContext(this, null, null) { MemberName = nameof(this.ConnectionString) },
                results
            );

            if (this.Diagnostic.Enable)
                Validator.TryValidateObject(this.Diagnostic, new ValidationContext(this.Diagnostic), results, true);
        }

        return results;
    }
}
