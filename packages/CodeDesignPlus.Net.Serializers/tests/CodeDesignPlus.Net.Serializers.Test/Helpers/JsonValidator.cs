namespace CodeDesignPlus.Net.Serializers.Test.Helpers;

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
            System.Text.Json.JsonDocument.Parse(jsonString);
            return true;
        }
        catch ( System.Text.Json.JsonException)
        {
            return false;
        }
    }
}
