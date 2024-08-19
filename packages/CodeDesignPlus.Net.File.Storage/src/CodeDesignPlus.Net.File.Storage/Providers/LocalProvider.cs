namespace CodeDesignPlus.Net.File.Storage.Providers;

/// <summary>
/// Provides methods for interacting with local file storage.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="LocalProvider"/> class.
/// </remarks>
/// <param name="options">The file storage options.</param>
/// <param name="logger">The logger instance.</param>
/// <param name="environment">The host environment.</param>
/// <param name="userContext">The user context.</param>
public class LocalProvider(
    IOptions<FileStorageOptions> options,
    ILogger<LocalProvider> logger,
    IHostEnvironment environment,
    IUserContext userContext
    ) : BaseProvider(logger, environment), ILocalProvider
{
    private readonly IUserContext UserContext = userContext;
    private readonly FileStorageOptions Options = options.Value;

    /// <summary>
    /// Downloads a file from local storage.
    /// </summary>
    /// <param name="filename">The name of the file to download.</param>
    /// <param name="target">The target directory.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task<M.Response> DownloadAsync(string filename, string target, CancellationToken cancellationToken = default)
    {
        return ProcessAsync(Options.Local.Enable, filename, TypeProviders.LocalProvider, async (file, response) =>
        {
            var path = Path.Combine(Options.Local.Folder, UserContext.Tenant.ToString(), target, filename);

            if (System.IO.File.Exists(path))
            {
                var memoryStream = new MemoryStream();
                using var fileStream = new FileStream(path, FileMode.Open);

                await fileStream.CopyToAsync(memoryStream, cancellationToken: cancellationToken).ConfigureAwait(false);

                response.Stream = memoryStream;
                response.Stream.Position = 0;
                response.Success = true;
            }
            else
            {
                response.Success = false;
                response.Message = "The system cannot find the file specified";
            }

            return response;
        });
    }

    /// <summary>
    /// Uploads a file to local storage.
    /// </summary>
    /// <param name="stream">The file stream.</param>
    /// <param name="filename">The name of the file.</param>
    /// <param name="target">The target directory.</param>
    /// <param name="renowned">Whether to rename the file if it already exists.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task<M.Response> UploadAsync(Stream stream, string filename, string target, bool renowned = false, CancellationToken cancellationToken = default)
    {
        return ProcessAsync(Options.Local.Enable, filename, TypeProviders.LocalProvider, async (file, response) =>
        {
            var path = GetFullPath(file, target, renowned);

            using var fileStream = new FileStream(path, FileMode.Create);

            await stream.CopyToAsync(fileStream, cancellationToken: cancellationToken).ConfigureAwait(false);

            file.Detail = new M.FileDetail(Options.UriDownload, target, file.FullName, TypeProviders.LocalProvider);

            response.Success = System.IO.File.Exists(path);

            return response;
        });
    }

    /// <summary>
    /// Deletes a file from local storage.
    /// </summary>
    /// <param name="filename">The name of the file to delete.</param>
    /// <param name="target">The target directory.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task<M.Response> DeleteAsync(string filename, string target, CancellationToken cancellationToken = default)
    {
        return ProcessAsync(Options.Local.Enable, filename, TypeProviders.LocalProvider, (file, response) =>
        {
            var path = Path.Combine(GetPath(target), filename);

            if (!System.IO.File.Exists(path))
            {
                response.Success = false;
                response.Message = "The system cannot find the file specified";

                return Task.FromResult(response);
            }

            System.IO.File.Delete(path);

            response.Success = true;

            return Task.FromResult(response);
        });
    }

    /// <summary>
    /// Gets the full path for the specified target directory.
    /// </summary>
    /// <param name="target">The target directory.</param>
    /// <returns>The full path.</returns>
    private string GetPath(string target)
    {
        var path = Path.Combine(Options.Local.Folder, UserContext.Tenant.ToString(), target);

        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        return path;
    }

    /// <summary>
    /// Gets the full path for the specified file.
    /// </summary>
    /// <param name="file">The file.</param>
    /// <param name="target">The target directory.</param>
    /// <param name="renowned">Whether to rename the file if it already exists.</param>
    /// <returns>The full path.</returns>
    private string GetFullPath(Abstractions.Models.File file, string target, bool renowned)
    {
        var path = GetPath(target);

        if (!renowned)
            return Path.Combine(path, file.FullName);

        return GetNextName(file, path);
    }

    /// <summary>
    /// Gets the next available name for the specified file.
    /// </summary>
    /// <param name="file">The file.</param>
    /// <param name="path">The path.</param>
    /// <returns>The next available name.</returns>
    private static string GetNextName(Abstractions.Models.File file, string path)
    {
        var fileName = file.Name;
        var fullPath = Path.Combine(path, fileName);
        var count = 1;

        while (System.IO.File.Exists(fullPath))
        {
            count += 1;

            file.Renowned = true;

            fileName = $@"{file.Name} ({count}){file.Extension}";

            fullPath = Path.Combine(path, fileName);
        }

        file.FullName = fileName;

        return fullPath;
    }
}