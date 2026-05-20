using MongoDB.Bson.Serialization.Serializers;

namespace CodeDesignPlus.Net.Mongo.Serializers;

/// <summary>
/// Serializes <see cref="Nullable{Duration}"/> as Int64 nanoseconds or null in BSON.
/// </summary>
public class NullableDurationSerializer : SerializerBase<Duration?>
{
    public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, Duration? value)
    {
        if (!value.HasValue)
        {
            context.Writer.WriteNull();
            return;
        }

        context.Writer.WriteInt64((long)value.Value.TotalNanoseconds);
    }

    public override Duration? Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        return context.Reader.CurrentBsonType switch
        {
            BsonType.Null => ReadNull(context),
            BsonType.Int64 => Duration.FromNanoseconds(context.Reader.ReadInt64()),
            BsonType.Int32 => Duration.FromNanoseconds(context.Reader.ReadInt32()),
            BsonType.Double => Duration.FromNanoseconds((long)context.Reader.ReadDouble()),
            _ => throw new FormatException($"Unexpected BsonType {context.Reader.CurrentBsonType} when deserializing Duration?.")
        };
    }

    private static Duration? ReadNull(BsonDeserializationContext context)
    {
        context.Reader.ReadNull();
        return null;
    }
}