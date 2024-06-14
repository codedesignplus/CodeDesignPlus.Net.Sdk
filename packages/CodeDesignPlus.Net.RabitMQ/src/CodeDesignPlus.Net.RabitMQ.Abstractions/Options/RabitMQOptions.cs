using System.ComponentModel.DataAnnotations;

namespace CodeDesignPlus.Net.RabitMQ.Abstractions.Options;

/// <summary>
/// Options to setting of the RabitMQ
/// </summary>
public class RabitMQOptions : IValidatableObject
{
    /// <summary>
    /// Name of the setions used in the appsettings
    /// </summary>
    public static readonly string Section = "RabitMQ";

    /// <summary>
    /// Get or sets the Enable
    /// </summary>
    public bool Enable { get; set; }
    [Required]
    public string Host { get; set; }

    public int Port { get; set; }

    [Required]
    public string UserName { get; set; }
    [Required]
    public string Password { get; set; }

    public bool ListenerEvents { get; set; }

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
            if (string.IsNullOrEmpty(UserName))
                results.Add(new ValidationResult($"The {nameof(this.UserName)} field is required."));

            Validator.TryValidateProperty(
                this.UserName,
                new ValidationContext(this, null, null) { MemberName = nameof(this.UserName) },
                results
            );
        }

        return results;
    }
}
