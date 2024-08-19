namespace CodeDesignPlus.Net.File.Storage.Abstractions;
/// <summary>
/// Interface for file storage services.
/// </summary>
public interface IFileStorageService
{
    /// <summary>
    /// Uploads a file to the storage service.
    /// </summary>
    /// <param name="stream">The file stream.</param>
    /// <param name="file">The name of the file.</param>
    /// <param name="target">The target directory.</param>
    /// <param name="renowned">Whether to rename the file if it already exists.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task<Response[]> UploadAsync(Stream stream, string file, string target, bool renowned, CancellationToken cancellationToken = default);

    /// <summary>
    /// Downloads a file from the storage service.
    /// </summary>
    /// <param name="file">The name of the file to download.</param>
    /// <param name="target">The target directory.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task<Response> DownloadAsync(string file, string target, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a file from the storage service.
    /// </summary>
    /// <param name="file">The name of the file to delete.</param>
    /// <param name="target">The target directory.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task<Response[]> DeleteAsync(string file, string target, CancellationToken cancellationToken = default);
}