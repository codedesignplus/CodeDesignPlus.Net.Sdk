using CodeDesignPlus.Net.Exceptions.Guards;

namespace CodeDesignPlus.Net.ValueObjects.Financial;

/// <summary>
/// Represents a tax definition applicable to a financial product or service.
/// The rate is stored in basis points to avoid floating-point precision issues
/// (e.g., 1900 = 19.00%, 700 = 7.00%).
/// </summary>
public sealed class TaxDefinition : IEquatable<TaxDefinition>
{
    /// <summary>
    /// Gets the tax code (e.g., "IVA", "IGV", "ITBMS", "VAT").
    /// </summary>
    public string Code { get; private set; }

    /// <summary>
    /// Gets the human-readable name of the tax (e.g., "Impuesto al Valor Agregado").
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// Gets the tax rate expressed in basis points (e.g., 1900 = 19.00%, 700 = 7.00%).
    /// Must be between 0 and 100000 (0% to 1000%).
    /// </summary>
    public int RateBasisPoints { get; private set; }

    /// <summary>
    /// Gets a value indicating whether the tax is already included in the base price (true)
    /// or should be added on top of it (false).
    /// </summary>
    public bool IsInclusive { get; private set; }

    /// <summary>
    /// Gets the minimum transaction base amount (in minor units) required to apply this tax.
    /// Zero means it applies to all amounts.
    /// </summary>
    public long MinimumBase { get; private set; }

    /// <summary>
    /// Gets the ISO 4217 currency code for the minimum base amount (e.g., "COP", "USD").
    /// </summary>
    public string Currency { get; private set; }

    [JsonConstructor]
    private TaxDefinition(string code, string name, int rateBasisPoints, bool isInclusive, long minimumBase, string currency)
    {
        var normalizedCode = code?.Trim().ToUpperInvariant() ?? string.Empty;
        var normalizedCurrency = currency?.Trim().ToUpperInvariant() ?? string.Empty;

        Guard.IsNullOrEmpty(normalizedCode, Exceptions.Layer.None, "000 : Code cannot be null or empty.");
        Guard.IsNullOrEmpty(name, Exceptions.Layer.None, "001 : Name cannot be null or empty.");
        Guard.IsNotInRange(rateBasisPoints, 0, 100000, Exceptions.Layer.None, "002 : RateBasisPoints must be between 0 and 100000.");
        Guard.IsLessThan(minimumBase, 0L, Exceptions.Layer.None, "003 : MinimumBase cannot be negative.");
        Guard.IsNullOrEmpty(normalizedCurrency, Exceptions.Layer.None, "004 : Currency cannot be null or empty.");
        Guard.IsFalse(normalizedCurrency.Length == 3, Exceptions.Layer.None, "005 : Currency must be exactly 3 characters (ISO 4217).");

        Code = normalizedCode;
        Name = name;
        RateBasisPoints = rateBasisPoints;
        IsInclusive = isInclusive;
        MinimumBase = minimumBase;
        Currency = normalizedCurrency;
    }

    /// <summary>
    /// Creates a new immutable instance of the <see cref="TaxDefinition"/> value object with minimum base threshold.
    /// </summary>
    /// <param name="code">The tax code (e.g., "IVA", "VAT"). Normalized to uppercase.</param>
    /// <param name="name">The human-readable name of the tax.</param>
    /// <param name="rateBasisPoints">The rate in basis points (e.g., 1900 = 19.00%).</param>
    /// <param name="isInclusive">Whether the tax is included in the price or added on top.</param>
    /// <param name="minimumBase">Minimum transaction amount in minor units to apply this tax. Zero = always applies.</param>
    /// <param name="currency">ISO 4217 currency code for the minimum base.</param>
    /// <returns>A new <see cref="TaxDefinition"/> instance.</returns>
    public static TaxDefinition Create(string code, string name, int rateBasisPoints, bool isInclusive, long minimumBase, string currency)
    {
        return new TaxDefinition(code, name, rateBasisPoints, isInclusive, minimumBase, currency);
    }

    /// <summary>
    /// Returns the tax rate as a decimal factor (e.g., 1900 basis points → 0.19m).
    /// </summary>
    /// <returns>The rate as a decimal factor.</returns>
    public decimal ToDecimalRate() => RateBasisPoints / 10000m;

    /// <summary>
    /// Calculates the tax amount for a given base price in minor units.
    /// </summary>
    /// <param name="baseAmountMinorUnits">The base amount in minor units (e.g., cents).</param>
    /// <returns>The calculated tax amount in minor units.</returns>
    public long CalculateTaxAmount(long baseAmountMinorUnits)
    {
        return (long)Math.Round(baseAmountMinorUnits * ToDecimalRate(), MidpointRounding.AwayFromZero);
    }

    /// <summary>
    /// Determines whether the tax applies to the given base amount.
    /// </summary>
    /// <param name="baseAmountMinorUnits">The transaction base in minor units.</param>
    /// <returns>True if the base meets the minimum threshold; otherwise, false.</returns>
    public bool AppliesTo(long baseAmountMinorUnits) => MinimumBase == 0 || baseAmountMinorUnits >= MinimumBase;

    /// <summary>
    /// Calculates the tax amount for a given base in minor units.
    /// Returns zero if the base is below the minimum threshold.
    /// </summary>
    /// <param name="baseAmountMinorUnits">The base amount in minor units.</param>
    /// <returns>The calculated tax amount in minor units, or zero if the threshold is not met.</returns>
    public long CalculateTaxAmountIfApplicable(long baseAmountMinorUnits)
    {
        if (!AppliesTo(baseAmountMinorUnits))
            return 0L;

        return CalculateTaxAmount(baseAmountMinorUnits);
    }

    /// <summary>
    /// Returns true if two <see cref="TaxDefinition"/> instances are equal.
    /// </summary>
    /// <param name="a">The first instance.</param>
    /// <param name="b">The second instance.</param>
    /// <returns>True if the instances are equal; otherwise, false.</returns>
    public static bool operator ==(TaxDefinition? a, TaxDefinition? b)
    {
        if (ReferenceEquals(a, b)) return true;
        if (a is null || b is null) return false;
        return a.Equals(b);
    }

    /// <summary>
    /// Returns true if two <see cref="TaxDefinition"/> instances are not equal.
    /// </summary>
    /// <param name="a">The first instance.</param>
    /// <param name="b">The second instance.</param>
    /// <returns>True if the instances are not equal; otherwise, false.</returns>
    public static bool operator !=(TaxDefinition? a, TaxDefinition? b) => !(a == b);

    /// <summary>
    /// Returns true if this instance is equal to another <see cref="TaxDefinition"/>.
    /// </summary>
    /// <param name="other">The other instance to compare to.</param>
    /// <returns>True if the instances are equal; otherwise, false.</returns>
    public bool Equals(TaxDefinition? other)
    {
        if (other is null) return false;
        return Code == other.Code &&
               Name == other.Name &&
               RateBasisPoints == other.RateBasisPoints &&
               IsInclusive == other.IsInclusive &&
               MinimumBase == other.MinimumBase &&
               Currency == other.Currency;
    }

    /// <summary>
    /// Returns true if this instance is equal to another object.
    /// </summary>
    /// <param name="obj">The object to compare to.</param>
    /// <returns>True if the objects are equal; otherwise, false.</returns>
    public override bool Equals(object? obj) => obj is TaxDefinition other && Equals(other);

    /// <summary>
    /// Returns the hash code for this instance.
    /// </summary>
    /// <returns>The hash code.</returns>
    public override int GetHashCode() => HashCode.Combine(Code, Name, RateBasisPoints, IsInclusive, MinimumBase, Currency);
}
