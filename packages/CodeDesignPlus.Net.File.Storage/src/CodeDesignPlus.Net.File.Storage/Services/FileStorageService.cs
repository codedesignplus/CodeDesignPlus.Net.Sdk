namespace CodeDesignPlus.Net.File.Storage.Services;

/// <summary>
/// Service for handling file storage operations.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="FileStorageService"/> class.
/// </remarks>
/// <param name="providers">The collection of file storage providers.</param>
public class FileStorageService(IEnumerable<IProvider> providers) : IFileStorage
{
    private readonly IEnumerable<IProvider> providers = providers;

    /// <summary>
    /// Deletes a file from all providers.
    /// </summary>
    /// <param name="file">The name of the file to delete.</param>
    /// <param name="target">The target directory.</param>
    /// <param name="tenant">The tenant identifier for storage isolation.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task<M.Response[]> DeleteAsync(string file, string target, Guid tenant, CancellationToken cancellationToken = default)
    {
        if (tenant == Guid.Empty)
            throw new FileStorageException("Tenant cannot be empty.");

        var tasks = new List<Task<M.Response>>();

        foreach (var provider in providers)
        {
            var task = provider.DeleteAsync(file, target, tenant, cancellationToken);
            tasks.Add(task);
        }

        return Task.WhenAll(tasks);
    }

    /// <summary>
    /// Downloads a file from the first provider where it exists.
    /// </summary>
    /// <param name="file">The name of the file to download.</param>
    /// <param name="target">The target directory.</param>
    /// <param name="tenant">The tenant identifier for storage isolation.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task<M.Response> DownloadAsync(string file, string target, Guid tenant, CancellationToken cancellationToken = default)
    {
        if (tenant == Guid.Empty)
            throw new FileStorageException("Tenant cannot be empty.");

        foreach (var provider in providers)
        {
            var response = await provider.DownloadAsync(file, target, tenant, cancellationToken).ConfigureAwait(false);

            if (response != null && response.Success)
                return response;
        }

        return null;
    }

    /// <summary>
    /// Gets a signed URL for downloading a file.
    /// </summary>
    /// <param name="file">The name of the file to download.</param>
    /// <param name="target">The target directory.</param>
    /// <param name="timeSpan">The time span for which the signed URL is valid.</param>
    /// <param name="tenant">The tenant identifier for storage isolation.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task<M.Response> GetSignedUrlAsync(string file, string target, TimeSpan timeSpan, Guid tenant, CancellationToken cancellationToken = default)
    {
        if (tenant == Guid.Empty)
            throw new FileStorageException("Tenant cannot be empty.");

        foreach (var provider in providers)
        {
            var response = await provider.GetSignedUrlAsync(file, target, timeSpan, tenant, cancellationToken).ConfigureAwait(false);

            if (response != null && response.Success)
                return response;
        }

        return null;
    }

    /// <summary>
    /// Uploads a file to all providers.
    /// </summary>
    /// <param name="stream">The file stream.</param>
    /// <param name="file">The name of the file.</param>
    /// <param name="target">The target directory.</param>
    /// <param name="renowned">Whether to rename the file if it already exists.</param>
    /// <param name="tenant">The tenant identifier for storage isolation.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task<M.Response[]> UploadAsync(Stream stream, string file, string target, bool renowned, Guid tenant, CancellationToken cancellationToken = default)
    {
        if (tenant == Guid.Empty)
            throw new FileStorageException("Tenant cannot be empty.");

        var tasks = new List<Task<M.Response>>();

        foreach (var provider in providers)
        {
            var memory = new MemoryStream();
            stream.CopyTo(memory);
            memory.Position = 0;
            stream.Position = 0;

            var task = provider.UploadAsync(memory, file, target, renowned, tenant, cancellationToken);

            tasks.Add(task);
        }

        return Task.WhenAll(tasks);
    }
}
