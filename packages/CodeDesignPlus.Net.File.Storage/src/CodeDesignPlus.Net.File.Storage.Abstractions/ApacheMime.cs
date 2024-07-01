using System.Collections.ObjectModel;
using System.Reflection;
using Newtonsoft.Json;

namespace CodeDesignPlus.Net.File.Storage.Abstractions;

/// <summary>
///  https://svn.apache.org/repos/asf/httpd/httpd/trunk/docs/conf/mime.types
/// </summary>
public class ApacheMime
{
    public string Extension { get; set; }
    public string Name { get; set; }
    public string MimeType { get; set; }

    public static ReadOnlyCollection<ApacheMime> ApacheMimes
    {
        get
        {
            if (apacheMimes.Count > 0)
                return new ReadOnlyCollection<ApacheMime>(apacheMimes);

            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "CodeDesignPlus.Net.File.Storage.Abstractions.mime-types.json";

            using var stream = assembly.GetManifestResourceStream(resourceName);
            using var reader = new StreamReader(stream);

            var result = reader.ReadToEnd();

            apacheMimes = JsonConvert.DeserializeObject<List<ApacheMime>>(result);

            return new ReadOnlyCollection<ApacheMime>(apacheMimes);

        }
    }

    /// <summary>
    /// Lista Mime Types https://svn.apache.org/repos/asf/httpd/httpd/trunk/docs/conf/mime.types
    /// </summary>
    private static List<ApacheMime> apacheMimes = [];
}