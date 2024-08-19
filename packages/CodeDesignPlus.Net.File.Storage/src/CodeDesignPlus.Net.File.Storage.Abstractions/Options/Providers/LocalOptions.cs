namespace CodeDesignPlus.Net.File.Storage.Abstractions.Options.Providers;

/// <summary>
/// Options for configuring local file storage.
/// </summary>
public class LocalOptions : IValidatableObject
{
    /// <summary>
    /// Gets the type of provider.
    /// </summary>
    public static TypeProviders TypeProvider { get => TypeProviders.LocalProvider; }

    /// <summary>
    /// Gets or sets a value indicating whether the provider is enabled.
    /// </summary>
    public bool Enable { get; set; }

    /// <summary>
    /// Gets or sets the folder path for local storage.
    /// </summary>
    public string Folder { get; set; }

    /// <summary>
    /// Validates the options.
    /// </summary>
    /// <param name="validationContext">The context of the validation.</param>
    /// <returns>A collection that holds failed-validation information.</returns>
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!Enable)
            yield break;

        if (string.IsNullOrWhiteSpace(Folder))
            yield return new ValidationResult("The Folder property is required.", new[] { nameof(Folder) });
    }
}