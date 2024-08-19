namespace CodeDesignPlus.Net.RabbitMQ.Abstractions.Options;

/// <summary>
/// Represents the configuration options for RabbitMQ.
/// </summary>
public class RabbitMQOptions : PubSubOptions, IValidatableObject
{
    /// <summary>
    /// The configuration section name for RabbitMQ options.
    /// </summary>
    public static new readonly string Section = "RabbitMQ";

    /// <summary>
    /// Gets or sets a value indicating whether RabbitMQ is enabled.
    /// </summary>
    public bool Enable { get; set; }

    /// <summary>
    /// Gets or sets the RabbitMQ host.
    /// </summary>
    [Required]
    public string Host { get; set; }

    /// <summary>
    /// Gets or sets the RabbitMQ port.
    /// </summary>
    [Range(1, 65535)]
    public int Port { get; set; }

    /// <summary>
    /// Gets or sets the RabbitMQ username.
    /// </summary>
    [Required]
    public string UserName { get; set; }

    /// <summary>
    /// Gets or sets the RabbitMQ password.
    /// </summary>
    [Required]
    public string Password { get; set; }

    /// <summary>
    /// Gets or sets the retry interval in milliseconds.
    /// </summary>
    [Range(1000, 5000)]
    public int RetryInterval { get; set; } = 1000;

    /// <summary>
    /// Gets or sets the maximum number of retry attempts.
    /// </summary>
    [Range(1, 10)]
    public int MaxRetry { get; set; } = 10;

    /// <summary>
    /// Gets or sets the queue arguments.
    /// </summary>
    public QueueArguments QueueArguments { get; set; } = new();

    /// <summary>
    /// Validates the properties of the <see cref="RabbitMQOptions"/> instance.
    /// </summary>
    /// <param name="validationContext">The context information about the validation operation.</param>
    /// <returns>A collection of validation results.</returns>
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var results = new List<ValidationResult>();

        if (this.Enable)
        {
            Validator.TryValidateProperty(this.Host, new ValidationContext(this, null, null) { MemberName = nameof(this.Host) }, results);
            Validator.TryValidateProperty(this.Port, new ValidationContext(this, null, null) { MemberName = nameof(this.Port) }, results);
            Validator.TryValidateProperty(this.UserName, new ValidationContext(this, null, null) { MemberName = nameof(this.UserName) }, results);
            Validator.TryValidateProperty(this.Password, new ValidationContext(this, null, null) { MemberName = nameof(this.Password) }, results);
            Validator.TryValidateProperty(this.RetryInterval, new ValidationContext(this, null, null) { MemberName = nameof(this.RetryInterval) }, results);
            Validator.TryValidateProperty(this.MaxRetry, new ValidationContext(this, null, null) { MemberName = nameof(this.MaxRetry) }, results);

            if (this.QueueArguments != null)
                results.AddRange(this.QueueArguments.Validate(new ValidationContext(this.QueueArguments)));
        }

        return results;
    }
}