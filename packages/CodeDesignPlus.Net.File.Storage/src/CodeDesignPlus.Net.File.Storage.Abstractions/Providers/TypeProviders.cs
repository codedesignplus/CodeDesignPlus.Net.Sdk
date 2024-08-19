namespace CodeDesignPlus.Net.File.Storage.Abstractions.Providers;

/// <summary>
/// Enum representing the different types of file storage providers.
/// </summary>
public enum TypeProviders
{
    /// <summary>
    /// No provider specified.
    /// </summary>
    None,

    /// <summary>
    /// Local file storage provider.
    /// </summary>
    LocalProvider,

    /// <summary>
    /// Azure File storage provider.
    /// </summary>
    AzureFileProvider,

    /// <summary>
    /// Azure Blob storage provider.
    /// </summary>
    AzureBlobProvider
}