using System.Security.Authentication;

namespace CodeDesignPlus.Net.Mongo.Abstractions.Options;

/// <summary>
/// Represents the options for configuring MongoDB.
/// </summary>
public class MongoOptions : IValidatableObject
{
    /// <summary>
    /// The name of the section used in the configuration.
    /// </summary>
    public static readonly string Section = "Mongo";

    /// <summary>
    /// Gets or sets a value indicating whether MongoDB is enabled.
    /// </summary>
    public bool Enable { get; set; }

    /// <summary>
    /// Gets or sets the connection string for MongoDB.
    /// </summary>
    [Required]
    public string ConnectionString { get; set; }

    /// <summary>
    /// Gets or sets the name of the MongoDB database.
    /// </summary>
    [Required]
    public string Database { get; set; }
    /// <summary>
    /// Gets or sets the SSL protocols to use when connecting to MongoDB.
    /// </summary>
    public SslProtocols SslProtocols { get; set; } = SslProtocols.Tls12 | SslProtocols.Tls13;

    /// <summary>
    /// Gets or sets the diagnostics options for MongoDB.
    /// </summary>
    public MongoDiagnosticsOptions Diagnostic { get; set; } = new();

    /// <summary>
    /// Gets or sets a value indicating whether to register automatic repositories.
    /// </summary>
    public bool RegisterAutomaticRepositories { get; set; } = true;

    /// <summary>
    /// Validates the properties of the MongoOptions.
    /// </summary>
    /// <param name="validationContext">The context information about the validation operation.</param>
    /// <returns>A collection of validation results.</returns>
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
