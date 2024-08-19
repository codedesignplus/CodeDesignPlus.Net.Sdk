namespace CodeDesignPlus.Net.File.Storage.Abstractions.Factories;

/// <summary>
/// Factory class for creating and managing Azure Blob Storage clients.
/// </summary>
public interface IAzureBlobFactory
{
    /// <summary>
    /// Gets the file storage options.
    /// </summary>
    FileStorageOptions Options { get; }

    /// <summary>
    /// Gets the user context.
    /// </summary>
    IUserContext UserContext { get; }

    /// <summary>
    /// Gets the Blob service client.
    /// </summary>
    BlobServiceClient Client { get; }

    /// <summary>
    /// Creates and initializes the Blob service client if it is not already created.
    /// </summary>
    /// <returns>The current instance of <see cref="IAzureBlobFactory"/>.</returns>
    IAzureBlobFactory Create();

    /// <summary>
    /// Gets the Blob container client for the current tenant.
    /// </summary>
    /// <returns>The Blob container client.</returns>
    BlobContainerClient GetContainerClient();
}