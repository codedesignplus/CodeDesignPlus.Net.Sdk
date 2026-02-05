namespace CodeDesignPlus.Net.File.Storage.Abstractions;

/// <summary>
/// Represents a MIME type as defined by Apache.
/// </summary>
/// <remarks>
/// For more information, see: https://svn.apache.org/repos/asf/httpd/httpd/trunk/docs/conf/mime.types
/// </remarks>
public class ApacheMime
{
    /// <summary>
    /// Gets or sets the file extension associated with the MIME type.
    /// </summary>
    public string Extension { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the name of the MIME type.
    /// </summary>
    public string Name { get; set; }= string.Empty;

    /// <summary>
    /// Gets or sets the MIME type.
    /// </summary>
    public string MimeType { get; set; }= string.Empty;

    /// <summary>
    /// Gets a read-only collection of Apache MIME types.
    /// </summary>
    public static ReadOnlyCollection<ApacheMime> ApacheMimes
    {
        get
        {
            if (apacheMimes.Count > 0)
                return new ReadOnlyCollection<ApacheMime>(apacheMimes);

            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "CodeDesignPlus.Net.File.Storage.Abstractions.mime-types.json";

            using var stream = assembly.GetManifestResourceStream(resourceName);
            using var reader = new StreamReader(stream!);

            var result = reader.ReadToEnd();

            apacheMimes = JsonSerializer.Deserialize<List<ApacheMime>>(result);

            return new ReadOnlyCollection<ApacheMime>(apacheMimes);
        }
    }

    /// <summary>
    /// List of Apache MIME types.
    /// </summary>
    private static List<ApacheMime> apacheMimes = [];
}