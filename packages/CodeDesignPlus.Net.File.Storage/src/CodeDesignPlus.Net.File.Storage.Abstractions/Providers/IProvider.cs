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
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task<Response> UploadAsync(Stream stream, string filename, string target, bool renowned = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Downloads a file from the storage provider.
    /// </summary>
    /// <param name="filename">The name of the file to download.</param>
    /// <param name="target">The target directory.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task<Response> DownloadAsync(string filename, string target, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a file from the storage provider.
    /// </summary>
    /// <param name="filename">The name of the file to delete.</param>
    /// <param name="target">The target directory.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task<Response> DeleteAsync(string filename, string target, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a signed URL for downloading a file.
    /// </summary>
    /// <param name="file">The name of the file to download.</param>
    /// <param name="target">The target directory.</param>
    /// <param name="timeSpan">The time span for which the signed URL is valid.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task<Response> GetSignedUrlAsync(string file, string target, TimeSpan timeSpan, CancellationToken cancellationToken);
}