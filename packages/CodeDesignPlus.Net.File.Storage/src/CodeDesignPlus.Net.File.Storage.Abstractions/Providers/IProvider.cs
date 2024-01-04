using CodeDesignPlus.Net.File.Storage.Abstractions.Models;

namespace CodeDesignPlus.Net.File.Storage.Abstractions.Providers;

public interface IProvider
{
    Task<Response> UploadAsync(Stream stream, Models.File file, string target, CancellationToken cancellationToken = default);
    Task<Response> DownloadAsync(string file, string target, CancellationToken cancellationToken = default);
    Task<Response> DeleteAsync(string file, string target, CancellationToken cancellationToken = default);
}
