using CodeDesignPlus.Net.Mongo.Serializers;
using MongoDB.Bson.Serialization.Serializers;

namespace CodeDesignPlus.Net.Mongo.Extensions;

/// <summary>
/// Provides methods to register custom BSON serializers for MongoDB.
/// </summary>
public static class MongoSerializerRegistration
{
    /// <summary>
    /// Registers custom BSON serializers for MongoDB if they have not been registered yet.
    /// </summary>
    public static void RegisterSerializers()
    {
        var serializer = BsonSerializer.SerializerRegistry as BsonSerializerRegistry;

        RegisterGuidSerializer(serializer);
        RegisterInstantSerializer(serializer);
        RegisterNullableInstantSerializer(serializer);
    }

    /// <summary>
    /// Registers the <see cref="NullableInstantSerializer"/> if it has not been registered yet.
    /// </summary>
    /// <param name="serializer">The <see cref="BsonSerializerRegistry"/> instance.</param>
    private static void RegisterNullableInstantSerializer(BsonSerializerRegistry serializer)
    {
        var nullableInstantSerializer = serializer.GetSerializer<Instant?>();

        if (nullableInstantSerializer is null)
            BsonSerializer.TryRegisterSerializer(new NullableInstantSerializer());
    }

    /// <summary>
    /// Registers the <see cref="InstantSerializer"/> if it has not been registered yet.
    /// </summary>
    /// <param name="serializer">The <see cref="BsonSerializerRegistry"/> instance.</param>
    private static void RegisterInstantSerializer(BsonSerializerRegistry serializer)
    {
        var instantSerializer = serializer.GetSerializer<Instant>();

        if (instantSerializer is null)
            BsonSerializer.TryRegisterSerializer(new InstantSerializer());
    }

    /// <summary>
    /// Registers the <see cref="GuidSerializer"/> if it has not been registered yet.
    /// </summary>
    /// <param name="serializer">The <see cref="BsonSerializerRegistry"/> instance.</param>
    private static void RegisterGuidSerializer(BsonSerializerRegistry serializer)
    {
        var guidSerializer = serializer.GetSerializer<Guid>();

        if (guidSerializer is not GuidSerializer)
            BsonSerializer.TryRegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));
    }
}

