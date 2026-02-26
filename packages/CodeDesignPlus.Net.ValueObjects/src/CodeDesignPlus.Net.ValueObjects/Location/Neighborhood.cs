using System.Text.Json.Serialization;
using CodeDesignPlus.Net.Exceptions.Guards;

namespace CodeDesignPlus.Net.ValueObjects.Location;

/// <summary>
/// Represents a specific neighborhood or subdivision within a locality or city.
/// Examples include specific zones like El Tintal, La Fraguita, or Perdomo Alto.
/// </summary>
public sealed class Neighborhood : IEquatable<Neighborhood>
{
    /// <summary>
    /// Gets the unique identifier for the neighborhood.
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Gets the full name of the neighborhood.
    /// </summary>
    public string Name { get; private set; }

    [JsonConstructor]
    private Neighborhood(Guid id, string name)
    {
        var normalizedName = name?.Trim() ?? string.Empty;

        Guard.GuidIsEmpty(id, Exceptions.Layer.None, "001 : Neighborhood ID cannot be empty.");
        Guard.IsNullOrEmpty(normalizedName, Exceptions.Layer.None, "002 : Neighborhood name cannot be null or empty.");

        this.Id = id;
        this.Name = normalizedName;
    }

    /// <summary>
    /// Creates a new immutable valid instance of the Neighborhood value object.
    /// </summary>
    public static Neighborhood Create(Guid id, string name)
    {
        return new Neighborhood(id, name);
    }


    /// <summary>
    /// Checks if two Neighborhood instances are equal in value.
    /// </summary>
    /// <param name="a">The first Neighborhood instance.</param>
    /// <param name="b">The second Neighborhood instance.</param>
    /// <returns>True if the instances are equal; otherwise, false.</returns>
    public static bool operator ==(Neighborhood? a, Neighborhood? b)
    {
        if (ReferenceEquals(a, b)) return true;
        if (a is null || b is null) return false;

        return a.Equals(b);
    }

    /// <summary>
    /// Checks if two Neighborhood instances are not equal in value.
    /// </summary>
    /// <param name="a">The first Neighborhood instance.</param>
    /// <param name="b">The second Neighborhood instance.</param>
    /// <returns>True if the instances are not equal; otherwise, false.</returns>
    public static bool operator !=(Neighborhood? a, Neighborhood? b) => !(a == b);

    /// <summary>
    /// Checks if the current Neighborhood instance is equal to another Neighborhood instance.
    /// </summary>
    /// <param name="other">The Neighborhood instance to compare with the current instance.</param>
    /// <returns>True if the current instance is equal to the specified Neighborhood instance; otherwise, false.</returns>
    public bool Equals(Neighborhood? other)
    {
        if (other is null) return false;

        return this.Id == other.Id &&
               this.Name == other.Name;
    }

    /// <summary>
    /// Checks if the current Neighborhood instance is equal to another object.
    /// </summary>
    /// <param name="obj">The object to compare with the current Neighborhood instance.</param>
    /// <returns>True if the current instance is equal to the specified object; otherwise, false.</returns>
    public override bool Equals(object? obj) => obj is Neighborhood other && Equals(other);

    /// <summary>
    /// Returns a hash code for the current Neighborhood instance.
    /// </summary>
    /// <returns>A hash code for the current Neighborhood instance.</returns>
    public override int GetHashCode()
    {
        return HashCode.Combine(Id, Name);
    }
}