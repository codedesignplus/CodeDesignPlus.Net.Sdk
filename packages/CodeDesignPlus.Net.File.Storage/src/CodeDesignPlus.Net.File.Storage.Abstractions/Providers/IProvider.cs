using CodeDesignPlus.Net.File.Storage.Abstractions.Models;

namespace CodeDesignPlus.Net.File.Storage.Abstractions.Providers;

public interface IProvider<TKeyUser, TTenant>
{
    Task<Response> UploadAsync(Stream stream, string filename, string target, bool renowned = false, CancellationToken cancellationToken = default);
    Task<Response> DownloadAsync(string filename, string target, CancellationToken cancellationToken = default);
    Task<Response> DeleteAsync(string filename, string target, CancellationToken cancellationToken = default);
}
