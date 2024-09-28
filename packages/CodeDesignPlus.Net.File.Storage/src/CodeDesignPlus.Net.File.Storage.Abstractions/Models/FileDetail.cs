namespace CodeDesignPlus.Net.File.Storage.Abstractions.Models;

/// <summary>
/// Represents the details of a file, including its URI and provider information.
/// </summary>
public class FileDetail
{
    /// <summary>
    /// Gets the file name.
    /// </summary>
    public string File { get; }

    /// <summary>
    /// Gets the URI of the file.
    /// </summary>
    public Uri Uri { get; }

    /// <summary>
    /// Gets the target directory of the file.
    /// </summary>
    public string Target { get; }

    /// <summary>
    /// Gets the URI for downloading the file.
    /// </summary>
    public string UriDownload { get; }

    /// <summary>
    /// Gets the URI for viewing the file in a browser.
    /// </summary>
    public string UriViewInBrowser { get; }

    /// <summary>
    /// Gets the provider of the file.
    /// </summary>
    public string Provider { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="FileDetail"/> class.
    /// </summary>
    /// <param name="uri">The URI of the file.</param>
    /// <param name="target">The target directory of the file.</param>
    /// <param name="file">The name of the file.</param>
    /// <param name="provider">The type of provider.</param>
    /// <exception cref="ArgumentException">Thrown when the provider is not valid.</exception>
    /// <exception cref="ArgumentNullException">Thrown when the URI or file name is null.</exception>
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