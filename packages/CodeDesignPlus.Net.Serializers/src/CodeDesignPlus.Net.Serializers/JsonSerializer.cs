using Newtonsoft.Json;

namespace CodeDesignPlus.Net.Serializers;

public static class JsonSerializer
{
    /// <summary>
    /// Serializes an object to a JSON string.
    /// </summary>
    /// <param name="value">The object to serialize.</param>
    /// <returns>A JSON string representing the serialized object.</returns>
    public static string Serialize(object value)
    {
        return JsonConvert.SerializeObject(value);
    }

    /// <summary>
    /// Serializes an object to a JSON string using the specified settings.
    /// </summary>
    /// <param name="value">The object to serialize.</param>
    /// <param name="settings">The settings to use during serialization.</param>
    /// <returns>A JSON string representing the serialized object.</returns>
    public static string Serialize(object value, JsonSerializerSettings settings)
    {
        return JsonConvert.SerializeObject(value, settings);
    }

    /// <summary>
    /// Serializes an object to a JSON string using the specified formatting.
    /// </summary>
    /// <param name="value">The object to serialize.</param>
    /// <param name="formatting">The formatting options to use during serialization.</param>
    /// <returns>A JSON string representing the serialized object.</returns>
    public static string Serialize(object value, Formatting formatting)
    {
        return JsonConvert.SerializeObject(value, formatting);
    }

    /// <summary>
    /// Serializes an object to a JSON string using the specified formatting and settings.
    /// </summary>
    /// <param name="value">The object to serialize.</param>
    /// <param name="formatting">The formatting options to use during serialization.</param>
    /// <param name="settings">The settings to use during serialization.</param>
    /// <returns>A JSON string representing the serialized object.</returns>
    public static string Serialize(object value, Formatting formatting, JsonSerializerSettings settings)
    {
        return JsonConvert.SerializeObject(value, formatting, settings);
    }

    /// <summary>
    /// Deserializes a JSON string to an object of the specified type.
    /// </summary>
    /// <typeparam name="T">The type of the object to deserialize.</typeparam>
    /// <param name="json">The JSON string to deserialize.</param>
    /// <returns>An object of the specified type deserialized from the JSON string.</returns>
    public static T Deserialize<T>(string json)
    {
        return JsonConvert.DeserializeObject<T>(json);
    }

    /// <summary>
    /// Deserializes a JSON string to an object of the specified type using the specified settings.
    /// </summary>
    /// <typeparam name="T">The type of the object to deserialize.</typeparam>
    /// <param name="json">The JSON string to deserialize.</param>
    /// <param name="settings">The settings to use during deserialization.</param>
    /// <returns>An object of the specified type deserialized from the JSON string.</returns>
    public static T Deserialize<T>(string json, JsonSerializerSettings settings)
    {
        return JsonConvert.DeserializeObject<T>(json, settings);
    }

    /// <summary>
    /// Deserializes a JSON string to an object of the specified type.
    /// </summary>
    /// <param name="json">The JSON string to deserialize.</param>
    /// <param name="type">The type of the object to deserialize.</param>
    /// <returns>An object of the specified type deserialized from the JSON string.</returns>
    public static object Deserialize(string json, Type type)
    {
        return JsonConvert.DeserializeObject(json, type);
    }

    /// <summary>
    /// Deserializes a JSON string to an object of the specified type using the specified settings.
    /// </summary>
    /// <param name="json">The JSON string to deserialize.</param>
    /// <param name="type">The type of the object to deserialize.</param>
    /// <param name="settings">The settings to use during deserialization.</param>
    /// <returns>An object of the specified type deserialized from the JSON string.</returns>
    public static object Deserialize(string json, Type type, JsonSerializerSettings settings)
    {
        return JsonConvert.DeserializeObject(json, type, settings);
    }
}
