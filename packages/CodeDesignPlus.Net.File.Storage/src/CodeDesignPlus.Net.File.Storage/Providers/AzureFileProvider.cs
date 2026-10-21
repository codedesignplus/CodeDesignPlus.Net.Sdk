namespace CodeDesignPlus.Net.File.Storage.Providers;

/// <summary>
/// Provides methods for interacting with Azure File Storage.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="AzureFileProvider"/> class.
/// </remarks>
/// <param name="factory">The Azure File factory.</param>
/// <param name="logger">The logger instance.</param>
/// <param name="environment">The host environment.</param>
public class AzureFileProvider(
    IAzureFileFactory factory,
    ILogger<AzureFileProvider> logger,
    IHostEnvironment environment
    ) : BaseProvider(logger, environment), IAzureFileProvider
{
    private readonly IAzureFileFactory factory = factory.Create();

    /// <summary>
    /// Downloads a file from Azure File Storage.
    /// </summary>
    /// <param name="filename">The name of the file to download.</param>
    /// <param name="target">The target directory.</param>
    /// <param name="tenant">The tenant identifier for storage isolation.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task<M.Response> DownloadAsync(string filename, string target, Guid tenant, CancellationToken cancellationToken = default)
    {
        return base.ProcessAsync(factory.Options.AzureFile.Enable, filename, TypeProviders.AzureFileProvider, async (file, response) =>
        {
            var directory = this.factory.GetContainerClient(tenant).GetDirectoryClient(target);

            var fileClient = directory.GetFileClient(filename);

            if (!await fileClient.ExistsAsync(cancellationToken).ConfigureAwait(false))
            {
                response.Success = false;
                response.Message = $"The file {filename} does not exist in the container {tenant}";

                return response;
            }

            var download = await fileClient.DownloadAsync(cancellationToken: cancellationToken).ConfigureAwait(false);

            response.Stream = download.Value.Content;
            response.Stream.Position = 0;
            response.Success = true;

            return response;
        });
    }

    /// <summary>
    /// Uploads a file to Azure File Storage.
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
        return base.ProcessAsync(factory.Options.AzureFile.Enable, filename, TypeProviders.AzureFileProvider, async (file, response) =>
        {
            var sharedClient = this.factory.GetContainerClient(tenant);

            await sharedClient.CreateIfNotExistsAsync(cancellationToken: cancellationToken).ConfigureAwait(false);

            var directory = sharedClient.GetDirectoryClient(target);

            if (!await directory.ExistsAsync(cancellationToken).ConfigureAwait(false))
            {
                var pathParts = target.Split('/');
                var currentPath = new StringBuilder();

                for (int i = 0; i < pathParts.Length; i++)
                {
                    currentPath.Append(pathParts[i]).Append('/');

                    directory = sharedClient.GetDirectoryClient(currentPath.ToString());

                    await directory.CreateIfNotExistsAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
                }
            }

            var name = System.IO.Path.GetFileName(file.FullName);
            var fileClient = directory.GetFileClient(name);

            if (renowned)
            {
                var count = 1;

                while (await fileClient.ExistsAsync(cancellationToken))
                {
                    count += 1;
                    file.Renowned = true;
                    file.Version = SemVersion.ParsedFrom(count, 0, 0);

                    name = $"{file.Name} ({count}){file.Extension}";

                    fileClient = directory.GetFileClient(name);
                }
            }

            await fileClient.CreateAsync(stream.Length, new Azure.Storage.Files.Shares.Models.ShareFileCreateOptions() { Metadata = file.GetMetadata(this.factory.Options.UriDownload) }, cancellationToken: cancellationToken).ConfigureAwait(false);

            await fileClient.UploadAsync(stream, cancellationToken: cancellationToken).ConfigureAwait(false);

            file.Size = stream.Length;
            file.Detail = new M.FileDetail(this.factory.Options.UriDownload, target, name, TypeProviders.AzureFileProvider);

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
        return base.ProcessAsync(factory.Options.AzureFile.Enable, filename, TypeProviders.AzureFileProvider, async (file, response) =>
         {
             var directory = this.factory.GetContainerClient(tenant).GetDirectoryClient(target);

             var fileClient = directory.GetFileClient(filename);

             if (!await fileClient.ExistsAsync(cancellationToken).ConfigureAwait(false))
             {
                 response.Success = false;
                 response.Message = $"The file {filename} does not exist in the container {tenant}";

                 return response;
             }

             var sasUri = fileClient.GenerateSasUri(Azure.Storage.Sas.ShareFileSasPermissions.Read, DateTimeOffset.UtcNow.Add(timeSpan));

             file.Detail = new Abstractions.Models.FileDetail(sasUri, DateTime.UtcNow.Add(timeSpan), target, filename, TypeProviders.AzureBlobProvider);

             response.Success = true;

             return response;
         });
    }

    /// <summary>
    /// Deletes a file from Azure File Storage.
    /// </summary>
    /// <param name="filename">The name of the file to delete.</param>
    /// <param name="target">The target directory.</param>
    /// <param name="tenant">The tenant identifier for storage isolation.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task<M.Response> DeleteAsync(string filename, string target, Guid tenant, CancellationToken cancellationToken = default)
    {
        return base.ProcessAsync(factory.Options.AzureFile.Enable, filename, TypeProviders.AzureFileProvider, async (file, response) =>
        {
            var directory = this.factory.GetContainerClient(tenant).GetDirectoryClient(target);

            var fileClient = directory.GetFileClient(filename);

            var deleted = await fileClient.DeleteIfExistsAsync(cancellationToken: cancellationToken).ConfigureAwait(false);

            response.Success = deleted;

            if (!deleted)
                response.Message = $"The file {filename} does not exist in the container {tenant}";

            return response;
        });
    }

    /// <summary>
    /// Deletes the tenant's file share, with every file inside.
    /// </summary>
    /// <param name="tenant">The tenant identifier for storage isolation.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task<M.Response> DeleteTenantAsync(Guid tenant, CancellationToken cancellationToken = default)
    {
        return base.ProcessAsync(factory.Options.AzureFile.Enable, tenant.ToString(), TypeProviders.AzureFileProvider, async (file, response) =>
        {
            var deleted = await this.factory.GetContainerClient(tenant).DeleteIfExistsAsync(cancellationToken: cancellationToken).ConfigureAwait(false);

            // A tenant that never stored a file has no share: nothing to delete is still a success.
            response.Success = true;

            if (!deleted)
            {
                response.Message = $"The share {tenant} does not exist";
            }

            return response;
        });
    }

    /// <summary>
    /// How long the read signature handed to the copy of an import lasts.
    /// </summary>
    private static readonly TimeSpan ImportSourceSignatureLifetime = TimeSpan.FromHours(1);

    /// <summary>
    /// How often the state of a pending copy is asked.
    /// </summary>
    private static readonly TimeSpan CopyPollingInterval = TimeSpan.FromSeconds(1);

    /// <summary>
    /// Copies a file left in another share of the same account into the tenant's share, on the server side: the content
    /// never passes through the process.
    /// </summary>
    /// <param name="sourceContainer">The share where the file was left.</param>
    /// <param name="sourcePath">The path of the file inside <paramref name="sourceContainer"/>.</param>
    /// <param name="filename">The name the file takes in the tenant's share.</param>
    /// <param name="target">The target directory.</param>
    /// <param name="tenant">The tenant identifier for storage isolation.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task<M.Response> ImportAsync(string sourceContainer, string sourcePath, string filename, string target, Guid tenant, CancellationToken cancellationToken = default)
    {
        return base.ProcessAsync(factory.Options.AzureFile.Enable, filename, TypeProviders.AzureFileProvider, async (file, response) =>
        {
            var source = GetSourceFile(sourceContainer, sourcePath);

            if (!await source.ExistsAsync(cancellationToken).ConfigureAwait(false))
            {
                response.Success = false;
                response.Message = $"The file {sourcePath} does not exist in the share {sourceContainer}";

                return response;
            }

            var share = this.factory.GetContainerClient(tenant);

            await share.CreateIfNotExistsAsync(cancellationToken: cancellationToken).ConfigureAwait(false);

            var directory = share.GetRootDirectoryClient();

            foreach (var part in target.Split('/', StringSplitOptions.RemoveEmptyEntries))
            {
                directory = directory.GetSubdirectoryClient(part);

                await directory.CreateIfNotExistsAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
            }

            var name = System.IO.Path.GetFileName(file.FullName);
            var destination = directory.GetFileClient(name);

            var sourceUri = source.CanGenerateSasUri
                ? source.GenerateSasUri(Azure.Storage.Sas.ShareFileSasPermissions.Read, DateTimeOffset.UtcNow.Add(ImportSourceSignatureLifetime))
                : source.Uri;

            await destination.StartCopyAsync(sourceUri, cancellationToken: cancellationToken).ConfigureAwait(false);

            var properties = (await destination.GetPropertiesAsync(cancellationToken: cancellationToken).ConfigureAwait(false)).Value;

            while (properties.CopyStatus == Azure.Storage.Files.Shares.Models.CopyStatus.Pending)
            {
                await Task.Delay(CopyPollingInterval, cancellationToken).ConfigureAwait(false);

                properties = (await destination.GetPropertiesAsync(cancellationToken: cancellationToken).ConfigureAwait(false)).Value;
            }

            if (properties.CopyStatus != Azure.Storage.Files.Shares.Models.CopyStatus.Success)
            {
                response.Success = false;
                response.Message = $"The copy of {sourcePath} from the share {sourceContainer} ended as {properties.CopyStatus}: {properties.CopyStatusDescription}";

                return response;
            }

            file.Size = properties.ContentLength;
            file.Detail = new M.FileDetail(this.factory.Options.UriDownload, target, name, TypeProviders.AzureFileProvider);

            response.Success = true;

            return response;
        });
    }

    /// <summary>
    /// Deletes the file an import was copied from. A file that no longer exists is a success: the import was already done.
    /// </summary>
    /// <param name="sourceContainer">The share where the file was left.</param>
    /// <param name="sourcePath">The path of the file inside <paramref name="sourceContainer"/>.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task<M.Response> DeleteImportSourceAsync(string sourceContainer, string sourcePath, CancellationToken cancellationToken = default)
    {
        return base.ProcessAsync(factory.Options.AzureFile.Enable, sourcePath, TypeProviders.AzureFileProvider, async (file, response) =>
        {
            var deleted = await GetSourceFile(sourceContainer, sourcePath).DeleteIfExistsAsync(cancellationToken: cancellationToken).ConfigureAwait(false);

            response.Success = true;

            if (!deleted)
            {
                response.Message = $"The file {sourcePath} does not exist in the share {sourceContainer}";
            }

            return response;
        });
    }

    /// <summary>
    /// The client of a file inside a share, walking its directories.
    /// </summary>
    private ShareFileClient GetSourceFile(string sourceContainer, string sourcePath)
    {
        var parts = sourcePath.Split('/', StringSplitOptions.RemoveEmptyEntries);
        var directory = this.factory.Client.GetShareClient(sourceContainer).GetRootDirectoryClient();

        foreach (var part in parts[..^1])
        {
            directory = directory.GetSubdirectoryClient(part);
        }

        return directory.GetFileClient(parts[^1]);
    }
}
