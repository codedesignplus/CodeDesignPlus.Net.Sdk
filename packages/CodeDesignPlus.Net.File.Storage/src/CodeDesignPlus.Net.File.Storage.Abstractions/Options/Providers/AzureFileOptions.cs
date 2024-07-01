using CodeDesignPlus.Net.File.Storage.Abstractions.Providers;
using System.ComponentModel.DataAnnotations;

namespace CodeDesignPlus.Net.File.Storage.Abstractions.Options.Providers;

public class AzureFileOptions : AzureOptions, IValidatableObject
{
    public static TypeProviders TypeProvider { get => TypeProviders.AzureFileProvider; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        return Validate();
    }

}
