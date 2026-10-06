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
    /// Gets or sets a value indicating whether to register the health check.
    /// </summary>
    public bool RegisterHealthCheck { get; set; } = true;

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
    /// Gets or sets a value indicating whether to register automatic repositories.
    /// </summary>
    public bool RegisterAutomaticRepositories { get; set; } = true;

    /// <summary>
    /// Gets or sets how many times <c>UpdateWithRetryAsync</c> reads, reapplies and saves a versioned aggregate before
    /// giving up with a concurrency conflict, when the caller does not pass its own limit.
    /// </summary>
    /// <remarks>
    /// Sized for the concurrency of one subscription across replicas (a handful of writers on the same document). When
    /// it runs out the conflict escapes and the message bus retries the whole message, so a low value is not data loss.
    /// </remarks>
    [Range(1, 100)]
    public int ConcurrencyMaxAttempts { get; set; } = 10;

    /// <summary>
    /// Gets or sets the upper bound, in milliseconds, of the random wait before the first retry of a conflicting write.
    /// It grows linearly with each attempt; 0 retries immediately.
    /// </summary>
    [Range(0, 10_000)]
    public int ConcurrencyRetryDelayMilliseconds { get; set; } = 25;

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
        }

        Validator.TryValidateProperty(
            this.ConcurrencyMaxAttempts,
            new ValidationContext(this, null, null) { MemberName = nameof(this.ConcurrencyMaxAttempts) },
            results
        );

        Validator.TryValidateProperty(
            this.ConcurrencyRetryDelayMilliseconds,
            new ValidationContext(this, null, null) { MemberName = nameof(this.ConcurrencyRetryDelayMilliseconds) },
            results
        );

        return results;
    }
}
