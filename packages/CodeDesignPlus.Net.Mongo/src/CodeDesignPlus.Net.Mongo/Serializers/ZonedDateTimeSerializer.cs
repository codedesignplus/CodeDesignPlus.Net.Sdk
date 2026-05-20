using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization.Serializers;

namespace CodeDesignPlus.Net.Mongo.Serializers;

/// <summary>
/// Serializes <see cref="ZonedDateTime"/> to/from BSON as a document with Instant (ms epoch) and Zone (IANA timezone ID).
/// </summary>
public class ZonedDateTimeSerializer : SerializerBase<ZonedDateTime>
{
    public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, ZonedDateTime value)
    {
        context.Writer.WriteStartDocument();
        context.Writer.WriteName("Instant");
        var dateTime = value.ToInstant().ToDateTimeUtc();
        long ms = (dateTime - DateTime.UnixEpoch).Ticks / TimeSpan.TicksPerMillisecond;
        context.Writer.WriteDateTime(ms);
        context.Writer.WriteName("Zone");
        context.Writer.WriteString(value.Zone.Id);
        context.Writer.WriteEndDocument();
    }

    public override ZonedDateTime Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        if (context.Reader.CurrentBsonType == BsonType.Document)
            return ParseFromDocument(context);

        throw new FormatException($"Unexpected BsonType {context.Reader.CurrentBsonType} when deserializing ZonedDateTime.");
    }

    private static ZonedDateTime ParseFromDocument(BsonDeserializationContext context)
    {
        long ms = 0;
        string zone = "UTC";

        context.Reader.ReadStartDocument();
        while (context.Reader.ReadBsonType() != BsonType.EndOfDocument)
        {
            var fieldName = context.Reader.ReadName(Utf8NameDecoder.Instance);
            switch (fieldName)
            {
                case "Instant": ms = context.Reader.ReadDateTime(); break;
                case "Zone": zone = context.Reader.ReadString(); break;
                default: context.Reader.SkipValue(); break;
            }
        }
        context.Reader.ReadEndDocument();

        var instant = Instant.FromDateTimeUtc(DateTime.UnixEpoch.AddMilliseconds(ms));
        var dateTimeZone = DateTimeZoneProviders.Tzdb.GetZoneOrNull(zone) ?? DateTimeZone.Utc;

        return instant.InZone(dateTimeZone);
    }
}
