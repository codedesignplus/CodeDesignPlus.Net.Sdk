namespace CodeDesignPlus.Net.Kafka.Serializer;

/// <summary>
/// Provides JSON serialization and deserialization using CodeDesignPlus.Net.Serializers; for Kafka messages.
/// </summary>
/// /// <typeparam name="T">The type of the message to serialize or deserialize.</typeparam>
public class JsonSystemTextSerializer<T> : ISerializer<T>, IDeserializer<T>
{
    /// <summary>
    /// Serializes the specified data to a byte array using JSON.
    /// </summary>
    /// <param name="data">The data to serialize.</param>
    /// <param name="context">The context for the serialization operation.</param>
    /// <returns>The serialized byte array, or null if the data is null.</returns>
    public byte[] Serialize(T data, SerializationContext context)
    {
        if (EqualityComparer<T>.Default.Equals(data, default(T)))
            return [];

        return Encoding.UTF8.GetBytes(JsonSerializer.Serialize(data));
    }

    /// <summary>
    /// Deserializes the specified byte array to an object using JSON.
    /// </summary>
    /// <param name="data">The byte array to deserialize.</param>
    /// <param name="isNull">Indicates whether the data is null.</param>
    /// <param name="context">The context for the deserialization operation.</param>
    /// <returns>The deserialized object of type <typeparamref name="T"/>, or the default value if the data is null.</returns>
    public T Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
    {
        if (isNull)
            return default;

        return JsonSerializer.Deserialize<T>(Encoding.UTF8.GetString(data));
    }
}
