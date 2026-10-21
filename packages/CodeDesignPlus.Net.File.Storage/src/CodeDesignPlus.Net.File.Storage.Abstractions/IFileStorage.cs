namespace CodeDesignPlus.Net.File.Storage.Abstractions;
/// <summary>
/// Interface for file storage services.
/// </summary>
public interface IFileStorage
{
    /// <summary>
    /// Uploads a file to the storage service.
    /// </summary>
    /// <param name="stream">The file stream.</param>
    /// <param name="file">The name of the file.</param>
    /// <param name="target">The target directory.</param>
    /// <param name="renowned">Whether to rename the file if it already exists.</param>
    /// <param name="tenant">The tenant identifier for storage isolation.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task<Response[]> UploadAsync(Stream stream, string file, string target, bool renowned, Guid tenant, CancellationToken cancellationToken = default);

    /// <summary>
    /// Downloads a file from the storage service.
    /// </summary>
    /// <param name="file">The name of the file to download.</param>
    /// <param name="target">The target directory.</param>
    /// <param name="tenant">The tenant identifier for storage isolation.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task<Response> DownloadAsync(string file, string target, Guid tenant, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a signed URL for downloading a file.
    /// </summary>
    /// <param name="file">The name of the file to download.</param>
    /// <param name="target">The target directory.</param>
    /// <param name="timeSpan">The time span for which the signed URL is valid.</param>
    /// <param name="tenant">The tenant identifier for storage isolation.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task<Response> GetSignedUrlAsync(string file, string target, TimeSpan timeSpan, Guid tenant, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a file from the storage service.
    /// </summary>
    /// <param name="file">The name of the file to delete.</param>
    /// <param name="target">The target directory.</param>
    /// <param name="tenant">The tenant identifier for storage isolation.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task<Response[]> DeleteAsync(string file, string target, Guid tenant, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes every file stored for the tenant in all providers, used to purge a tenant removed from the platform.
    /// </summary>
    /// <param name="tenant">The tenant identifier for storage isolation. It cannot be empty.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task<Response[]> DeleteTenantAsync(Guid tenant, CancellationToken cancellationToken = default);

    /// <summary>
    /// Copies a file that another system left in a container of the same storage into the tenant's storage, on the server
    /// side: the content never passes through the process, so its size is not limited by memory or by a message size.
    /// </summary>
    /// <remarks>
    /// The source is kept until <see cref="DeleteImportSourceAsync"/>: the caller registers the imported file first and then
    /// deletes the source, so a failure in between can be retried. An empty tenant, source container or source path is
    /// rejected with a <c>FileStorageException</c>.
    /// </remarks>
    /// <param name="sourceContainer">The container where the file was left. It is never a tenant container.</param>
    /// <param name="sourcePath">The path of the file inside <paramref name="sourceContainer"/>.</param>
    /// <param name="file">The name the file takes in the tenant's storage.</param>
    /// <param name="target">The target directory.</param>
    /// <param name="tenant">The tenant identifier for storage isolation.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The responses of the enabled providers; a disabled provider does not answer.</returns>
    Task<Response[]> ImportAsync(string sourceContainer, string sourcePath, string file, string target, Guid tenant, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes the source of an import in every provider once the imported file is registered.
    /// </summary>
    /// <remarks>
    /// An empty source container or source path is rejected with a <c>FileStorageException</c>.
    /// </remarks>
    /// <param name="sourceContainer">The container where the file was left.</param>
    /// <param name="sourcePath">The path of the file inside <paramref name="sourceContainer"/>.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The responses of the enabled providers; a disabled provider does not answer.</returns>
    Task<Response[]> DeleteImportSourceAsync(string sourceContainer, string sourcePath, CancellationToken cancellationToken = default);
}