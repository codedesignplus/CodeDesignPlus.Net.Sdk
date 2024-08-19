namespace CodeDesignPlus.Net.File.Storage.Abstractions.Options.Providers;

/// <summary>
/// Abstract base class for Azure storage options.
/// </summary>
public abstract class AzureOptions
{
    /// <summary>
    /// Gets or sets a value indicating whether the provider is enabled.
    /// </summary>
    public bool Enable { get; set; }

    /// <summary>
    /// Gets or sets the default endpoints protocol.
    /// </summary>
    public string DefaultEndpointsProtocol { get; set; }

    /// <summary>
    /// Gets or sets the account name.
    /// </summary>
    public string AccountName { get; set; }

    /// <summary>
    /// Gets or sets the account key.
    /// </summary>
    public string AccountKey { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to use password-less authentication.
    /// </summary>
    public bool UsePasswordLess { get; set; }

    /// <summary>
    /// Gets or sets the endpoint suffix.
    /// </summary>
    public string EndpointSuffix { get; set; }

    /// <summary>
    /// Gets or sets the URI for password-less authentication.
    /// </summary>
    public Uri Uri { get; set; }

    /// <summary>
    /// Gets the connection string for the Azure storage account.
    /// </summary>
    public string ConnectionString
    {
        get
        {
            return $"DefaultEndpointsProtocol={DefaultEndpointsProtocol};AccountName={AccountName};AccountKey={AccountKey};EndpointSuffix={EndpointSuffix}";
        }
    }

    /// <summary>
    /// Validates the options.
    /// </summary>
    /// <returns>A collection that holds failed-validation information.</returns>
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