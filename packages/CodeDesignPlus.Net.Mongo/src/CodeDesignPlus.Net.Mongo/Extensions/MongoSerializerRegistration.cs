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
        RegisterGuidSerializer();
        RegisterInstantSerializer();
        RegisterNullableInstantSerializer();
        RegisterDurationSerializer();
        RegisterNullableDurationSerializer();
    }

    /// <summary>
    /// Registers the <see cref="NullableInstantSerializer"/> if it has not been registered yet.
    /// </summary>
    private static void RegisterNullableInstantSerializer()
    {
        try
        {
            BsonSerializer.TryRegisterSerializer(new NullableInstantSerializer());
        }
        catch
        {
            // the unit test will fail if the serializer is already registered
        }
    }

    /// <summary>
    /// Registers the <see cref="InstantSerializer"/> if it has not been registered yet.
    /// </summary>
    private static void RegisterInstantSerializer()
    {
        try
        {
            BsonSerializer.TryRegisterSerializer(new InstantSerializer());
        }
        catch
        {
            // the unit test will fail if the serializer is already registered
        }
    }

    /// <summary>
    /// Registers the <see cref="GuidSerializer"/> if it has not been registered yet.
    /// </summary>
    private static void RegisterGuidSerializer()
    {
        try
        {
            BsonSerializer.TryRegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));
        }
        catch
        {
            // the unit test will fail if the serializer is already registered
        }
    }

    private static void RegisterNullableDurationSerializer()
    {
        try
        {
            BsonSerializer.TryRegisterSerializer(new NullableInstantSerializer());
        }
        catch
        {
            // the unit test will fail if the serializer is already registered
        }
    }

    private static void RegisterDurationSerializer()
    {
        try
        {
            BsonSerializer.TryRegisterSerializer(new InstantSerializer());
        }
        catch
        {
            // the unit test will fail if the serializer is already registered
        }
    }
}

