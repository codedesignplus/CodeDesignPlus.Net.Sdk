using System.Text.RegularExpressions;
using CodeDesignPlus.Net.Exceptions.Guards;

namespace CodeDesignPlus.Net.ValueObjects.User;

/// <summary>
/// Represents a simplified billing or shipping address snapshot.
/// This immutable Value Object captures essential address components for payment processing.
/// </summary>
/// <remarks>
/// The constructor only hydrates: Mongo, the cache and the deserialization of events and requests use it, and a read never
/// validates. The factory methods validate with <see cref="EnsureValid"/>.
/// </remarks>
public sealed partial class Address : IEquatable<Address>, IValidatableValueObject
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
        Street = street?.Trim() ?? string.Empty;
        Country = country?.Trim().ToUpperInvariant() ?? string.Empty;
        State = state?.Trim();
        City = city?.Trim() ?? string.Empty;
        PostalCode = postalCode?.Trim();
    }

    /// <summary>
    /// Checks the street, the ISO 3166-1 country code and the city, and the state and postal code when they are present.
    /// </summary>
    /// <exception cref="Exceptions.CodeDesignPlusException">Thrown with the error of the first invariant that does not hold.</exception>
    public void EnsureValid()
    {
        Guard.IsNullOrEmpty(Street, Exceptions.Layer.None, Errors.StreetCannotBeNullOrEmpty);
        Guard.IsGreaterThan(Street.Length, 100, Exceptions.Layer.None, Errors.StreetCannotBeGreaterThan100Characters);

        Guard.IsNullOrEmpty(Country, Exceptions.Layer.None, Errors.CountryCannotBeNullOrEmpty);
        Guard.IsFalse(CountryRegex().IsMatch(Country), Exceptions.Layer.None, Errors.CountryMustBeAValidISO31661);

        Guard.IsNullOrEmpty(City, Exceptions.Layer.None, Errors.CityCannotBeNullOrEmpty);
        Guard.IsGreaterThan(City.Length, 50, Exceptions.Layer.None, Errors.CityCannotBeGreaterThan50Characters);

        // Optional fields validation - only validate if provided
        if (State != null)
        {
            Guard.IsNullOrEmpty(State, Exceptions.Layer.None, Errors.StateCannotBeEmptyWhenProvided);
            Guard.IsGreaterThan(State.Length, 40, Exceptions.Layer.None, Errors.StateCannotBeGreaterThan40Characters);
        }

        if (PostalCode != null)
        {
            Guard.IsNullOrEmpty(PostalCode, Exceptions.Layer.None, Errors.PostalCodeCannotBeEmptyWhenProvided);
            Guard.IsFalse(PostalCodeRegex().IsMatch(PostalCode), Exceptions.Layer.None, Errors.PostalCodeMustContainOnlyDigits18);
        }
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
        var address = new Address(street, country, state, city, postalCode);

        address.EnsureValid();

        return address;
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
        var address = new Address(street, country, null, city, null);

        address.EnsureValid();

        return address;
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
        var address = new Address(street, country, state, city, null);

        address.EnsureValid();

        return address;
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
