using CodeDesignPlus.Net.File.Storage.Abstractions.Models;
using CodeDesignPlus.Net.File.Storage.Abstractions.Providers;

namespace CodeDesignPlus.Net.File.Storage.Abstractions;

public interface IFileStorageService<TTenant>
{
    Task<IList<Response>> UploadFileAsync(TTenant tenant, Stream stream, string uri, string target, Models.File file, TypeProviders typeProvider);
    Task<Response> DownloadFileAsync(TTenant tenant, string file, string target, TypeProviders typeProvider);
}
