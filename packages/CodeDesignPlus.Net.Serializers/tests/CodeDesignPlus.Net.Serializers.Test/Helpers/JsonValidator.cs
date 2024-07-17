namespace CodeDesignPlus.Net.Serializers.Test.Helpers;

using System.Text.Json;

public class JsonValidator
{
    public static bool IsValidJson(string jsonString)
    {
        if (string.IsNullOrWhiteSpace(jsonString))
        {
            return false;
        }

        try
        {
            JsonDocument.Parse(jsonString);
            return true;
        }
        catch (JsonException)
        {
            return false;
        }
    }
}
