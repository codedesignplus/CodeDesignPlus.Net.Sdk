using NodaTime;
using NodaTime.Serialization.JsonNet;

namespace CodeDesignPlus.Net.Serializers;

/// <summary>
/// Provides methods for serializing and deserializing objects to and from JSON.
/// </summary>
public static class JsonSerializer
{
    private static readonly JsonSerializerSettings defaultSettings = CreateSettings();

    /// <summary>
    /// Creates a new <see cref="JsonSerializerSettings"/> instance already configured for NodaTime.
    /// </summary>
    /// <remarks>
    /// Callers that reuse a settings instance across requests must build it with this method and treat it as
    /// immutable afterwards. <see cref="JsonSerializerSettings.Converters"/> is a plain <see cref="System.Collections.Generic.List{T}"/>:
    /// mutating it while other threads serialize corrupts the list and makes Newtonsoft throw a
    /// <see cref="NullReferenceException"/> for the rest of the process lifetime.
    /// </remarks>
    /// <param name="configure">An optional action to apply the caller's own configuration before NodaTime is configured.</param>
    /// <returns>A new settings instance, configured for NodaTime, owned by the caller.</returns>
    public static JsonSerializerSettings CreateSettings(Action<JsonSerializerSettings> configure = null)
    {
        var settings = new JsonSerializerSettings();

        configure?.Invoke(settings);

        settings.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);

        return settings;
    }

    /// <summary>
    /// Returns a copy of the supplied settings configured for NodaTime, leaving the caller's instance untouched.
    /// </summary>
    /// <param name="settings">The settings supplied by the caller.</param>
    /// <returns>A configured copy, or the default settings when none were supplied.</returns>
    private static JsonSerializerSettings WithNodaTime(JsonSerializerSettings settings)
    {
        if (settings == null)
            return defaultSettings;

        var copy = new JsonSerializerSettings(settings);

        copy.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);

        return copy;
    }

    /// <summary>
    /// Serializes an object to a JSON string.
    /// </summary>
    /// <param name="value">The object to serialize.</param>
    /// <returns>A JSON string representing the serialized object.</returns>
    public static string Serialize(object value)
    {
        return JsonConvert.SerializeObject(value, defaultSettings);
    }

    /// <summary>
    /// Serializes an object to a JSON string using the specified settings.
    /// </summary>
    /// <param name="value">The object to serialize.</param>
    /// <param name="settings">The settings to use during serialization. The instance is not modified.</param>
    /// <returns>A JSON string representing the serialized object.</returns>
    public static string Serialize(object value, JsonSerializerSettings settings)
    {
        return JsonConvert.SerializeObject(value, WithNodaTime(settings));
    }

    /// <summary>
    /// Serializes an object to a JSON string using the specified formatting.
    /// </summary>
    /// <param name="value">The object to serialize.</param>
    /// <param name="formatting">The formatting options to use during serialization.</param>
    /// <returns>A JSON string representing the serialized object.</returns>
    public static string Serialize(object value, Formatting formatting)
    {
        return JsonConvert.SerializeObject(value, formatting, defaultSettings);
    }

    /// <summary>
    /// Serializes an object to a JSON string using the specified formatting and settings.
    /// </summary>
    /// <param name="value">The object to serialize.</param>
    /// <param name="formatting">The formatting options to use during serialization.</param>
    /// <param name="settings">The settings to use during serialization. The instance is not modified.</param>
    /// <returns>A JSON string representing the serialized object.</returns>
    public static string Serialize(object value, Formatting formatting, JsonSerializerSettings settings)
    {
        return JsonConvert.SerializeObject(value, formatting, WithNodaTime(settings));
    }

    /// <summary>
    /// Deserializes a JSON string to an object of the specified type.
    /// </summary>
    /// <typeparam name="T">The type of the object to deserialize.</typeparam>
    /// <param name="json">The JSON string to deserialize.</param>
    /// <returns>An object of the specified type deserialized from the JSON string.</returns>
    public static T Deserialize<T>(string json)
    {
        return JsonConvert.DeserializeObject<T>(json, defaultSettings);
    }

    /// <summary>
    /// Deserializes a JSON string to an object of the specified type using the specified settings.
    /// </summary>
    /// <typeparam name="T">The type of the object to deserialize.</typeparam>
    /// <param name="json">The JSON string to deserialize.</param>
    /// <param name="settings">The settings to use during deserialization. The instance is not modified.</param>
    /// <returns>An object of the specified type deserialized from the JSON string.</returns>
    public static T Deserialize<T>(string json, JsonSerializerSettings settings)
    {
        return JsonConvert.DeserializeObject<T>(json, WithNodaTime(settings));
    }

    /// <summary>
    /// Deserializes a JSON string to an object of the specified type.
    /// </summary>
    /// <param name="json">The JSON string to deserialize.</param>
    /// <param name="type">The type of the object to deserialize.</param>
    /// <returns>An object of the specified type deserialized from the JSON string.</returns>
    public static object Deserialize(string json, Type type)
    {
        return JsonConvert.DeserializeObject(json, type, defaultSettings);
    }

    /// <summary>
    /// Deserializes a JSON string to an object of the specified type using the specified settings.
    /// </summary>
    /// <param name="json">The JSON string to deserialize.</param>
    /// <param name="type">The type of the object to deserialize.</param>
    /// <param name="settings">The settings to use during deserialization. The instance is not modified.</param>
    /// <returns>An object of the specified type deserialized from the JSON string.</returns>
    public static object Deserialize(string json, Type type, JsonSerializerSettings settings)
    {
        return JsonConvert.DeserializeObject(json, type, WithNodaTime(settings));
    }
}
