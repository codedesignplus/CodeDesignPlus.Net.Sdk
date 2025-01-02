using Newtonsoft.Json.Linq;

namespace CodeDesignPlus.Net.Serializers;

/// <summary>
/// Provides methods for validating JSON strings.
/// </summary>
public class JsonValidate
{
    /// <summary>
    /// Determines whether the specified JSON string is valid.
    /// </summary>
    /// <param name="jsonString">The JSON string to validate.</param>
    /// <returns><c>true</c> if the JSON string is valid; otherwise, <c>false</c>.</returns>
    public static bool IsValidJson(string jsonString)
    {
        if (string.IsNullOrWhiteSpace(jsonString))
        {
            return false;
        }

        try
        {
            JToken.Parse(jsonString);

            return true; 
        }
        catch (JsonReaderException)
        {
            return false;
        }
    }
}
