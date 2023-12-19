using CodeDesignPlus.Net.File.Storage.Abstractions.Models;
using CodeDesignPlus.Net.File.Storage.Abstractions.Providers;

namespace CodeDesignPlus.Net.File.Storage.Providers;

public class AzureFileProvider<TTenant> : IAzureFileProvider<TTenant>
{
    public Task<Response> ReadFileAsync(TTenant tenant, string file, string target)
    {
        throw new NotImplementedException();
    }

    public Task<IList<Response>> WriteFileAsync( TTenant tenant, Stream stream, string target, Abstractions.Models.File file)
    {
        throw new NotImplementedException();
    }
}
