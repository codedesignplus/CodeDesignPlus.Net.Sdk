using System.ComponentModel.DataAnnotations;

namespace CodeDesignPlus.Net.RabbitMQ.Abstractions.Options;

public class QueueArguments : IValidatableObject
{
    public int MessageTtl { get; set; } = 172800000;
    public int? Expires { get; set; }
    public int? MaxLength { get; set; }
    public int? MaxLengthBytes { get; set; }
    [Range(0, 255)]
    public int? MaxPriority { get; set; }

    [RegularExpression(@"^(default|lazy)?$")]
    public string QueueMode { get; set; } = "lazy";
    [RegularExpression(@"^(min-masters)?$")]
    public string QueueMasterLocator { get; set; }
    [RegularExpression(@"^(all|exactly|nodes|nodes\[\d+\])?$")]
    public string HaPolicy { get; set; } = "all";
    [RegularExpression(@"^(drop-head|reject-publish)?$")]
    public string Overflow { get; set; }
    public int? OverflowRejectPublish { get; set; }
    public Dictionary<string, object> ExtraArguments { get; set; }

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

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var results = new List<ValidationResult>();

        if (this.MessageTtl < 0)
            results.Add(new ValidationResult("The field MessageTtl must be greater than or equal to zero.", [nameof(this.MessageTtl)]));

        if (this.Expires.HasValue && this.Expires.Value < 0)
            results.Add(new ValidationResult("The field Expires must be greater than or equal to zero.", [nameof(this.Expires)]));

        if (this.MaxLength.HasValue && this.MaxLength.Value < 0)
            results.Add(new ValidationResult("The field MaxLength must be greater than or equal to zero.", [nameof(this.MaxLength)]));

        if (this.MaxLengthBytes.HasValue && this.MaxLengthBytes.Value < 0)
            results.Add(new ValidationResult("The field MaxLengthBytes must be greater than or equal to zero.", [nameof(this.MaxLengthBytes)]));

        if (this.MaxPriority.HasValue && this.MaxPriority.Value < 0)
            Validator.TryValidateProperty(this.MaxPriority, new ValidationContext(this, null, null) { MemberName = nameof(this.MaxPriority) }, results);

        if (this.OverflowRejectPublish.HasValue && this.OverflowRejectPublish.Value < 0)
            results.Add(new ValidationResult("The field OverflowRejectPublish must be greater than or equal to zero.", [nameof(this.OverflowRejectPublish)]));
            
        ValidateExtraArguments(results);

        return results;
    }

    private void ValidateExtraArguments(List<ValidationResult> results)
    {
        if (this.ExtraArguments != null)
            foreach (var item in this.ExtraArguments)
            {
                if (item.Value == null)
                    results.Add(new ValidationResult($"The field {item.Key} of the ExtraArguments cannot be null.", [nameof(this.ExtraArguments)]));
            }
    }
}
