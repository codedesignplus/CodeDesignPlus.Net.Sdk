using System.ComponentModel.DataAnnotations;
using CodeDesignPlus.Net.File.Storage.Abstractions.Providers;

namespace CodeDesignPlus.Net.File.Storage.Abstractions;

public class LocalOptions : IValidatableObject
{
    public static TypeProviders TypeProvider { get => TypeProviders.LocalProvider; }
    public bool Enable { get; set; }
    public string Folder { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if(!this.Enable)
            yield break;
        
        if (string.IsNullOrWhiteSpace(this.Folder))
            yield return new ValidationResult("The Folder property is required.", new[] { nameof(this.Folder) });
    }
}
