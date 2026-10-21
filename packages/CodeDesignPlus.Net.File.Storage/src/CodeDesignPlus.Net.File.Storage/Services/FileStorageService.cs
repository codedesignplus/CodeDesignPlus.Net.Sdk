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
    /// Deletes every file stored for the tenant in all providers.
    /// </summary>
    /// <param name="tenant">The tenant identifier for storage isolation. It cannot be empty.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The responses of the enabled providers; a disabled provider does not answer.</returns>
    /// <exception cref="FileStorageException">Thrown when <paramref name="tenant"/> is empty.</exception>
    public async Task<M.Response[]> DeleteTenantAsync(Guid tenant, CancellationToken cancellationToken = default)
    {
        // An empty tenant is a bug upstream, never a request to wipe the storage.
        if (tenant == Guid.Empty)
        {
            throw new FileStorageException("Tenant cannot be empty.");
        }

        var tasks = providers.Select(provider => provider.DeleteTenantAsync(tenant, cancellationToken));

        var responses = await Task.WhenAll(tasks).ConfigureAwait(false);

        // A disabled provider returns null. Handing those nulls to the caller made every consumer that reads
        // Success throw, and the purge of a tenant was retried until it reached the dead-letter queue.
        return responses.Where(response => response is not null).ToArray();
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

    /// <summary>
    /// Copies a file left in a container of the same storage into the tenant's storage, in every provider.
    /// </summary>
    /// <param name="sourceContainer">The container where the file was left.</param>
    /// <param name="sourcePath">The path of the file inside <paramref name="sourceContainer"/>.</param>
    /// <param name="file">The name the file takes in the tenant's storage.</param>
    /// <param name="target">The target directory.</param>
    /// <param name="tenant">The tenant identifier for storage isolation. It cannot be empty.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The responses of the enabled providers; a disabled provider does not answer.</returns>
    /// <exception cref="FileStorageException">Thrown when the tenant, the source container or the source path is empty.</exception>
    public async Task<M.Response[]> ImportAsync(string sourceContainer, string sourcePath, string file, string target, Guid tenant, CancellationToken cancellationToken = default)
    {
        if (tenant == Guid.Empty)
        {
            throw new FileStorageException("Tenant cannot be empty.");
        }

        GuardSource(sourceContainer, sourcePath);

        var tasks = providers.Select(provider => provider.ImportAsync(sourceContainer, sourcePath, file, target, tenant, cancellationToken));

        var responses = await Task.WhenAll(tasks).ConfigureAwait(false);

        return responses.Where(response => response is not null).ToArray();
    }

    /// <summary>
    /// Deletes the source of an import in every provider.
    /// </summary>
    /// <param name="sourceContainer">The container where the file was left.</param>
    /// <param name="sourcePath">The path of the file inside <paramref name="sourceContainer"/>.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The responses of the enabled providers; a disabled provider does not answer.</returns>
    /// <exception cref="FileStorageException">Thrown when the source container or the source path is empty.</exception>
    public async Task<M.Response[]> DeleteImportSourceAsync(string sourceContainer, string sourcePath, CancellationToken cancellationToken = default)
    {
        GuardSource(sourceContainer, sourcePath);

        var tasks = providers.Select(provider => provider.DeleteImportSourceAsync(sourceContainer, sourcePath, cancellationToken));

        var responses = await Task.WhenAll(tasks).ConfigureAwait(false);

        return responses.Where(response => response is not null).ToArray();
    }

    /// <summary>
    /// An import without a source container or path would read from the root of the storage, never what was asked.
    /// </summary>
    private static void GuardSource(string sourceContainer, string sourcePath)
    {
        if (string.IsNullOrWhiteSpace(sourceContainer))
        {
            throw new FileStorageException("Source container cannot be empty.");
        }

        if (string.IsNullOrWhiteSpace(sourcePath))
        {
            throw new FileStorageException("Source path cannot be empty.");
        }

        if (sourcePath.Split('/', '\\').Contains(".."))
        {
            throw new FileStorageException("Source path cannot leave its container.");
        }
    }
}
