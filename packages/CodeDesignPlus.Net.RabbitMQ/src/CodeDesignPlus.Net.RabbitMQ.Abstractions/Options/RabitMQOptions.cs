namespace CodeDesignPlus.Net.RabbitMQ.Abstractions.Options;

/// <summary>
/// Options to setting of the RabbitMQ
/// </summary>
public class RabbitMQOptions : IValidatableObject
{
    /// <summary>
    /// Name of the setions used in the appsettings
    /// </summary>
    public static readonly string Section = "RabbitMQ";

    /// <summary>
    /// Get or sets the Enable
    /// </summary>
    public bool Enable { get; set; }
    [Required]
    public string Host { get; set; }
    [Range(1, 65535)]
    public int Port { get; set; }
    [Required]
    public string UserName { get; set; }
    [Required]
    public string Password { get; set; }
    [Range(1000, 5000)]
    public int RetryInterval { get; set; } = 1000;
    [Range(1, 10)]
    public int MaxRetry { get; set; } = 10;
    public QueueArguments QueueArguments { get; set; } = new();

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
