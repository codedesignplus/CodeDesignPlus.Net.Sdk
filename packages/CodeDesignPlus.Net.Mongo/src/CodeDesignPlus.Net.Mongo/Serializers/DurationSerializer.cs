using MongoDB.Bson.Serialization.Serializers;

namespace CodeDesignPlus.Net.Mongo.Serializers;

/// <summary>
/// Serializes <see cref="Duration"/> as Int64 nanoseconds in BSON.
/// </summary>
public class DurationSerializer : SerializerBase<Duration>
{
    public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, Duration value)
    {
        context.Writer.WriteInt64((long)value.TotalNanoseconds);
    }

    public override Duration Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        return context.Reader.CurrentBsonType switch
        {
            BsonType.Int64 => Duration.FromNanoseconds(context.Reader.ReadInt64()),
            BsonType.Int32 => Duration.FromNanoseconds(context.Reader.ReadInt32()),
            BsonType.Double => Duration.FromNanoseconds((long)context.Reader.ReadDouble()),
            _ => throw new FormatException($"Unexpected BsonType {context.Reader.CurrentBsonType} when deserializing Duration.")
        };
    }
}