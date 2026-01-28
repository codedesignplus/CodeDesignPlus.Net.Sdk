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
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task<M.Response> DownloadAsync(string filename, string target, CancellationToken cancellationToken = default)
    {
        return base.ProcessAsync(factory.Options.AzureBlob.Enable, filename, TypeProviders.AzureBlobProvider, async (file, response) =>
        {
            var name = GetName(target, filename);

            var blobClient = this.factory.GetContainerClient().GetBlobClient(name);

            if (!await blobClient.ExistsAsync(cancellationToken).ConfigureAwait(false))
            {
                response.Success = false;
                response.Message = $"The file {filename} does not exist in the container {this.factory.UserContext.Tenant}";

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
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task<M.Response> UploadAsync(Stream stream, string filename, string target, bool renowned = false, CancellationToken cancellationToken = default)
    {
        return base.ProcessAsync(factory.Options.AzureBlob.Enable, filename, TypeProviders.AzureBlobProvider, async (file, response) =>
        {
            var container = this.factory.GetContainerClient();

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
                Tags = file.GetTags(this.factory.UserContext.Tenant),
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
    /// <param name="file">The name of the file to download.</param>
    /// <param name="target">The target directory.</param>
    /// <param name="timeSpan">The time span for which the signed URL is valid.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task<M.Response> GetSignedUrlAsync(string filename, string target, TimeSpan timeSpan, CancellationToken cancellationToken)
    {
        return base.ProcessAsync(factory.Options.AzureBlob.Enable, filename, TypeProviders.AzureBlobProvider, async (file, response) =>
        {
            var name = GetName(target, filename);

            var blobClient = this.factory.GetContainerClient().GetBlobClient(name);

            if (!await blobClient.ExistsAsync(cancellationToken).ConfigureAwait(false))
            {
                response.Success = false;
                response.Message = $"The file {file} does not exist in the container {this.factory.UserContext.Tenant}";

                return response;
            }

            var sasUri = blobClient.GenerateSasUri(Azure.Storage.Sas.BlobSasPermissions.Read, DateTimeOffset.UtcNow.AddMinutes(timeSpan.Minutes));

            file.Detail = new Abstractions.Models.FileDetail(sasUri, DateTime.UtcNow.AddMinutes(timeSpan.Minutes), target, filename, TypeProviders.AzureBlobProvider);

            response.Success = true;

            return response;
        });
    }

    /// <summary>
    /// Deletes a file from Azure Blob Storage.
    /// </summary>
    /// <param name="filename">The name of the file to delete.</param>
    /// <param name="target">The target directory.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task<M.Response> DeleteAsync(string filename, string target, CancellationToken cancellationToken = default)
    {
        return base.ProcessAsync(factory.Options.AzureBlob.Enable, filename, TypeProviders.AzureBlobProvider, async (file, response) =>
        {
            var container = this.factory.GetContainerClient();

            var name = GetName(target, filename);

            var blobClient = container.GetBlobClient(name);

            var deleted = await blobClient.DeleteIfExistsAsync(cancellationToken: cancellationToken).ConfigureAwait(false);

            response.Success = deleted;

            if (!deleted)
                response.Message = $"The file {filename} does not exist in the container {this.factory.UserContext.Tenant}";

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