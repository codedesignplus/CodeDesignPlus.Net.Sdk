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
        RegisterLocalDateSerializer();
        RegisterNullableLocalDateSerializer();
        RegisterLocalTimeSerializer();
        RegisterNullableLocalTimeSerializer();
        RegisterLocalDateTimeSerializer();
        RegisterNullableLocalDateTimeSerializer();
        RegisterZonedDateTimeSerializer();
        RegisterNullableZonedDateTimeSerializer();
    }

    private static void RegisterGuidSerializer()
    {
        try { BsonSerializer.TryRegisterSerializer(new GuidSerializer(GuidRepresentation.Standard)); }
        catch { }
    }

    private static void RegisterInstantSerializer()
    {
        try { BsonSerializer.TryRegisterSerializer(new InstantSerializer()); }
        catch { }
    }

    private static void RegisterNullableInstantSerializer()
    {
        try { BsonSerializer.TryRegisterSerializer(new NullableInstantSerializer()); }
        catch { }
    }

    private static void RegisterDurationSerializer()
    {
        try { BsonSerializer.TryRegisterSerializer(new DurationSerializer()); }
        catch { }
    }

    private static void RegisterNullableDurationSerializer()
    {
        try { BsonSerializer.TryRegisterSerializer(new NullableDurationSerializer()); }
        catch { }
    }

    private static void RegisterLocalDateSerializer()
    {
        try { BsonSerializer.TryRegisterSerializer(new LocalDateSerializer()); }
        catch { }
    }

    private static void RegisterNullableLocalDateSerializer()
    {
        try { BsonSerializer.TryRegisterSerializer(new NullableLocalDateSerializer()); }
        catch { }
    }

    private static void RegisterLocalTimeSerializer()
    {
        try { BsonSerializer.TryRegisterSerializer(new LocalTimeSerializer()); }
        catch { }
    }

    private static void RegisterNullableLocalTimeSerializer()
    {
        try { BsonSerializer.TryRegisterSerializer(new NullableLocalTimeSerializer()); }
        catch { }
    }

    private static void RegisterLocalDateTimeSerializer()
    {
        try { BsonSerializer.TryRegisterSerializer(new LocalDateTimeSerializer()); }
        catch { }
    }

    private static void RegisterNullableLocalDateTimeSerializer()
    {
        try { BsonSerializer.TryRegisterSerializer(new NullableLocalDateTimeSerializer()); }
        catch { }
    }

    private static void RegisterZonedDateTimeSerializer()
    {
        try { BsonSerializer.TryRegisterSerializer(new ZonedDateTimeSerializer()); }
        catch { }
    }

    private static void RegisterNullableZonedDateTimeSerializer()
    {
        try { BsonSerializer.TryRegisterSerializer(new NullableZonedDateTimeSerializer()); }
        catch { }
    }
}

