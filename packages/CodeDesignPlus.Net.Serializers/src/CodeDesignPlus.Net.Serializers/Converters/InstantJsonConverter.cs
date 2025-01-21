using NodaTime;
using NodaTime.Text;

namespace CodeDesignPlus.Net.Serializers.Converters;

public class InstantJsonConverter : JsonConverter<Instant?>
{
    private static readonly InstantPattern Pattern = InstantPattern.CreateWithInvariantCulture("yyyy-MM-ddTHH:mm:ss.fffffffff'Z'");

    public override Instant? ReadJson(JsonReader reader, Type objectType, Instant? existingValue, bool hasExistingValue, Newtonsoft.Json.JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null)
            return null;

        if(reader.Value is DateTime datetime)
            return Instant.FromDateTimeUtc(datetime);

        if (reader.Value is string value)
        {
            var parseResult = Pattern.Parse(value);

            if (parseResult.Success)
                return parseResult.Value;

            throw new JsonSerializationException($"Error parsing Instant from: '{value}'");
        }

        throw new JsonSerializationException($"Unexpected token {reader.TokenType} when parsing Instant.");
    }

    public override void WriteJson(JsonWriter writer, Instant? value, Newtonsoft.Json.JsonSerializer serializer)
    {
        if (value.HasValue)
        {
            writer.WriteValue(Pattern.Format(value.Value));
        }
        else
        {
            writer.WriteNull();
        }
    }
}