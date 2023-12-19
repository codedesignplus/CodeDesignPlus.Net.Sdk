using CodeDesignPlus.Net.File.Storage.Abstractions.Models;
using CodeDesignPlus.Net.File.Storage.Abstractions.Options;
using CodeDesignPlus.Net.File.Storage.Abstractions.Providers;

namespace CodeDesignPlus.Net.File.Storage.Services;

/// <summary>
/// Default implementation of the <see cref="IFileStorageService"/>
/// </summary>
/// <remarks>
/// Initialize a new instance of the <see cref="FileStorageService"/>
/// </remarks>
/// <param name="logger">Logger Service</param>
/// <param name="options">File.Storage Options</param>
public class FileStorageService<TTenant>(ILogger<FileStorageService<TTenant>> logger, IOptions<FileStorageOptions> options) : IFileStorageService<TTenant>
{
    /// <summary>
    /// Logger Service
    /// </summary>
    private readonly ILogger<FileStorageService<TTenant>> logger = logger;
    /// <summary>
    /// File.Storage Options
    /// </summary>
    private readonly FileStorageOptions options = options.Value;

    public Task<Response> DownloadFileAsync(TTenant tenant, string file, string target, TypeProviders typeProvider)
    {
        throw new NotImplementedException();
    }

    public Task<IList<Response>> UploadFileAsync(TTenant tenant, Stream stream, string uri, string target, Abstractions.Models.File file, TypeProviders typeProvider)
    {
        throw new NotImplementedException();
    }

}
