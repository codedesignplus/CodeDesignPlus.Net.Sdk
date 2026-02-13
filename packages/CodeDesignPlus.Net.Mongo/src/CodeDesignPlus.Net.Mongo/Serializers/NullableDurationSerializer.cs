using System;
using MongoDB.Bson.Serialization.Serializers;

namespace CodeDesignPlus.Net.Mongo.Serializers;

public class NullableDurationSerializer : SerializerBase<Duration?>
{
    public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, Duration? value)
    {
        context.Writer.WriteInt64(value?.NanosecondOfDay ?? 0);
    }

    public override Duration? Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        if (context.Reader.CurrentBsonType == BsonType.Null)
        {
            context.Reader.ReadNull();
            return null;
        }
        
        var nanoseconds = context.Reader.ReadInt64();

        return Duration.FromNanoseconds(nanoseconds);
    }
}