namespace CodeDesignPlus.Net.File.Storage.Factories;

/// <summary>
/// Factory class for creating and managing Azure Blob Storage clients.
/// </summary>
public class AzureBlobFactory : IAzureBlobFactory
{
    /// <summary>
    /// Gets the file storage options.
    /// </summary>
    public FileStorageOptions Options { get; private set; }

    /// <summary>
    /// Gets the Blob service client.
    /// </summary>
    public BlobServiceClient Client { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AzureBlobFactory"/> class.
    /// </summary>
    /// <param name="options">The file storage options.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="options"/> is null.
    /// </exception>
    public AzureBlobFactory(IOptions<FileStorageOptions> options)
    {
        ArgumentNullException.ThrowIfNull(options);

        Options = options.Value;
    }

    /// <summary>
    /// Creates and initializes the Blob service client if it is not already created.
    /// </summary>
    /// <returns>The current instance of <see cref="AzureBlobFactory"/>.</returns>
    /// <exception cref="FileStorageException">
    /// Thrown when Azure Blob storage is not enabled.
    /// </exception>
    public IAzureBlobFactory Create()
    {
        if (!this.Options.AzureBlob.Enable)
            return this;

        if (this.Client != null)
            return this;

        if (this.Options.AzureBlob.UsePasswordLess)
            this.Client = new BlobServiceClient(this.Options.AzureBlob.Uri, new DefaultAzureCredential());
        else
            this.Client = new BlobServiceClient(this.Options.AzureBlob.ConnectionString);

        return this;
    }

    /// <summary>
    /// Gets the Blob container client for the specified tenant.
    /// </summary>
    /// <param name="tenant">The tenant identifier.</param>
    /// <returns>The Blob container client.</returns>
    public BlobContainerClient GetContainerClient(Guid tenant)
    {
        return this.Client.GetBlobContainerClient(tenant.ToString());
    }
}
