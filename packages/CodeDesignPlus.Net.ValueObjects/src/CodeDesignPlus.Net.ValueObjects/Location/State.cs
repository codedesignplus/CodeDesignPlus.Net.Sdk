using CodeDesignPlus.Net.Exceptions.Guards;

namespace CodeDesignPlus.Net.ValueObjects.Location;

/// <summary>
/// Represents a geographical state, province, or department.
/// This immutable Value Object standardizes location details across the microservices ecosystem.
/// </summary>
public sealed class State : IEquatable<State>
{
    /// <summary>
    /// Gets the unique identifier for the state.
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Gets the full name of the state, province, or department (e.g., "Cundinamarca", "California").
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// Gets the standard code or abbreviation for the state (e.g., "CUN", "CA").
    /// </summary>
    public string Code { get; private set; }

    [JsonConstructor]
    private State(Guid id, string name, string code)
    {
        var normalizedName = name?.Trim() ?? string.Empty;
        var normalizedCode = code?.Trim().ToUpperInvariant() ?? string.Empty;

        Guard.GuidIsEmpty(id, Exceptions.Layer.None, "001 : State ID cannot be empty.");
        Guard.IsNullOrEmpty(normalizedName, Exceptions.Layer.None, "002 : State name cannot be null or empty.");
        Guard.IsNullOrEmpty(normalizedCode, Exceptions.Layer.None, "003 : State code cannot be null or empty.");

        this.Id = id;
        this.Name = normalizedName;
        this.Code = normalizedCode;
    }

    /// <summary>
    /// Creates a new immutable valid instance of the State value object.
    /// </summary>
    public static State Create(Guid id, string name, string code)
    {
        return new State(id, name, code);
    }

    /// <summary>
    /// Checks if two State instances are equal in value.
    /// </summary>
    /// <param name="a">The first State instance.</param>
    /// <param name="b">The second State instance.</param>
    /// <returns>True if the instances are equal; otherwise, false.</returns>
    public static bool operator ==(State? a, State? b)
    {
        if (ReferenceEquals(a, b)) 
            return true;

        if (a is null || b is null) 
            return false;

        return a.Equals(b);
    }

    /// <summary>
    /// Checks if two State instances are not equal in value.
    /// </summary>
    /// <param name="a">The first State instance.</param>
    /// <param name="b">The second State instance.</param>
    /// <returns>True if the instances are not equal; otherwise, false.</returns>
    public static bool operator !=(State? a, State? b) => !(a == b);

    /// <summary>
    /// Checks if the current State instance is equal to another State instance.
    /// </summary>
    /// <param name="other">The State instance to compare with the current instance.</param>
    /// <returns>True if the current instance is equal to the specified State instance; otherwise, false.</returns>
    public bool Equals(State? other)
    {
        if (other is null) 
            return false;

        return this.Id == other.Id &&
               this.Name == other.Name &&
               this.Code == other.Code;
    }

    /// <summary>
    /// Checks if the current State instance is equal to another object.
    /// </summary>
    /// <param name="obj">The object to compare with the current State instance.</param>
    /// <returns>True if the current instance is equal to the specified object; otherwise, false.</returns>
    public override bool Equals(object? obj) => obj is State other && Equals(other);

    /// <summary>
    /// Returns a hash code for the current State instance.
    /// </summary>
    /// <returns>A hash code for the current State instance.</returns>
    public override int GetHashCode()
    {
        return HashCode.Combine(Id, Name, Code);
    }
}