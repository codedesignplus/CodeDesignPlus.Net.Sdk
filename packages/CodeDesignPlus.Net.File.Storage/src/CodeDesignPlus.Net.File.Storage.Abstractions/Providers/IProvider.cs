using CodeDesignPlus.Net.File.Storage.Abstractions.Models;

namespace CodeDesignPlus.Net.File.Storage.Abstractions.Providers;

public interface IProvider<TTenant>
{
    Task<IList<Response>> WriteFileAsync(TTenant tenant, Stream stream, string target, Models.File file);
    Task<Response> ReadFileAsync(TTenant tenant, string file, string target);
}
