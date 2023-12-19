using System.ComponentModel.DataAnnotations;
using CodeDesignPlus.Net.File.Storage.Abstractions.Providers;
using Microsoft.Azure.Storage;

namespace CodeDesignPlus.Net.File.Storage.Abstractions;

public class AzureFileOptions: IValidatableObject
{
    public static TypeProviders TypeProvider { get => TypeProviders.AzureFileProvider; }
    public bool Enable { get; set; }
    public string DefaultEndpointsProtocol { get; set; }
    public string AccountName { get; set; }
    public string AccountKey { get; set; }
    public string Folder { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!this.Enable)
            yield break;

        if (string.IsNullOrEmpty(this.DefaultEndpointsProtocol))
            yield return new ValidationResult("The DefaultEndpointsProtocol is required", new[] { nameof(this.DefaultEndpointsProtocol) });

        if (string.IsNullOrEmpty(this.AccountName))
            yield return new ValidationResult("The AccountName is required", new[] { nameof(this.AccountName) });

        if (string.IsNullOrEmpty(this.AccountKey))
            yield return new ValidationResult("The AccountKey is required", new[] { nameof(this.AccountKey) });

        if (string.IsNullOrEmpty(this.Folder))
            yield return new ValidationResult("The Folder is required", new[] { nameof(this.Folder) });
    }

    public override string ToString()
    {
        if (!this.Enable)
            return default;

        return $"DefaultEndpointsProtocol={this.DefaultEndpointsProtocol};AccountName={this.AccountName};AccountKey={this.AccountKey}";
    }

    private CloudStorageAccount Account => CloudStorageAccount.Parse(this.ToString());

}
