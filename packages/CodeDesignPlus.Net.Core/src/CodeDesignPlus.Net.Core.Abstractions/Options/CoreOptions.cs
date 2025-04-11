namespace CodeDesignPlus.Net.Core.Abstractions.Options;

/// <summary>
/// Options to setting of the Core
/// </summary>
public class CoreOptions : IValidatableObject
{
    /// <summary>
    /// Name of the setions used in the appsettings
    /// </summary>
    public static readonly string Section = "Core";
    /// <summary>
    /// Gets or sets the Id of the microservice
    /// </summary>
    [Required]
    public Guid Id { get; set; }
    /// <summary>
    /// Gets or sets the business
    /// </summary>
    [Required]
    public required string Business { get; set; }
    /// <summary>
    /// Gets or sets the name
    /// </summary>
    [Required]
    [RegularExpression(@"^[a-z-]+$")]
    public required string AppName { get; set; }
    /// <summary>
    /// Gets or sets the version
    /// </summary>
    [Required]
    [RegularExpression(@"^v\d+$")]
    public required string Version { get; set; }
    /// <summary>
    /// Gets or sets the description
    /// </summary>
    [Required]
    public required string Description { get; set; }
    /// <summary>
    /// Gets or sets the contact information
    /// </summary>
    [Required]
    public required Contact Contact { get; set; }

    /// <summary>
    /// Validate the properties of the class
    /// </summary>
    /// <param name="validationContext">The context of the validation</param>
    /// <returns> A collection that holds failed-validation information.</returns>
    /// <exception cref="NotImplementedException"></exception>
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var results = new List<ValidationResult>();

        if (this.Contact is null)
        {
            results.Add(new ValidationResult($"The {nameof(this.Contact)} field is required."));
        }
        else
        {
            var context = new ValidationContext(this.Contact, serviceProvider: null, items: null);

            Validator.TryValidateObject(this.Contact, context, results, validateAllProperties: true);
        }

        return results;
    }
}

