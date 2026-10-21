namespace CodeDesignPlus.Net.File.Storage.Providers;

/// <summary>
/// Provides methods for interacting with Azure Blob Storage.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="AzureBlobProvider"/> class.
/// </remarks>
/// <param name="factory">The Azure Blob factory.</param>
/// <param name="logger">The logger instance.</param>
/// <param name="environment">The host environment.</param>
public class AzureBlobProvider(
    IAzureBlobFactory factory,
    ILogger<AzureBlobProvider> logger,
    IHostEnvironment environment
    ) : BaseProvider(logger, environment), IAzureBlobProvider
{
    private readonly IAzureBlobFactory factory = factory.Create();

    /// <summary>
    /// Downloads a file from Azure Blob Storage.
    /// </summary>
    /// <param name="filename">The name of the file to download.</param>
    /// <param name="target">The target directory.</param>
    /// <param name="tenant">The tenant identifier for storage isolation.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task<M.Response> DownloadAsync(string filename, string target, Guid tenant, CancellationToken cancellationToken = default)
    {
        return base.ProcessAsync(factory.Options.AzureBlob.Enable, filename, TypeProviders.AzureBlobProvider, async (file, response) =>
        {
            var name = GetName(target, filename);

            var blobClient = this.factory.GetContainerClient(tenant).GetBlobClient(name);

            if (!await blobClient.ExistsAsync(cancellationToken).ConfigureAwait(false))
            {
                response.Success = false;
                response.Message = $"The file {filename} does not exist in the container {tenant}";

                return response;
            }

            var stream = new MemoryStream();

            await blobClient.DownloadToAsync(stream, cancellationToken: cancellationToken).ConfigureAwait(false);

            response.Stream = stream;
            response.Stream.Position = 0;
            response.Success = true;

            return response;
        });
    }

    /// <summary>
    /// Uploads a file to Azure Blob Storage.
    /// </summary>
    /// <param name="stream">The file stream.</param>
    /// <param name="filename">The name of the file.</param>
    /// <param name="target">The target directory.</param>
    /// <param name="renowned">Whether to rename the file if it already exists.</param>
    /// <param name="tenant">The tenant identifier for storage isolation.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task<M.Response> UploadAsync(Stream stream, string filename, string target, bool renowned, Guid tenant, CancellationToken cancellationToken = default)
    {
        return base.ProcessAsync(factory.Options.AzureBlob.Enable, filename, TypeProviders.AzureBlobProvider, async (file, response) =>
        {
            var container = this.factory.GetContainerClient(tenant);

            await container.CreateIfNotExistsAsync(cancellationToken: cancellationToken).ConfigureAwait(false);

            var name = GetName(target, System.IO.Path.GetFileName(file.FullName));

            var blobClient = container.GetBlobClient(name);

            if (renowned)
            {
                var count = 1;

                while (await blobClient.ExistsAsync(cancellationToken).ConfigureAwait(false))
                {
                    count += 1;

                    file.Renowned = true;
                    file.Version = SemVersion.ParsedFrom(count, 0, 0);

                    name = GetName(target, $"{file.Name} ({count}){file.Extension}");

                    blobClient = container.GetBlobClient(name);
                }
            }

            await blobClient.UploadAsync(stream, new BlobUploadOptions
            {
                AccessTier = AccessTier.Hot,
                Metadata = file.GetMetadata(this.factory.Options.UriDownload),
                Tags = file.GetTags(tenant),
                HttpHeaders = new BlobHttpHeaders
                {
                    ContentType = file.Mime.MimeType
                },
            }, cancellationToken: cancellationToken).ConfigureAwait(false);

            file.Size = stream.Length;
            file.Detail = new Abstractions.Models.FileDetail(this.factory.Options.UriDownload, target, System.IO.Path.GetFileName(name), TypeProviders.AzureBlobProvider);

            response.Success = true;

            return response;
        });
    }

    /// <summary>
    /// Gets a signed URL for downloading a file.
    /// </summary>
    /// <param name="filename">The name of the file to download.</param>
    /// <param name="target">The target directory.</param>
    /// <param name="timeSpan">The time span for which the signed URL is valid.</param>
    /// <param name="tenant">The tenant identifier for storage isolation.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task<M.Response> GetSignedUrlAsync(string filename, string target, TimeSpan timeSpan, Guid tenant, CancellationToken cancellationToken)
    {
        return base.ProcessAsync(factory.Options.AzureBlob.Enable, filename, TypeProviders.AzureBlobProvider, async (file, response) =>
        {
            var name = GetName(target, filename);

            var blobClient = this.factory.GetContainerClient(tenant).GetBlobClient(name);

            if (!await blobClient.ExistsAsync(cancellationToken).ConfigureAwait(false))
            {
                response.Success = false;
                response.Message = $"The file {filename} does not exist in the container {tenant}";

                return response;
            }

            var sasUri = blobClient.GenerateSasUri(Azure.Storage.Sas.BlobSasPermissions.Read, DateTimeOffset.UtcNow.Add(timeSpan));

            file.Detail = new Abstractions.Models.FileDetail(sasUri, DateTime.UtcNow.Add(timeSpan), target, filename, TypeProviders.AzureBlobProvider);

            response.Success = true;

            return response;
        });
    }

    /// <summary>
    /// Deletes the tenant's Blob container, with every file inside.
    /// </summary>
    /// <param name="tenant">The tenant identifier for storage isolation.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task<M.Response> DeleteTenantAsync(Guid tenant, CancellationToken cancellationToken = default)
    {
        return base.ProcessAsync(factory.Options.AzureBlob.Enable, tenant.ToString(), TypeProviders.AzureBlobProvider, async (file, response) =>
        {
            var deleted = await this.factory.GetContainerClient(tenant).DeleteIfExistsAsync(cancellationToken: cancellationToken).ConfigureAwait(false);

            // A tenant that never stored a file has no container: nothing to delete is still a success.
            response.Success = true;

            if (!deleted)
            {
                response.Message = $"The container {tenant} does not exist";
            }

            return response;
        });
    }

    /// <summary>
    /// Deletes a file from Azure Blob Storage.
    /// </summary>
    /// <param name="filename">The name of the file to delete.</param>
    /// <param name="target">The target directory.</param>
    /// <param name="tenant">The tenant identifier for storage isolation.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task<M.Response> DeleteAsync(string filename, string target, Guid tenant, CancellationToken cancellationToken = default)
    {
        return base.ProcessAsync(factory.Options.AzureBlob.Enable, filename, TypeProviders.AzureBlobProvider, async (file, response) =>
        {
            var container = this.factory.GetContainerClient(tenant);

            var name = GetName(target, filename);

            var blobClient = container.GetBlobClient(name);

            var deleted = await blobClient.DeleteIfExistsAsync(cancellationToken: cancellationToken).ConfigureAwait(false);

            response.Success = deleted;

            if (!deleted)
                response.Message = $"The file {filename} does not exist in the container {tenant}";

            return response;
        });
    }

    /// <summary>
    /// How long the read signature handed to the copy of an import lasts. The copy only has to start with it: Azure
    /// authorizes the source when it accepts the request.
    /// </summary>
    private static readonly TimeSpan ImportSourceSignatureLifetime = TimeSpan.FromHours(1);

    /// <summary>
    /// Copies a blob left in another container of the same account into the tenant's container, on the server side
    /// (Copy Blob): the content never passes through the process.
    /// </summary>
    /// <remarks>
    /// With an account key the source is read through a short read signature; with a token (password-less) the identity
    /// of the service must be able to read the source container. The copy keeps the source: it is deleted with
    /// <see cref="DeleteImportSourceAsync"/>.
    /// </remarks>
    /// <param name="sourceContainer">The container where the blob was left.</param>
    /// <param name="sourcePath">The name of the blob inside <paramref name="sourceContainer"/>.</param>
    /// <param name="filename">The name the file takes in the tenant's container.</param>
    /// <param name="target">The target directory.</param>
    /// <param name="tenant">The tenant identifier for storage isolation.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task<M.Response> ImportAsync(string sourceContainer, string sourcePath, string filename, string target, Guid tenant, CancellationToken cancellationToken = default)
    {
        return base.ProcessAsync(factory.Options.AzureBlob.Enable, filename, TypeProviders.AzureBlobProvider, async (file, response) =>
        {
            var source = this.factory.Client.GetBlobContainerClient(sourceContainer).GetBlobClient(sourcePath);

            if (!await source.ExistsAsync(cancellationToken).ConfigureAwait(false))
            {
                response.Success = false;
                response.Message = $"The file {sourcePath} does not exist in the container {sourceContainer}";

                return response;
            }

            var container = this.factory.GetContainerClient(tenant);

            await container.CreateIfNotExistsAsync(cancellationToken: cancellationToken).ConfigureAwait(false);

            var name = GetName(target, System.IO.Path.GetFileName(file.FullName));
            var destination = container.GetBlobClient(name);

            var sourceUri = source.CanGenerateSasUri
                ? source.GenerateSasUri(Azure.Storage.Sas.BlobSasPermissions.Read, DateTimeOffset.UtcNow.Add(ImportSourceSignatureLifetime))
                : source.Uri;

            var operation = await destination.StartCopyFromUriAsync(sourceUri, new BlobCopyFromUriOptions
            {
                AccessTier = AccessTier.Hot,
                Tags = file.GetTags(tenant),
            }, cancellationToken).ConfigureAwait(false);

            await operation.WaitForCompletionAsync(cancellationToken).ConfigureAwait(false);

            var properties = (await destination.GetPropertiesAsync(cancellationToken: cancellationToken).ConfigureAwait(false)).Value;

            if (properties.CopyStatus != CopyStatus.Success)
            {
                response.Success = false;
                response.Message = $"The copy of {sourcePath} from the container {sourceContainer} ended as {properties.CopyStatus}: {properties.CopyStatusDescription}";

                return response;
            }

            file.Size = properties.ContentLength;

            // The copy brings the headers and metadata of the source; the imported file gets the same ones as an upload.
            await destination.SetHttpHeadersAsync(new BlobHttpHeaders { ContentType = file.Mime?.MimeType ?? properties.ContentType }, cancellationToken: cancellationToken).ConfigureAwait(false);

            if (file.Mime is not null)
            {
                await destination.SetMetadataAsync(file.GetMetadata(this.factory.Options.UriDownload), cancellationToken: cancellationToken).ConfigureAwait(false);
            }

            file.Detail = new Abstractions.Models.FileDetail(this.factory.Options.UriDownload, target, System.IO.Path.GetFileName(name), TypeProviders.AzureBlobProvider);

            response.Success = true;

            return response;
        });
    }

    /// <summary>
    /// Deletes the blob an import was copied from. A blob that no longer exists is a success: the import was already done.
    /// </summary>
    /// <param name="sourceContainer">The container where the blob was left.</param>
    /// <param name="sourcePath">The name of the blob inside <paramref name="sourceContainer"/>.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task<M.Response> DeleteImportSourceAsync(string sourceContainer, string sourcePath, CancellationToken cancellationToken = default)
    {
        return base.ProcessAsync(factory.Options.AzureBlob.Enable, sourcePath, TypeProviders.AzureBlobProvider, async (file, response) =>
        {
            var source = this.factory.Client.GetBlobContainerClient(sourceContainer).GetBlobClient(sourcePath);

            var deleted = await source.DeleteIfExistsAsync(cancellationToken: cancellationToken).ConfigureAwait(false);

            response.Success = true;

            if (!deleted)
            {
                response.Message = $"The file {sourcePath} does not exist in the container {sourceContainer}";
            }

            return response;
        });
    }

    /// <summary>
    /// Constructs the full name of the file including the target directory.
    /// </summary>
    /// <param name="target">The target directory.</param>
    /// <param name="name">The name of the file.</param>
    /// <returns>The full name of the file.</returns>
    protected static string GetName(string target, string name)
    {
        if (string.IsNullOrEmpty(target))
            return name;

        return $"{target}/{name}";
    }
}
