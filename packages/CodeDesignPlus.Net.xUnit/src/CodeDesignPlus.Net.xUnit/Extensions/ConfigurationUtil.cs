namespace CodeDesignPlus.Net.xUnit.Extensions;

/// <summary>
/// Provides utility methods for configuration management in xUnit tests.
/// </summary>
public static class ConfigurationUtil
{
    /// <summary>
    /// Creates an <see cref="IConfiguration"/> instance from the provided app settings object.
    /// </summary>
    /// <param name="appSettings">The application settings object to be serialized to JSON. Default is null.</param>
    /// <returns>An <see cref="IConfiguration"/> instance containing the serialized app settings.</returns>
    public static IConfiguration GetConfiguration(object appSettings = null)
    {
        // Serialize the app settings object to JSON.
        var json = JsonSerializer.Serialize(appSettings);

        // Convert the JSON string to a memory stream.
        var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(json));

        // Build and return the configuration from the JSON stream.
        return new ConfigurationBuilder().AddJsonStream(memoryStream).Build();
    }
}