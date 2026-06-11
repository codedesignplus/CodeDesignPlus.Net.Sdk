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
    /// Gets the Blob service client.
    /// </summary>
    BlobServiceClient Client { get; }

    /// <summary>
    /// Creates and initializes the Blob service client if it is not already created.
    /// </summary>
    /// <returns>The current instance of <see cref="IAzureBlobFactory"/>.</returns>
    IAzureBlobFactory Create();

    /// <summary>
    /// Gets the Blob container client for the specified tenant.
    /// </summary>
    /// <param name="tenant">The tenant identifier.</param>
    /// <returns>The Blob container client.</returns>
    BlobContainerClient GetContainerClient(Guid tenant);
}