namespace CodeDesignPlus.Net.File.Storage.Abstractions.Models;

public class FileDetail
{
    public string File { get; }
    public Uri Uri { get; }
    public string Target { get; }
    public string UriDownload { get; }
    public string UriViewInBrowser { get; }
    public string Provider { get; private set; }

    public FileDetail(Uri uri, string target, string file, TypeProviders provider)
    {
        if (provider == TypeProviders.None)
            throw new ArgumentException("The type provider is not valid", nameof(provider));

        if (string.IsNullOrEmpty(file))
            throw new ArgumentNullException(nameof(file));

        this.Uri = uri ?? throw new ArgumentNullException(nameof(uri));
        this.Provider = provider.ToString();
        this.Target = target;
        this.File = file;

        this.UriDownload = $"{uri}/{(int)provider}?file={file}{(!string.IsNullOrEmpty(target) ? $"&target={target}" : string.Empty)}";
        this.UriViewInBrowser = $"{uri}/{(int)provider}?viewInBrowser=true&file={file}{(!string.IsNullOrEmpty(target) ? $"&target={target}" : string.Empty)}";
    }

}
