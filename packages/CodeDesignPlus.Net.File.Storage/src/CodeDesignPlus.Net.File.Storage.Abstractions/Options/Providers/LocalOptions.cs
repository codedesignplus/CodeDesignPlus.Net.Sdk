using CodeDesignPlus.Net.File.Storage.Abstractions.Providers;
using System.ComponentModel.DataAnnotations;

namespace CodeDesignPlus.Net.File.Storage.Abstractions.Options.Providers;

public class LocalOptions : IValidatableObject
{
    public static TypeProviders TypeProvider { get => TypeProviders.LocalProvider; }
    public bool Enable { get; set; }
    public string Folder { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!Enable)
            yield break;

        if (string.IsNullOrWhiteSpace(Folder))
            yield return new ValidationResult("The Folder property is required.", new[] { nameof(Folder) });
    }
}
