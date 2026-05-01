using System.Text.RegularExpressions;
using CodeDesignPlus.Net.Exceptions.Guards;

namespace CodeDesignPlus.Net.ValueObjects.User;

/// <summary>
/// Represents a simplified billing or shipping address snapshot.
/// This immutable Value Object captures essential address components for payment processing.
/// </summary>
public sealed partial class Address : IEquatable<Address>
{
    [GeneratedRegex(@"^[A-Z]{2}$", RegexOptions.Compiled)]
    private static partial Regex CountryRegex();

    [GeneratedRegex(@"^\d{1,8}$", RegexOptions.Compiled)]
    private static partial Regex PostalCodeRegex();

    /// <summary>
    /// Gets the street address (e.g., "Carrera 86 # 6-37").
    /// </summary>
    public string Street { get; private set; }

    /// <summary>
    /// Gets the country code (ISO 3166-1 Alpha-2, e.g., "CO", "US").
    /// </summary>
    public string Country { get; private set; }

    /// <summary>
    /// Gets the state, province, or department (optional, depends on country).
    /// </summary>
    public string? State { get; private set; }

    /// <summary>
    /// Gets the city or municipality.
    /// </summary>
    public string City { get; private set; }

    /// <summary>
    /// Gets the postal or zip code (optional, not all countries use it).
    /// </summary>
    public string? PostalCode { get; private set; }

    [JsonConstructor]
    private Address(string street, string country, string? state, string city, string? postalCode)
    {
        var normalizedStreet = street?.Trim() ?? string.Empty;
        var normalizedCountry = country?.Trim().ToUpperInvariant() ?? string.Empty;
        var normalizedCity = city?.Trim() ?? string.Empty;

        Guard.IsNullOrEmpty(normalizedStreet, Exceptions.Layer.None, "000 : Street cannot be null or empty");
        Guard.IsGreaterThan(normalizedStreet.Length, 100, Exceptions.Layer.None, "001 : Street cannot be greater than 100 characters");

        Guard.IsNullOrEmpty(normalizedCountry, Exceptions.Layer.None, "002 : Country cannot be null or empty");
        Guard.IsFalse(CountryRegex().IsMatch(normalizedCountry), Exceptions.Layer.None, "003 : Country must be a valid ISO 3166-1 Alpha-2 code");

        Guard.IsNullOrEmpty(normalizedCity, Exceptions.Layer.None, "006 : City cannot be null or empty");
        Guard.IsGreaterThan(normalizedCity.Length, 50, Exceptions.Layer.None, "007 : City cannot be greater than 50 characters");

        // Optional fields validation - only validate if provided
        if (state != null)
        {
            var normalizedState = state.Trim();
            Guard.IsNullOrEmpty(normalizedState, Exceptions.Layer.None, "004 : State cannot be empty when provided");
            Guard.IsGreaterThan(normalizedState.Length, 40, Exceptions.Layer.None, "005 : State cannot be greater than 40 characters");
            this.State = normalizedState;
        }

        if (postalCode != null)
        {
            var normalizedPostalCode = postalCode.Trim();
            Guard.IsNullOrEmpty(normalizedPostalCode, Exceptions.Layer.None, "008 : Postal code cannot be empty when provided");
            Guard.IsFalse(PostalCodeRegex().IsMatch(normalizedPostalCode), Exceptions.Layer.None, "009 : Postal code must contain only digits (1-8 characters)");
            this.PostalCode = normalizedPostalCode;
        }

        this.Street = normalizedStreet;
        this.Country = normalizedCountry;
        this.City = normalizedCity;
    }

    /// <summary>
    /// Creates a new immutable snapshot of an address with all fields.
    /// Use this when you have complete address information from the user.
    /// </summary>
    /// <param name="street">The street address.</param>
    /// <param name="country">The country code (ISO 3166-1 Alpha-2).</param>
    /// <param name="state">The state, province, or department.</param>
    /// <param name="city">The city or municipality.</param>
    /// <param name="postalCode">The postal or zip code.</param>
    /// <returns>A new instance of the <see cref="Address"/> class.</returns>
    public static Address Create(string street, string country, string state, string city, string postalCode)
    {
        return new Address(street, country, state, city, postalCode);
    }

    /// <summary>
    /// Creates a minimal address snapshot with only essential fields.
    /// Use this for payment gateways with relaxed address requirements (e.g., Mercado Pago).
    /// State and PostalCode will be null.
    /// </summary>
    /// <param name="street">The street address.</param>
    /// <param name="country">The country code (ISO 3166-1 Alpha-2).</param>
    /// <param name="city">The city or municipality.</param>
    /// <returns>A new instance of the <see cref="Address"/> class.</returns>
    public static Address CreateMinimal(string street, string country, string city)
    {
        return new Address(street, country, null, city, null);
    }

    /// <summary>
    /// Creates an address snapshot without postal code (countries where it's not standardized).
    /// Use this for countries where postal codes are optional or not widely used.
    /// PostalCode will be null.
    /// </summary>
    /// <param name="street">The street address.</param>
    /// <param name="country">The country code (ISO 3166-1 Alpha-2).</param>
    /// <param name="state">The state, province, or department.</param>
    /// <param name="city">The city or municipality.</param>
    /// <returns>A new instance of the <see cref="Address"/> class.</returns>
    public static Address CreateWithoutPostalCode(string street, string country, string state, string city)
    {
        return new Address(street, country, state, city, null);
    }

    /// <summary>
    /// Determines whether two Address instances are equal.
    /// </summary>
    public static bool operator ==(Address? a, Address? b)
    {
        if (ReferenceEquals(a, b))
            return true;

        if (a is null || b is null)
            return false;

        return a.Equals(b);
    }

    /// <summary>
    /// Determines whether two Address instances are not equal.
    /// </summary>
    public static bool operator !=(Address? a, Address? b) => !(a == b);

    /// <summary>
    /// Determines whether the specified Address is equal to the current Address.
    /// </summary>
    public bool Equals(Address? other)
    {
        if (other is null)
            return false;

        return this.Street == other.Street &&
               this.Country == other.Country &&
               this.State == other.State &&
               this.City == other.City &&
               this.PostalCode == other.PostalCode;
    }

    /// <summary>
    /// Determines whether the specified object is equal to the current Address.
    /// </summary>
    public override bool Equals(object? obj) => obj is Address other && Equals(other);

    /// <summary>
    /// Returns a hash code for the current Address.
    /// </summary>
    public override int GetHashCode()
    {
        return HashCode.Combine(Street, Country, State, City, PostalCode);
    }
}
