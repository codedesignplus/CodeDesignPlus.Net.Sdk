using CodeDesignPlus.Net.Exceptions.Guards;

namespace CodeDesignPlus.Net.ValueObjects.Location;

/// <summary>
/// Represents a comprehensive geographical location.
/// Aggregates all hierarchical geographic components into a single, immutable snapshot.
/// </summary>
public sealed class Location : IEquatable<Location>
{
    /// <summary>
    /// Gets the country component of the location.
    /// </summary>
    public Country Country { get; private set; }

    /// <summary>
    /// Gets the state, province, or department.
    /// </summary>
    public State State { get; private set; }

    /// <summary>
    /// Gets the city or municipality (e.g., "Bogotá").
    /// </summary>
    public City City { get; private set; }

    /// <summary>
    /// Gets the locality or administrative borough (e.g., "Kennedy", "Teusaquillo").
    /// </summary>
    public Locality Locality { get; private set; }

    /// <summary>
    /// Gets the specific neighborhood (e.g., "El Tintal", "La Fraguita").
    /// </summary>
    public Neighborhood Neighborhood { get; private set; }

    /// <summary>
    /// Gets the specific street address (e.g., "Carrera 86 # 6-37").
    /// </summary>
    public string Address { get; private set; }

    /// <summary>
    /// Gets the postal or zip code.
    /// </summary>
    public string PostalCode { get; private set; }

    [JsonConstructor]
    private Location(Country country, State state, City city, Locality locality, Neighborhood neighborhood, string address, string postalCode)
    {
        var normalizedAddress = address?.Trim() ?? string.Empty;
        var normalizedPostalCode = postalCode?.Trim() ?? string.Empty;

        Guard.IsNull(country, Exceptions.Layer.None, "000 : Country cannot be null.");
        Guard.IsNull(state, Exceptions.Layer.None, "001 : State cannot be null.");
        Guard.IsNull(city, Exceptions.Layer.None, "002 : City cannot be null.");
        Guard.IsNull(locality, Exceptions.Layer.None, "003 : Locality cannot be null.");
        Guard.IsNull(neighborhood, Exceptions.Layer.None, "004 : Neighborhood cannot be null.");

        Guard.IsNullOrEmpty(normalizedAddress, Exceptions.Layer.None, "005 : Address cannot be null or empty.");
        Guard.IsNullOrEmpty(normalizedPostalCode, Exceptions.Layer.None, "006 : Postal code cannot be null or empty.");

        this.Country = country;
        this.State = state;
        this.City = city;
        this.Locality = locality;
        this.Neighborhood = neighborhood;
        this.Address = normalizedAddress;
        this.PostalCode = normalizedPostalCode;
    }

    /// <summary>
    /// Creates a new immutable valid instance of the Location value object.
    /// </summary>
    /// <param name="country">The country component of the location.</param>
    /// <param name="state">The state, province, or department.</param>
    /// <param name="city">The city or municipality.</param>
    /// <param name="locality">The locality or administrative borough.</param>
    /// <param name="neighborhood">The specific neighborhood.</param>
    /// <param name="address">The specific street address.</param>
    /// <param name="postalCode">The postal or zip code.</param>
    /// <returns>A new instance of the Location value object.</returns>
    public static Location Create(Country country, State state, City city, Locality locality, Neighborhood neighborhood, string address, string postalCode)
    {
        return new Location(country, state, city, locality, neighborhood, address, postalCode);
    }

    /// <summary>
    /// Determines whether two Location instances are equal.
    /// </summary>
    /// <param name="a">The first Location instance.</param>
    /// <param name="b">The second Location instance.</param>
    /// <returns>True if the instances are equal; otherwise, false.</returns>
    public static bool operator ==(Location? a, Location? b)
    {
        if (ReferenceEquals(a, b)) 
            return true;

        if (a is null || b is null) 
            return false;

        return a.Equals(b);
    }

    /// <summary>
    /// Determines whether two Location instances are not equal.
    /// </summary>
    /// <param name="a">The first Location instance.</param>
    /// <param name="b">The second Location instance.</param>
    /// <returns>True if the instances are not equal; otherwise, false.</returns>
    public static bool operator !=(Location? a, Location? b) => !(a == b);

    /// <summary>
    /// Determines whether the current Location instance is equal to another Location instance.
    /// </summary>
    /// <param name="other">The other Location instance to compare with.</param>
    /// <returns>True if the instances are equal; otherwise, false.</returns>
    public bool Equals(Location? other)
    {
        if (other is null) return false;

        return this.Country == other.Country &&
               this.State == other.State &&
               this.City == other.City &&
               this.Locality == other.Locality &&
               this.Neighborhood == other.Neighborhood &&
               this.Address == other.Address &&
               this.PostalCode == other.PostalCode;
    }

    /// <summary>
    /// Determines whether the current Location instance is equal to another object.
    /// </summary>
    /// <param name="obj">The object to compare with the current Location instance.</param>
    /// <returns>True if the current Location instance is equal to the specified object; otherwise, false.</returns>
    public override bool Equals(object? obj) => obj is Location other && Equals(other);

    /// <summary>
    /// Returns a hash code for the current Location instance.
    /// </summary>
    /// <returns>A hash code for the current Location instance.</returns>
    public override int GetHashCode()
    {
        return HashCode.Combine(Country, State, City, Locality, Neighborhood, Address, PostalCode);
    }
}