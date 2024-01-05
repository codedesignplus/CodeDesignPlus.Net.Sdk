using System.ComponentModel.DataAnnotations;
using CodeDesignPlus.Net.File.Storage.Abstractions.Providers;
using Azure.Storage.Files.Shares;
using Azure.Identity;

namespace CodeDesignPlus.Net.File.Storage.Abstractions;

public class AzureFileOptions : IValidatableObject
{
    public static TypeProviders TypeProvider { get => TypeProviders.AzureFileProvider; }
    public bool Enable { get; set; }
    public string DefaultEndpointsProtocol { get; set; }
    public string AccountName { get; set; }
    public string AccountKey { get; set; }
    public string EndpointSuffix { get; set; }
    public bool UsePasswordLess { get; set; }
    public Uri Uri { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!this.Enable)
            yield break;

        if (!this.UsePasswordLess)
        {
            if (string.IsNullOrEmpty(this.DefaultEndpointsProtocol))
                yield return new ValidationResult("The DefaultEndpointsProtocol is required", new[] { nameof(this.DefaultEndpointsProtocol) });

            if (string.IsNullOrEmpty(this.AccountName))
                yield return new ValidationResult("The AccountName is required", new[] { nameof(this.AccountName) });

            if (string.IsNullOrEmpty(this.AccountKey))
                yield return new ValidationResult("The AccountKey is required", new[] { nameof(this.AccountKey) });

            if (string.IsNullOrEmpty(this.EndpointSuffix))
                yield return new ValidationResult("The EndpointSuffix is required", new[] { nameof(this.EndpointSuffix) });
        }
        else if (this.Uri is null)
            yield return new ValidationResult("The Uri is required", new[] { nameof(this.Uri) });
    }

    public string ConnectionString
    {
        get
        {
            return $"DefaultEndpointsProtocol={this.DefaultEndpointsProtocol};AccountName={this.AccountName};AccountKey={this.AccountKey};EndpointSuffix={this.EndpointSuffix}";
        }
    }
}
