using CodeDesignPlus.Net.Exceptions.Guards;

namespace CodeDesignPlus.Net.ValueObjects.Location;

/// <summary>
/// Represents a comprehensive geographical location.
/// Aggregates all hierarchical geographic components into a single, immutable snapshot.
/// </summary>
/// <remarks>
/// The constructor only hydrates: Mongo, the cache and the deserialization of events and requests use it, and a read never
/// validates. The factory methods validate with <see cref="EnsureValid"/>.
/// </remarks>
public sealed class Location : IEquatable<Location>, IValidatableValueObject
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
    /// Null when the city has no localities, which is the case for most municipalities.
    /// </summary>
    public Locality? Locality { get; private set; }

    /// <summary>
    /// Gets the specific neighborhood (e.g., "El Tintal", "La Fraguita").
    /// Null when the locality has no neighborhoods in the catalog. A neighborhood always comes with its locality.
    /// </summary>
    public Neighborhood? Neighborhood { get; private set; }

    /// <summary>
    /// Gets the specific street address (e.g., "Carrera 86 # 6-37").
    /// </summary>
    public string Address { get; private set; }

    /// <summary>
    /// Gets the postal or zip code.
    /// </summary>
    public string PostalCode { get; private set; }

    [JsonConstructor]
    private Location(Country country, State state, City city, Locality? locality, Neighborhood? neighborhood, string address, string postalCode)
    {
        Country = country;
        State = state;
        City = city;
        Locality = locality;
        Neighborhood = neighborhood;
        Address = address?.Trim() ?? string.Empty;
        PostalCode = postalCode?.Trim() ?? string.Empty;
    }

    /// <summary>
    /// Checks that the country, the state and the city are present, the address and the postal code, and each part of the
    /// location.
    /// </summary>
    /// <exception cref="Exceptions.CodeDesignPlusException">Thrown with the error of the first invariant that does not hold.</exception>
    public void EnsureValid()
    {
        Guard.IsNull(Country, Exceptions.Layer.None, Errors.CountryCannotBeNull);
        Guard.IsNull(State, Exceptions.Layer.None, Errors.StateCannotBeNull);
        Guard.IsNull(City, Exceptions.Layer.None, Errors.CityCannotBeNull);
        Guard.IsTrue(Neighborhood is not null && Locality is null, Exceptions.Layer.None, Errors.NeighborhoodRequiresLocality);

        Guard.IsNullOrEmpty(Address, Exceptions.Layer.None, Errors.AddressCannotBeNullOrEmpty);
        Guard.IsNullOrEmpty(PostalCode, Exceptions.Layer.None, Errors.PostalCodeCannotBeNullOrEmpty);

        Country.EnsureValid();
        State.EnsureValid();
        City.EnsureValid();
        Locality?.EnsureValid();
        Neighborhood?.EnsureValid();
    }

    /// <summary>
    /// Creates a new immutable valid instance of the Location value object.
    /// </summary>
    /// <param name="country">The country component of the location.</param>
    /// <param name="state">The state, province, or department.</param>
    /// <param name="city">The city or municipality.</param>
    /// <param name="locality">The locality or administrative borough, or null if the city has none.</param>
    /// <param name="neighborhood">The specific neighborhood, or null if the locality has none. Requires a locality.</param>
    /// <param name="address">The specific street address.</param>
    /// <param name="postalCode">The postal or zip code.</param>
    /// <returns>A new instance of the Location value object.</returns>
    public static Location Create(Country country, State state, City city, Locality? locality, Neighborhood? neighborhood, string address, string postalCode)
    {
        var location = new Location(country, state, city, locality, neighborhood, address, postalCode);

        location.EnsureValid();

        return location;
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