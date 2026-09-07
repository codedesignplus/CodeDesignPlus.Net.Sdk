using System.Collections.Concurrent;
using System.Threading.Tasks;
using CodeDesignPlus.Net.Serializers;
using Newtonsoft.Json.Serialization;

namespace CodeDesignPlus.Net.Serializers.Test;

/// <summary>
/// Guards that the serializer never mutates the settings instance it receives.
/// </summary>
/// <remarks>
/// Callers such as the REST exception middleware keep a single settings instance for the life of the process.
/// Adding converters to it on every call grows the list without bound and, because List{T} is not thread safe,
/// two concurrent calls leave a null hole in it. From that point on Newtonsoft throws a NullReferenceException
/// while looking for a matching converter, and every error response of that process becomes an empty 500.
/// </remarks>
public class JsonSerializerSettingsIsolationTest
{
    private sealed class Payload
    {
        public string Name { get; set; } = string.Empty;
        public Instant When { get; set; }
    }

    private static JsonSerializerSettings BuildSettings()
    {
        return new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            Formatting = Formatting.None
        };
    }

    private static Payload BuildPayload()
    {
        return new Payload { Name = "John", When = Instant.FromUnixTimeSeconds(1_700_000_000) };
    }

    [Fact]
    public void Serialize_WithCallerSettings_DoesNotMutateThem()
    {
        // Arrange
        var settings = BuildSettings();
        var converters = settings.Converters.Count;

        // Act
        for (var i = 0; i < 10; i++)
            JsonSerializer.Serialize(BuildPayload(), settings);

        // Assert
        Assert.Equal(converters, settings.Converters.Count);
    }

    [Fact]
    public void Deserialize_WithCallerSettings_DoesNotMutateThem()
    {
        // Arrange
        var settings = BuildSettings();
        var converters = settings.Converters.Count;
        var json = JsonSerializer.Serialize(BuildPayload(), settings);

        // Act
        for (var i = 0; i < 10; i++)
            JsonSerializer.Deserialize<Payload>(json, settings);

        // Assert
        Assert.Equal(converters, settings.Converters.Count);
    }

    [Fact]
    public void Serialize_WithCallerSettings_StillAppliesNodaTimeConverters()
    {
        // Arrange
        var settings = BuildSettings();

        // Act
        var json = JsonSerializer.Serialize(BuildPayload(), settings);

        // Assert
        Assert.Contains("\"when\":\"2023-11-14T22:13:20Z\"", json);
    }

    [Fact]
    public void Serialize_ConcurrentCallsSharingSettings_DoesNotCorruptTheConverters()
    {
        // Arrange
        var settings = BuildSettings();
        var converters = settings.Converters.Count;
        var failures = new ConcurrentBag<Exception>();

        // Act
        Parallel.For(0, 8, _ =>
        {
            for (var i = 0; i < 500; i++)
            {
                try
                {
                    JsonSerializer.Serialize(BuildPayload(), settings);
                }
                catch (Exception ex)
                {
                    failures.Add(ex);
                }
            }
        });

        // Assert
        Assert.Empty(failures);
        Assert.Equal(converters, settings.Converters.Count);
        Assert.DoesNotContain(settings.Converters, converter => converter == null);
    }

    [Fact]
    public void CreateSettings_AppliesTheCallerConfigurationAndNodaTime()
    {
        // Arrange
        var resolver = new CamelCasePropertyNamesContractResolver();

        // Act
        var settings = JsonSerializer.CreateSettings(x => x.ContractResolver = resolver);
        var json = JsonSerializer.Serialize(BuildPayload(), settings);

        // Assert
        Assert.Same(resolver, settings.ContractResolver);
        Assert.NotEmpty(settings.Converters);
        Assert.Contains("\"when\":\"2023-11-14T22:13:20Z\"", json);
    }

    [Fact]
    public void CreateSettings_ReusedAcrossCalls_KeepsItsConvertersStable()
    {
        // Arrange
        var settings = JsonSerializer.CreateSettings();
        var converters = settings.Converters.Count;

        // Act
        for (var i = 0; i < 10; i++)
            JsonSerializer.Serialize(BuildPayload(), settings);

        // Assert
        Assert.Equal(converters, settings.Converters.Count);
    }
}
