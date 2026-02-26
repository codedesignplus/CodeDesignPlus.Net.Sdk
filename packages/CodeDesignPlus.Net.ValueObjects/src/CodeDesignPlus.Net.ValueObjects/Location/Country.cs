using System;

using CodeDesignPlus.Net.Exceptions.Guards;
using CodeDesignPlus.Net.ValueObjects.Financial;

namespace CodeDesignPlus.Net.ValueObjects.Location;

/// <summary>
/// Represents a country according to the ISO 3166-1 standard.
/// This immutable Value Object guarantees the structural integrity of geographic and financial relationships.
/// </summary>
public sealed class Country : IEquatable<Country>
{
    /// <summary>
    /// The unique identifier for the country.
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// The name of the country.
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// ISO 3166-1 Alpha-2 code (e.g., "US", "CO").
    /// </summary>
    public string Alpha2 { get; private set; }

    /// <summary>
    /// ISO 3166-1 Alpha-3 code (e.g., "USA", "COL").
    /// </summary>
    public string Alpha3 { get; private set; }

    /// <summary>
    /// ISO 3166-1 numeric code (e.g., 840 for USA, 170 for Colombia).
    /// </summary>
    public ushort Code { get; private set; }

    /// <summary>
    /// The primary timezone of the country.
    /// </summary>
    public string Timezone { get; private set; }

    /// <summary>
    /// The primary currency used in this country.
    /// </summary>
    public Currency Currency { get; private set; }

    [JsonConstructor]
    private Country(Guid id, string name, string alpha2, string alpha3, ushort code, string timezone, Currency currency)
    {
        var normalizedAlpha2 = alpha2?.Trim().ToUpperInvariant() ?? string.Empty;
        var normalizedAlpha3 = alpha3?.Trim().ToUpperInvariant() ?? string.Empty;

        Guard.GuidIsEmpty(id, Exceptions.Layer.None, "000 : Country ID cannot be empty.");
        Guard.IsNullOrEmpty(name, Exceptions.Layer.None, "001 : Country name cannot be empty.");

        Guard.IsNullOrEmpty(normalizedAlpha2, Exceptions.Layer.None, "002 : Country Alpha2 code cannot be empty.");
        Guard.IsFalse(normalizedAlpha2.Length == 2, Exceptions.Layer.None, "003 : Country Alpha2 code length is invalid.");

        Guard.IsNullOrEmpty(normalizedAlpha3, Exceptions.Layer.None, "004 : Country Alpha3 code cannot be empty.");
        Guard.IsFalse(normalizedAlpha3.Length == 3, Exceptions.Layer.None, "005 : Country Alpha3 code length is invalid.");

        Guard.IsNotInRange(code, 1, 999, Exceptions.Layer.None, "006 : Country numeric code is invalid.");

        Guard.IsNullOrEmpty(timezone, Exceptions.Layer.None, "007 : Country timezone cannot be empty.");

        Guard.IsNull(currency, Exceptions.Layer.None, "008 : Country currency is required.");

        this.Id = id;
        this.Name = name;
        this.Alpha2 = normalizedAlpha2;
        this.Alpha3 = normalizedAlpha3;
        this.Code = code;
        this.Timezone = timezone;
        this.Currency = currency;
    }

    /// <summary>
    /// Creates a new instance of the <see cref="Country"/> class.
    /// </summary>
    /// <param name="id">The unique identifier for the country.</param>
    /// <param name="name">The name of the country.</param>
    /// <param name="alpha2">ISO 3166-1 Alpha-2 code (e.g., "US", "CO").</param>
    /// <param name="alpha3">ISO 3166-1 Alpha-3 code (e.g., "USA", "COL").</param>
    /// <param name="code">ISO 3166-1 numeric code (e.g., 840 for USA, 170 for Colombia).</param>
    /// <param name="timezone">The primary timezone of the country.</param>
    /// <param name="currency">The primary currency used in this country.</param>
    /// <returns>A new instance of the <see cref="Country"/> class.</returns>
    public static Country Create(Guid id, string name, string alpha2, string alpha3, ushort code, string timezone, Currency currency)
    {
        return new Country(id, name, alpha2, alpha3, code, timezone, currency);
    }

    /// <summary>
    /// Determines whether two <see cref="Country"/> instances are equal.
    /// </summary>
    /// <param name="a">The first <see cref="Country"/> instance to compare.</param>
    /// <param name="b">The second <see cref="Country"/> instance to compare.</param>
    /// <returns><c>true</c> if the instances are equal; otherwise, <c>false</c>.</returns>
    public static bool operator ==(Country? a, Country? b)
    {
        if (ReferenceEquals(a, b))
            return true;

        if (a is null || b is null)
            return false;

        return a.Equals(b);
    }

    /// <summary>
    /// Determines whether two <see cref="Country"/> instances are not equal.
    /// </summary>
    /// <param name="a">The first <see cref="Country"/> instance to compare.</param>
    /// <param name="b">The second <see cref="Country"/> instance to compare.</param>
    /// <returns><c>true</c> if the instances are not equal; otherwise, <c>false</c>.</returns>
    public static bool operator !=(Country? a, Country? b) => !(a == b);

    /// <summary>
    /// Determines whether the specified <see cref="Country"/> is equal to the current <see cref="Country"/>.
    /// </summary>
    /// <param name="other">The <see cref="Country"/> instance to compare with the current instance.</param>
    /// <returns><c>true</c> if the specified <see cref="Country"/> is equal to the current instance; otherwise, <c>false</c>.</returns>
    public bool Equals(Country? other)
    {
        if (other is null) 
            return false;

        return this.Id == other.Id &&
               this.Name == other.Name &&
               this.Alpha2 == other.Alpha2 &&
               this.Alpha3 == other.Alpha3 &&
               this.Code == other.Code &&
               this.Timezone == other.Timezone &&
               this.Currency == other.Currency;
    }

    /// <summary>
    /// Determines whether the specified object is equal to the current <see cref="Country"/>.
    /// </summary>
    /// <param name="obj">The object to compare with the current <see cref="Country"/>.</param>
    /// <returns><c>true</c> if the specified object is equal to the current <see cref="Country"/>; otherwise, <c>false</c>.</returns>
    public override bool Equals(object? obj) => obj is Country other && Equals(other);

    /// <summary>
    /// Serves as the default hash function.
    /// </summary>
    /// <returns>A hash code for the current <see cref="Country"/>.</returns>
    public override int GetHashCode()
    {
        return HashCode.Combine(Id, Name, Alpha2, Alpha3, Code, Timezone, Currency);
    }
}