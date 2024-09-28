namespace CodeDesignPlus.Net.File.Storage.Abstractions.Models;

/// <summary>
/// Represents the response from a file storage operation.
/// </summary>
public class Response
{
    /// <summary>
    /// Gets or sets a value indicating whether the operation was successful.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Gets or sets the message associated with the response.
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// Gets or sets the exception associated with the response, if any.
    /// </summary>
    public Exception Exception { get; set; }

    /// <summary>
    /// Gets the file associated with the response.
    /// </summary>
    public File File { get; }

    /// <summary>
    /// Gets or sets the stream associated with the response.
    /// </summary>
    public Stream Stream { get; set; }

    /// <summary>
    /// Gets the provider associated with the response.
    /// </summary>
    public string Provider { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Response"/> class.
    /// </summary>
    /// <param name="file">The file associated with the response.</param>
    /// <param name="typeProvider">The type of provider.</param>
    /// <exception cref="ArgumentException">Thrown when the provider is not valid.</exception>
    /// <exception cref="ArgumentNullException">Thrown when the file is null.</exception>
    public Response(File file, TypeProviders typeProvider)
    {
        if (typeProvider == TypeProviders.None)
            throw new ArgumentException("The type provider is not valid", nameof(typeProvider));

        this.File = file ?? throw new ArgumentNullException(nameof(file));
        this.Provider = typeProvider.ToString();
    }
}