using System;
using MongoDB.Bson.Serialization.Serializers;

namespace CodeDesignPlus.Net.Mongo.Serializers;

public class DurationSerializer : SerializerBase<Duration>
{
    public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, Duration value)
    {
        context.Writer.WriteInt64(value.NanosecondOfDay);
    }

    public override Duration Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        var nanoseconds = context.Reader.ReadInt64();
        return Duration.FromNanoseconds(nanoseconds);
    }
}