using CodeDesignPlus.Net.Exceptions.Guards;

namespace CodeDesignPlus.Net.ValueObjects.Location;

/// <summary>
/// Represents a locality, borough, or administrative district within a city.
/// Useful for modeling complex urban structures (e.g., Kennedy, Chapinero, or Usaquén).
/// </summary>
public sealed class Locality : IEquatable<Locality>
{
    /// <summary>
    /// Gets the unique identifier for the locality.
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Gets the full name of the locality.
    /// </summary>
    public string Name { get; private set; }

    [JsonConstructor]
    private Locality(Guid id, string name)
    {
        var normalizedName = name?.Trim() ?? string.Empty;

        Guard.GuidIsEmpty(id, Exceptions.Layer.None, "001 : Locality ID cannot be empty.");
        Guard.IsNullOrEmpty(normalizedName, Exceptions.Layer.None, "002 : Locality name cannot be null or empty.");

        this.Id = id;
        this.Name = normalizedName;
    }

    /// <summary>
    /// Creates a new immutable valid instance of the Locality value object.
    /// </summary>
    public static Locality Create(Guid id, string name)
    {
        return new Locality(id, name);
    }

    /// <summary>
    /// Checks if two Locality instances are equal in value.
    /// </summary>
    /// <param name="a">The first Locality instance.</param>
    /// <param name="b">The second Locality instance.</param>
    /// <returns>True if the instances are equal; otherwise, false.</returns>
    public static bool operator ==(Locality? a, Locality? b)
    {
        if (ReferenceEquals(a, b)) 
            return true;

        if (a is null || b is null) 
            return false;

        return a.Equals(b);
    }

    /// <summary>
    /// Checks if two Locality instances are not equal in value.
    /// </summary>
    /// <param name="a">The first Locality instance.</param>
    /// <param name="b">The second Locality instance.</param>
    /// <returns>True if the instances are not equal; otherwise, false.</returns>
    public static bool operator !=(Locality? a, Locality? b) => !(a == b);

    /// <summary>
    /// Checks if the current Locality instance is equal to another Locality instance.
    /// </summary>
    /// <param name="other">The Locality instance to compare with the current instance.</param>
    /// <returns>True if the current instance is equal to the specified Locality instance; otherwise, false.</returns>
    public bool Equals(Locality? other)
    {
        if (other is null) 
            return false;

        return this.Id == other.Id &&
               this.Name == other.Name;
    }

    /// <summary>
    /// Checks if the current Locality instance is equal to another object.
    /// </summary>
    /// <param name="obj">The object to compare with the current Locality instance.</param>
    /// <returns>True if the current instance is equal to the specified object; otherwise, false.</returns>
    public override bool Equals(object? obj) => obj is Locality other && Equals(other);

    /// <summary>
    /// Returns a hash code for the current Locality instance.
    /// </summary>
    /// <returns>A hash code for the current Locality instance.</returns>
    public override int GetHashCode()
    {
        return HashCode.Combine(Id, Name);
    }
}