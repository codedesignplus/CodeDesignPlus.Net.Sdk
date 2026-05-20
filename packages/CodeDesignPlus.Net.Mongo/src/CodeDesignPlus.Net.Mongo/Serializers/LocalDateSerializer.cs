using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization.Serializers;
using NodaTime.Text;

namespace CodeDesignPlus.Net.Mongo.Serializers;

/// <summary>
/// Serializes <see cref="LocalDate"/> to/from BSON. Supports String (ISO format), DateTime, and Document representations.
/// </summary>
public class LocalDateSerializer : SerializerBase<LocalDate>
{
    private static readonly LocalDatePattern IsoPattern = LocalDatePattern.Iso;

    public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, LocalDate value)
    {
        context.Writer.WriteString(IsoPattern.Format(value));
    }

    public override LocalDate Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        return context.Reader.CurrentBsonType switch
        {
            BsonType.String => ParseFromString(context.Reader.ReadString()),
            BsonType.DateTime => ParseFromDateTime(context.Reader.ReadDateTime()),
            BsonType.Document => ParseFromDocument(context),
            _ => throw new FormatException($"Unexpected BsonType {context.Reader.CurrentBsonType} when deserializing LocalDate.")
        };
    }

    private static LocalDate ParseFromString(string value)
    {
        var result = IsoPattern.Parse(value);
        if (result.Success)
            return result.Value;

        throw new FormatException($"Error parsing LocalDate from: '{value}'");
    }

    private static LocalDate ParseFromDateTime(long millisecondsSinceEpoch)
    {
        var dateTime = DateTime.UnixEpoch.AddMilliseconds(millisecondsSinceEpoch);
        return LocalDate.FromDateTime(dateTime);
    }

    private static LocalDate ParseFromDocument(BsonDeserializationContext context)
    {
        int year = 0, month = 0, day = 0;

        context.Reader.ReadStartDocument();
        while (context.Reader.ReadBsonType() != BsonType.EndOfDocument)
        {
            var fieldName = context.Reader.ReadName(Utf8NameDecoder.Instance);
            switch (fieldName)
            {
                case "Year": year = context.Reader.ReadInt32(); break;
                case "Month": month = context.Reader.ReadInt32(); break;
                case "Day": day = context.Reader.ReadInt32(); break;
                default: context.Reader.SkipValue(); break;
            }
        }
        context.Reader.ReadEndDocument();

        return new LocalDate(year, month, day);
    }
}
