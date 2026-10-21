namespace CodeDesignPlus.Net.File.Storage.Abstractions.Providers;

/// <summary>
/// Interface for file storage providers.
/// </summary>
public interface IProvider
{
    /// <summary>
    /// Uploads a file to the storage provider.
    /// </summary>
    /// <param name="stream">The file stream.</param>
    /// <param name="filename">The name of the file.</param>
    /// <param name="target">The target directory.</param>
    /// <param name="renowned">Whether to rename the file if it already exists.</param>
    /// <param name="tenant">The tenant identifier for storage isolation.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task<Response> UploadAsync(Stream stream, string filename, string target, bool renowned, Guid tenant, CancellationToken cancellationToken = default);

    /// <summary>
    /// Downloads a file from the storage provider.
    /// </summary>
    /// <param name="filename">The name of the file to download.</param>
    /// <param name="target">The target directory.</param>
    /// <param name="tenant">The tenant identifier for storage isolation.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task<Response> DownloadAsync(string filename, string target, Guid tenant, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a file from the storage provider.
    /// </summary>
    /// <param name="filename">The name of the file to delete.</param>
    /// <param name="target">The target directory.</param>
    /// <param name="tenant">The tenant identifier for storage isolation.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task<Response> DeleteAsync(string filename, string target, Guid tenant, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a signed URL for downloading a file.
    /// </summary>
    /// <param name="file">The name of the file to download.</param>
    /// <param name="target">The target directory.</param>
    /// <param name="timeSpan">The time span for which the signed URL is valid.</param>
    /// <param name="tenant">The tenant identifier for storage isolation.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task<Response> GetSignedUrlAsync(string file, string target, TimeSpan timeSpan, Guid tenant, CancellationToken cancellationToken);

    /// <summary>
    /// Deletes every file stored for the tenant: its container, share or folder, with everything inside.
    /// </summary>
    /// <param name="tenant">The tenant identifier for storage isolation.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task<Response> DeleteTenantAsync(Guid tenant, CancellationToken cancellationToken = default);

    /// <summary>
    /// Copies a file that another system left in a container of the same storage into the tenant's storage, without
    /// passing its content through the process: the provider copies it on the server side.
    /// </summary>
    /// <remarks>
    /// The source is kept: it is deleted with <see cref="DeleteImportSourceAsync"/> once the caller has registered the
    /// imported file, so a failure in between can be retried. Importing the same file twice overwrites the copy.
    /// </remarks>
    /// <param name="sourceContainer">The container (Blob), share (Azure File) or folder (local) where the file was left.</param>
    /// <param name="sourcePath">The path of the file inside <paramref name="sourceContainer"/>.</param>
    /// <param name="filename">The name the file takes in the tenant's storage.</param>
    /// <param name="target">The target directory.</param>
    /// <param name="tenant">The tenant identifier for storage isolation.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task<Response> ImportAsync(string sourceContainer, string sourcePath, string filename, string target, Guid tenant, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes the source of an import once the imported file is registered. A source that no longer exists is a success.
    /// </summary>
    /// <param name="sourceContainer">The container (Blob), share (Azure File) or folder (local) where the file was left.</param>
    /// <param name="sourcePath">The path of the file inside <paramref name="sourceContainer"/>.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task<Response> DeleteImportSourceAsync(string sourceContainer, string sourcePath, CancellationToken cancellationToken = default);
}