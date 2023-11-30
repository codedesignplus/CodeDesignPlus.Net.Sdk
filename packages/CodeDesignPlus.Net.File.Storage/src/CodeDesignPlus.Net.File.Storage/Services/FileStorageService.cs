using CodeDesignPlus.Net.File.Storage.Abstractions.Options;

namespace CodeDesignPlus.Net.File.Storage.Services;

/// <summary>
/// Default implementation of the <see cref="IFileStorageService"/>
/// </summary>
/// <remarks>
/// Initialize a new instance of the <see cref="FileStorageService"/>
/// </remarks>
/// <param name="logger">Logger Service</param>
/// <param name="options">File.Storage Options</param>
public class FileStorageService(ILogger<FileStorageService> logger, IOptions<FileStorageOptions> options) : IFileStorageService
{
    /// <summary>
    /// Logger Service
    /// </summary>
    private readonly ILogger<FileStorageService> logger = logger;
    /// <summary>
    /// File.Storage Options
    /// </summary>
    private readonly FileStorageOptions options = options.Value;

    /// <summary>
    /// Asynchronously echoes the specified value. 
    /// </summary>
    /// <param name="value">The value to echo.</param>
    /// <returns>A task that represents the asynchronous echo operation. The result of the task is the echoed value as a</returns>
    public Task<string> EchoAsync(string value)
    {
        this.logger.LogDebug("{section}, Echo {enable}", options.Enable);

        return Task.FromResult(value);
    }
}
