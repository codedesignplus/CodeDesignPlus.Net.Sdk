using System.ComponentModel.DataAnnotations;

namespace CodeDesignPlus.Net.File.Storage.Abstractions.Options.Providers;

public abstract class AzureOptions
{
    public bool Enable { get; set; }
    public string DefaultEndpointsProtocol { get; set; }
    public string AccountName { get; set; }
    public string AccountKey { get; set; }
    public bool UsePasswordLess { get; set; }
    public string EndpointSuffix { get; set; }
    public Uri Uri { get; set; }

    public string ConnectionString
    {
        get
        {
            return $"DefaultEndpointsProtocol={DefaultEndpointsProtocol};AccountName={AccountName};AccountKey={AccountKey};EndpointSuffix={EndpointSuffix}";
        }
    }


    protected IEnumerable<ValidationResult> Validate()
    {
        if (!Enable)
            yield break;

        if (!UsePasswordLess)
        {
            if (string.IsNullOrEmpty(DefaultEndpointsProtocol))
                yield return new ValidationResult("The DefaultEndpointsProtocol is required", new[] { nameof(DefaultEndpointsProtocol) });

            if (string.IsNullOrEmpty(AccountName))
                yield return new ValidationResult("The AccountName is required", new[] { nameof(AccountName) });

            if (string.IsNullOrEmpty(AccountKey))
                yield return new ValidationResult("The AccountKey is required", new[] { nameof(AccountKey) });

            if (string.IsNullOrEmpty(EndpointSuffix))
                yield return new ValidationResult("The EndpointSuffix is required", new[] { nameof(EndpointSuffix) });
        }
        else if (Uri is null)
            yield return new ValidationResult("The Uri is required", new[] { nameof(Uri) });
    }

}
