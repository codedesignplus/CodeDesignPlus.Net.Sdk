using CodeDesignPlus.Net.File.Storage.Abstractions.Providers;

namespace CodeDesignPlus.Net.File.Storage.Abstractions.Models;

public class Response
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public Exception Exception { get; set; }
    public File File { get; }
    public Stream Stream { get; set; }
    public string Provider { get; }

    public Response(File file, TypeProviders typeProvider)
    {
        if (typeProvider == TypeProviders.None)
            throw new ArgumentException("The type provider is not valid", nameof(typeProvider));

        this.File = file ?? throw new ArgumentNullException(nameof(file));
        this.Provider = typeProvider.ToString();
    }

}
