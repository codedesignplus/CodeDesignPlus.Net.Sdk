namespace CodeDesignPlus.Net.RabbitMQ.Abstractions.Options;

/// <summary>
/// Represents the arguments for configuring a RabbitMQ queue.
/// </summary>
public class QueueArguments : IValidatableObject
{
    /// <summary>
    /// Gets or sets the message time-to-live (TTL) in milliseconds.
    /// </summary>
    public int MessageTtl { get; set; } = 172800000;

    /// <summary>
    /// Gets or sets the expiration time for the queue in milliseconds.
    /// </summary>
    public int? Expires { get; set; }

    /// <summary>
    /// Gets or sets the maximum number of messages in the queue.
    /// </summary>
    public int? MaxLength { get; set; }

    /// <summary>
    /// Gets or sets the maximum size of the queue in bytes.
    /// </summary>
    public int? MaxLengthBytes { get; set; }

    /// <summary>
    /// Gets or sets the maximum priority for the queue.
    /// </summary>
    [Range(0, 255)]
    public int? MaxPriority { get; set; }

    /// <summary>
    /// Gets or sets the queue mode.
    /// </summary>
    [RegularExpression(@"^(default|lazy)?$")]
    public string QueueMode { get; set; } = "lazy";

    /// <summary>
    /// Gets or sets the queue master locator.
    /// </summary>
    [RegularExpression(@"^(min-masters)?$")]
    public string QueueMasterLocator { get; set; }

    /// <summary>
    /// Gets or sets the high availability (HA) policy for the queue.
    /// </summary>
    [RegularExpression(@"^(all|exactly|nodes|nodes\[\d+\])?$")]
    public string HaPolicy { get; set; } = "all";

    /// <summary>
    /// Gets or sets the overflow behavior for the queue.
    /// </summary>
    [RegularExpression(@"^(drop-head|reject-publish)?$")]
    public string Overflow { get; set; }

    /// <summary>
    /// Gets or sets the overflow reject publish value.
    /// </summary>
    public int? OverflowRejectPublish { get; set; }

    /// <summary>
    /// Gets or sets additional arguments for the queue.
    /// </summary>
    public Dictionary<string, object> ExtraArguments { get; set; }

    /// <summary>
    /// Gets the dictionary of arguments for the queue.
    /// </summary>
    /// <returns>A dictionary of queue arguments.</returns>
    public Dictionary<string, object> GetArguments()
    {
        var arguments = new Dictionary<string, object>();

        if (this.MessageTtl > 0)
            arguments.Add("x-message-ttl", this.MessageTtl);

        if (this.Expires.HasValue)
            arguments.Add("x-expires", this.Expires.Value);

        if (this.MaxLength.HasValue)
            arguments.Add("x-max-length", this.MaxLength.Value);

        if (this.MaxLengthBytes.HasValue)
            arguments.Add("x-max-length-bytes", this.MaxLengthBytes.Value);

        if (this.MaxPriority.HasValue)
            arguments.Add("x-max-priority", this.MaxPriority.Value);

        if (!string.IsNullOrWhiteSpace(this.QueueMode))
            arguments.Add("x-queue-mode", this.QueueMode);

        if (!string.IsNullOrWhiteSpace(this.QueueMasterLocator))
            arguments.Add("x-queue-master-locator", this.QueueMasterLocator);

        if (!string.IsNullOrWhiteSpace(this.HaPolicy))
            arguments.Add("x-ha-policy", this.HaPolicy);

        if (!string.IsNullOrWhiteSpace(this.Overflow))
            arguments.Add("x-overflow", this.Overflow);

        if (this.OverflowRejectPublish.HasValue)
            arguments.Add("x-overflow-reject-publish", this.OverflowRejectPublish);

        if (this.ExtraArguments != null)
            arguments.Add("x-arguments", this.ExtraArguments);

        return arguments;
    }

    /// <summary>
    /// Validates the properties of the <see cref="QueueArguments"/> instance.
    /// </summary>
    /// <param name="validationContext">The context information about the validation operation.</param>
    /// <returns>A collection of validation results.</returns>
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var results = new List<ValidationResult>();

        if (this.MessageTtl < 0)
            results.Add(new ValidationResult("The field MessageTtl must be greater than or equal to zero.", new[] { nameof(this.MessageTtl) }));

        if (this.Expires.HasValue && this.Expires.Value < 0)
            results.Add(new ValidationResult("The field Expires must be greater than or equal to zero.", new[] { nameof(this.Expires) }));

        if (this.MaxLength.HasValue && this.MaxLength.Value < 0)
            results.Add(new ValidationResult("The field MaxLength must be greater than or equal to zero.", new[] { nameof(this.MaxLength) }));

        if (this.MaxLengthBytes.HasValue && this.MaxLengthBytes.Value < 0)
            results.Add(new ValidationResult("The field MaxLengthBytes must be greater than or equal to zero.", new[] { nameof(this.MaxLengthBytes) }));

        if (this.MaxPriority.HasValue && this.MaxPriority.Value < 0)
            Validator.TryValidateProperty(this.MaxPriority, new ValidationContext(this, null, null) { MemberName = nameof(this.MaxPriority) }, results);

        if (this.OverflowRejectPublish.HasValue && this.OverflowRejectPublish.Value < 0)
            results.Add(new ValidationResult("The field OverflowRejectPublish must be greater than or equal to zero.", new[] { nameof(this.OverflowRejectPublish) }));

        ValidateExtraArguments(results);

        return results;
    }

    /// <summary>
    /// Validates the extra arguments for the queue.
    /// </summary>
    /// <param name="results">The list of validation results.</param>
    private void ValidateExtraArguments(List<ValidationResult> results)
    {
        if (this.ExtraArguments != null)
        {
            foreach (var item in this.ExtraArguments)
            {
                if (item.Value == null)
                    results.Add(new ValidationResult($"The field {item.Key} of the ExtraArguments cannot be null.", new[] { nameof(this.ExtraArguments) }));
            }
        }
    }
}