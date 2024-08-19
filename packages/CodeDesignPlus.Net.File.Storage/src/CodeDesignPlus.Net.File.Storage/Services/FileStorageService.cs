﻿namespace CodeDesignPlus.Net.File.Storage.Services;

/// <summary>
/// Service for handling file storage operations.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="FileStorageService"/> class.
/// </remarks>
/// <param name="providers">The collection of file storage providers.</param>
public class FileStorageService(IEnumerable<IProvider> providers) : IFileStorageService
{
    private readonly IEnumerable<IProvider> providers = providers;

    /// <summary>
    /// Deletes a file from all providers.
    /// </summary>
    /// <param name="file">The name of the file to delete.</param>
    /// <param name="target">The target directory.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task<M.Response[]> DeleteAsync(string file, string target, CancellationToken cancellationToken = default)
    {
        var tasks = new List<Task<M.Response>>();

        foreach (var provider in providers)
        {
            var task = provider.DeleteAsync(file, target, cancellationToken);
            tasks.Add(task);
        }

        return Task.WhenAll(tasks);
    }

    /// <summary>
    /// Downloads a file from the first provider where it exists.
    /// </summary>
    /// <param name="file">The name of the file to download.</param>
    /// <param name="target">The target directory.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task<M.Response> DownloadAsync(string file, string target, CancellationToken cancellationToken = default)
    {
        foreach (var provider in providers)
        {
            var response = await provider.DownloadAsync(file, target, cancellationToken).ConfigureAwait(false);

            if (response.Success)
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
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task<M.Response[]> UploadAsync(Stream stream, string file, string target, bool renowned, CancellationToken cancellationToken = default)
    {
        var tasks = new List<Task<M.Response>>();

        foreach (var provider in providers)
        {
            var task = provider.UploadAsync(stream, file, target, renowned, cancellationToken);
            tasks.Add(task);
        }

        return Task.WhenAll(tasks);
    }
}