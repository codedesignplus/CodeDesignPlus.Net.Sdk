using MongoDB.Bson.Serialization.Serializers;
using NodaTime.Text;

namespace CodeDesignPlus.Net.Mongo.Serializers;

/// <summary>
/// Serializes <see cref="LocalDateTime"/> to/from BSON as a string in general ISO format.
/// </summary>
public class LocalDateTimeSerializer : SerializerBase<LocalDateTime>
{
    private static readonly LocalDateTimePattern Pattern = LocalDateTimePattern.GeneralIso;

    public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, LocalDateTime value)
    {
        context.Writer.WriteString(Pattern.Format(value));
    }

    public override LocalDateTime Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        return context.Reader.CurrentBsonType switch
        {
            BsonType.String => ParseFromString(context.Reader.ReadString()),
            BsonType.DateTime => ParseFromDateTime(context.Reader.ReadDateTime()),
            _ => throw new FormatException($"Unexpected BsonType {context.Reader.CurrentBsonType} when deserializing LocalDateTime.")
        };
    }

    private static LocalDateTime ParseFromString(string value)
    {
        var result = Pattern.Parse(value);
        if (result.Success)
            return result.Value;

        throw new FormatException($"Error parsing LocalDateTime from: '{value}'");
    }

    private static LocalDateTime ParseFromDateTime(long millisecondsSinceEpoch)
    {
        var dateTime = DateTime.UnixEpoch.AddMilliseconds(millisecondsSinceEpoch);
        return LocalDateTime.FromDateTime(dateTime);
    }
}
