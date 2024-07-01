using CodeDesignPlus.Net.File.Storage.Abstractions.Providers;
using System.ComponentModel.DataAnnotations;

namespace CodeDesignPlus.Net.File.Storage.Abstractions.Options.Providers;

public class AzureBlobOptions : AzureOptions, IValidatableObject
{
    public static TypeProviders TypeProvider { get => TypeProviders.AzureBlobProvider; }


    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        return Validate();
    }
}
