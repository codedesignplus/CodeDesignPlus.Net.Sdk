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
    /// <remarks>
    /// The extension is matched against each extension listed by the MIME type first: a substring match alone resolved
    /// <c>.mp4</c> to <c>audio/mp4</c> (listed as <c>.mp4a</c> before the video), so a recorded video was served as audio.
    /// The substring match is kept as the fallback, for names whose extension no MIME type lists exactly.
    /// <para>
    /// A name without extension has no MIME type: the empty extension is contained in every entry, and the substring match
    /// gave it the first one of the catalog (<c>application/vnd.hzn-3d-crossword</c>) instead of binary data (Kappali
    /// pendings/526).
    /// </para>
    /// </remarks>
    public ApacheMime Mime
    {
        get => string.IsNullOrEmpty(this.Extension) ? null : ApacheMime.ApacheMimes.FirstOrDefault(x => x.Extension.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).Contains(this.Extension, StringComparer.OrdinalIgnoreCase))
            ?? ApacheMime.ApacheMimes.FirstOrDefault(x => x.Extension.Contains(this.Extension));
    }

    /// <summary>
    /// The content type of a file whose extension no MIME type lists: arbitrary binary data, as RFC 2046 (section 4.5.1)
    /// defines it.
    /// </summary>
    public const string UnknownContentType = "application/octet-stream";

    /// <summary>
    /// Gets the content type of the file: the one of its <see cref="Mime"/>, or <see cref="UnknownContentType"/> when no
    /// MIME type lists its extension.
    /// </summary>
    /// <remarks>
    /// Tags, metadata and headers are written with this value and not with <see cref="Mime"/>, which is <see langword="null"/>
    /// for an unknown extension: an <c>.mp3</c> recording, before <c>.mp3</c> was listed, failed every import with a
    /// <see cref="NullReferenceException"/> while its tags were built.
    /// </remarks>
    public string ContentType => this.Mime?.MimeType ?? UnknownContentType;

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
    /// The longest value Azure accepts in a blob index tag.
    /// </summary>
    public const int MaxTagValueLength = 256;

    /// <summary>
    /// Gets the metadata for the file.
    /// </summary>
    /// <remarks>
    /// Metadata travels as HTTP headers, which only carry ASCII: the name, the full name and the extension are written
    /// percent-encoded (<see cref="Uri.EscapeDataString(string)"/>) and read back with <see cref="Uri.UnescapeDataString(string)"/>.
    /// A file named «Cotización (1).pdf» failed every upload with «Request headers must contain only ASCII characters»
    /// (Kappali pendings/526).
    /// </remarks>
    /// <param name="uri">The URI for the file.</param>
    /// <returns>A dictionary containing the file metadata.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="uri"/> is null.</exception>
    public Dictionary<string, string> GetMetadata(Uri uri)
    {
        ArgumentNullException.ThrowIfNull(uri);

        return new Dictionary<string, string>
        {
            { "FullName", Uri.EscapeDataString(this.FullName) },
            { "Name", Uri.EscapeDataString(this.Name) },
            { "Extension", Uri.EscapeDataString(this.Extension) },
            { "Size", this.Size.ToString() },
            { "Version", this.Version.ToString() },
            { "Renowned", this.Renowned.ToString() },
            { "Mime", this.ContentType },
            { "Uri", uri.ToString()},
            { "CreatedAt", SystemClock.Instance.GetCurrentInstant().ToString() }
        };
    }

    /// <summary>
    /// Gets the tags for the file.
    /// </summary>
    /// <remarks>
    /// A blob index tag only admits ASCII letters and digits, space and <c>+ - . / : = _</c>, up to
    /// <see cref="MaxTagValueLength"/> characters: the name goes in as <see cref="ToTagValue(string)"/> folds it (Kappali
    /// pendings/526).
    /// </remarks>
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
            { "Name", ToTagValue(this.Name) },
            { "Mime", ToTagValue(this.ContentType) },
            { "Tenant", ToTagValue(tenant.ToString()) }
        };
    }
    /// <summary>
    /// Folds a text into a value a blob index tag accepts: accents and the tilde of the «ñ» are dropped, every other
    /// character outside ASCII letters, digits, space and <c>+ - . / : = _</c> becomes <c>_</c>, and the result is cut at
    /// <see cref="MaxTagValueLength"/> characters.
    /// </summary>
    /// <param name="value">The text to fold.</param>
    /// <returns>The value of the tag; empty for <see langword="null"/>.</returns>
    public static string ToTagValue(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return string.Empty;
        }

        var builder = new System.Text.StringBuilder(value.Length);

        foreach (var character in value.Normalize(System.Text.NormalizationForm.FormD))
        {
            if (System.Globalization.CharUnicodeInfo.GetUnicodeCategory(character) == System.Globalization.UnicodeCategory.NonSpacingMark)
            {
                continue;
            }

            builder.Append(IsTagCharacter(character) ? character : '_');

            if (builder.Length == MaxTagValueLength)
            {
                break;
            }
        }

        return builder.ToString();
    }

    /// <summary>
    /// Whether Azure accepts the character in a blob index tag.
    /// </summary>
    private static bool IsTagCharacter(char character) => char.IsAsciiLetterOrDigit(character) || character is ' ' or '+' or '-' or '.' or '/' or ':' or '=' or '_';
}