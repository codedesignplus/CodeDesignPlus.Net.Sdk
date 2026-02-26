using System.Text.Json.Serialization;
using CodeDesignPlus.Net.Exceptions.Guards;

namespace CodeDesignPlus.Net.ValueObjects.Location;

/// <summary>
/// Represents a geographical city or municipality.
/// This immutable Value Object standardizes city details across the microservices ecosystem.
/// </summary>
public sealed class City : IEquatable<City>
{
    /// <summary>
    /// Gets the unique identifier for the city.
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Gets the full name of the city (e.g., "Bogotá", "New York").
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// Gets the timezone of the city, if applicable.
    /// Can be null if the timezone is managed at the State or Country level.
    /// </summary>
    public string? Timezone { get; private set; }

    [JsonConstructor]
    private City(Guid id, string name, string? timezone)
    {
        var normalizedName = name?.Trim() ?? string.Empty;
        var normalizedTimezone = timezone?.Trim();

        Guard.GuidIsEmpty(id, Exceptions.Layer.None, "001 : City ID cannot be empty.");
        Guard.IsNullOrEmpty(normalizedName, Exceptions.Layer.None, "002 : City name cannot be null or empty.");

        this.Id = id;
        this.Name = normalizedName;
        this.Timezone = normalizedTimezone;
    }

    /// <summary>
    /// Creates a new immutable valid instance of the City value object.
    /// </summary>
    public static City Create(Guid id, string name, string? timezone)
    {
        return new City(id, name, timezone);
    }

    /// <summary>
    /// Checks if two City instances are equal in value.
    /// </summary>
    /// <param name="a">The first City instance.</param>
    /// <param name="b">The second City instance.</param>
    /// <returns>True if the instances are equal; otherwise, false.</returns>
    public static bool operator ==(City? a, City? b)
    {
        if (ReferenceEquals(a, b)) return true;
        if (a is null || b is null) return false;

        return a.Equals(b);
    }

    /// <summary>
    /// Checks if two City instances are not equal in value.
    /// </summary>
    /// <param name="a">The first City instance.</param>
    /// <param name="b">The second City instance.</param>
    /// <returns>True if the instances are not equal; otherwise, false.</returns>
    public static bool operator !=(City? a, City? b) => !(a == b);

    /// <summary>
    /// Checks if the current City instance is equal to another City instance.
    /// </summary>
    /// <param name="other">The City instance to compare with the current instance.</param>
    /// <returns>True if the current instance is equal to the specified City instance; otherwise, false.</returns>
    public bool Equals(City? other)
    {
        if (other is null) return false;

        return this.Id == other.Id &&
               this.Name == other.Name &&
               this.Timezone == other.Timezone;
    }

    /// <summary>
    /// Checks if the current City instance is equal to another object.
    /// </summary>
    /// <param name="obj">The object to compare with the current City instance.</param>
    /// <returns>True if the current instance is equal to the specified object; otherwise, false.</returns>
    public override bool Equals(object? obj) => obj is City other && Equals(other);

    /// <summary>
    /// Returns a hash code for the current City instance.
    /// </summary>
    /// <returns>A hash code for the current City instance.</returns>
    public override int GetHashCode()
    {
        return HashCode.Combine(Id, Name, Timezone);
    }
}