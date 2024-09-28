namespace CodeDesignPlus.Net.File.Storage.Abstractions.Models;

/// <summary>
/// Represents a file with metadata and versioning information.
/// </summary>
public class File
{
    /// <summary>
    /// Gets the file extension.
    /// </summary>
    public string Extension { get => System.IO.Path.GetExtension(this.FullName); }

    /// <summary>
    /// Gets or sets the full name of the file.
    /// </summary>
    public string FullName { get; set; }

    /// <summary>
    /// Gets the name of the file without the extension.
    /// </summary>
    public string Name { get => System.IO.Path.GetFileNameWithoutExtension(this.FullName); }

    /// <summary>
    /// Gets or sets the file detail.
    /// </summary>
    public FileDetail Detail { get; set; }

    /// <summary>
    /// Gets or sets the size of the file.
    /// </summary>
    public long Size { get; set; }

    /// <summary>
    /// Gets or sets the version of the file.
    /// </summary>
    public SemVersion Version { get; set; } = new SemVersion(1, 0, 0);

    /// <summary>
    /// Gets or sets a value indicating whether the file has been renamed.
    /// </summary>
    public bool Renowned { get; set; }

    /// <summary>
    /// Gets the MIME type of the file.
    /// </summary>
    public ApacheMime Mime { get => ApacheMime.ApacheMimes.FirstOrDefault(x => x.Extension.Contains(this.Extension)); }

    /// <summary>
    /// Initializes a new instance of the <see cref="File"/> class.
    /// </summary>
    /// <param name="fullName">The full name of the file.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="fullName"/> is null or empty.</exception>
    public File(string fullName)
    {
        if (string.IsNullOrEmpty(fullName))
            throw new ArgumentNullException(nameof(fullName));

        this.FullName = fullName;
    }

    /// <summary>
    /// Gets the metadata for the file.
    /// </summary>
    /// <param name="uri">The URI for the file.</param>
    /// <returns>A dictionary containing the file metadata.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="uri"/> is null.</exception>
    public Dictionary<string, string> GetMetadata(Uri uri)
    {
        ArgumentNullException.ThrowIfNull(uri);

        return new Dictionary<string, string>
        {
            { "FullName", this.FullName },
            { "Name", this.Name },
            { "Extension", this.Extension },
            { "Size", this.Size.ToString() },
            { "Version", this.Version.ToString() },
            { "Renowned", this.Renowned.ToString() },
            { "Mime", this.Mime.ToString() },
            { "Uri", uri.ToString()},
            { "CreatedAt", DateTime.UtcNow.ToString() }
        };
    }

    /// <summary>
    /// Gets the tags for the file.
    /// </summary>
    /// <typeparam name="TTenant">The type of the tenant.</typeparam>
    /// <param name="tenant">The tenant information.</param>
    /// <returns>A dictionary containing the file tags.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="tenant"/> is null.</exception>
    public Dictionary<string, string> GetTags<TTenant>(TTenant tenant)
    {
        if (tenant is null)
            throw new ArgumentNullException(nameof(tenant));

        return new Dictionary<string, string>
        {
            { "Name", this.Name },
            { "Mime", this.Mime.ToString() },
            { "Tenant", tenant.ToString() }
        };
    }
}