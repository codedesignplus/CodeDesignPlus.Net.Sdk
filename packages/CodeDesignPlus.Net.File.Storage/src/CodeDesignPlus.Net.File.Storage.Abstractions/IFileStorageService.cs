using CodeDesignPlus.Net.File.Storage.Abstractions.Models;

namespace CodeDesignPlus.Net.File.Storage.Abstractions;

public interface IFileStorageService
{
    Task<Response[]> UploadAsync(Stream stream, string file, string target, bool renowned, CancellationToken cancellationToken = default);
    Task<Response> DownloadAsync(string file, string target, CancellationToken cancellationToken = default);
    Task<Response[]> DeleteAsync(string file, string target, CancellationToken cancellationToken = default);
}
