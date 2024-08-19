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
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task<M.Response> DownloadAsync(string filename, string target, CancellationToken cancellationToken = default)
    {
        return base.ProcessAsync(factory.Options.AzureFile.Enable, filename, TypeProviders.AzureFileProvider, async (file, response) =>
        {
            var directory = this.factory.GetContainerClient().GetDirectoryClient(target);

            var fileClient = directory.GetFileClient(filename);

            if (!await fileClient.ExistsAsync(cancellationToken).ConfigureAwait(false))
            {
                response.Success = false;
                response.Message = $"The file {filename} does not exist in the container {this.factory.UserContext.Tenant}";

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
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task<M.Response> UploadAsync(Stream stream, string filename, string target, bool renowned = false, CancellationToken cancellationToken = default)
    {
        return base.ProcessAsync(factory.Options.AzureFile.Enable, filename, TypeProviders.AzureFileProvider, async (file, response) =>
        {
            var sharedClient = this.factory.GetContainerClient();

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

            await fileClient.CreateAsync(stream.Length, metadata: file.GetMetadata(this.factory.Options.UriDownload), cancellationToken: cancellationToken).ConfigureAwait(false);

            await fileClient.UploadAsync(stream, cancellationToken: cancellationToken).ConfigureAwait(false);

            file.Size = stream.Length;
            file.Detail = new M.FileDetail(this.factory.Options.UriDownload, target, name, TypeProviders.AzureFileProvider);

            response.Success = true;

            return response;
        });
    }

    /// <summary>
    /// Deletes a file from Azure File Storage.
    /// </summary>
    /// <param name="filename">The name of the file to delete.</param>
    /// <param name="target">The target directory.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task<M.Response> DeleteAsync(string filename, string target, CancellationToken cancellationToken = default)
    {
        return base.ProcessAsync(factory.Options.AzureFile.Enable, filename, TypeProviders.AzureFileProvider, async (file, response) =>
        {
            var directory = this.factory.GetContainerClient().GetDirectoryClient(target);

            var fileClient = directory.GetFileClient(filename);

            var deleted = await fileClient.DeleteIfExistsAsync(cancellationToken: cancellationToken).ConfigureAwait(false);

            response.Success = deleted;

            if (!deleted)
                response.Message = $"The file {filename} does not exist in the container {this.factory.UserContext.Tenant}";

            return response;
        });
    }
}