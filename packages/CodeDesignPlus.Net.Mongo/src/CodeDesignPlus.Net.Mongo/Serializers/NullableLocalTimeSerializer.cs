using MongoDB.Bson.Serialization.Serializers;
using NodaTime.Text;

namespace CodeDesignPlus.Net.Mongo.Serializers;

/// <summary>
/// Serializes <see cref="Nullable{LocalTime}"/> to/from BSON as a string in extended ISO format or null.
/// </summary>
public class NullableLocalTimeSerializer : SerializerBase<LocalTime?>
{
    private static readonly LocalTimePattern Pattern = LocalTimePattern.ExtendedIso;

    public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, LocalTime? value)
    {
        if (!value.HasValue)
        {
            context.Writer.WriteNull();
            return;
        }

        context.Writer.WriteString(Pattern.Format(value.Value));
    }

    public override LocalTime? Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        return context.Reader.CurrentBsonType switch
        {
            BsonType.Null => ReadNull(context),
            BsonType.String => ParseFromString(context.Reader.ReadString()),
            BsonType.Int64 => LocalTime.FromNanosecondsSinceMidnight(context.Reader.ReadInt64()),
            BsonType.Int32 => LocalTime.FromSecondsSinceMidnight(context.Reader.ReadInt32()),
            _ => throw new FormatException($"Unexpected BsonType {context.Reader.CurrentBsonType} when deserializing LocalTime?.")
        };
    }

    private static LocalTime? ReadNull(BsonDeserializationContext context)
    {
        context.Reader.ReadNull();
        return null;
    }

    private static LocalTime ParseFromString(string value)
    {
        var result = Pattern.Parse(value);
        if (result.Success)
            return result.Value;

        throw new FormatException($"Error parsing LocalTime from: '{value}'");
    }
}
