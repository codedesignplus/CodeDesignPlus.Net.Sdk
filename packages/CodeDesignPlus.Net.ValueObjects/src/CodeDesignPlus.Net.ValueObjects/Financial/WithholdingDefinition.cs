using CodeDesignPlus.Net.Exceptions.Guards;

namespace CodeDesignPlus.Net.ValueObjects.Financial;

/// <summary>
/// Represents a withholding tax definition (retención) applicable in certain jurisdictions.
/// Common examples: ReteFuente (Colombia), ISR (Mexico), Ganancias (Argentina).
/// The rate is stored in basis points to avoid floating-point precision issues
/// (e.g., 350 = 3.50%).
/// </summary>
public sealed class WithholdingDefinition : IEquatable<WithholdingDefinition>
{
    /// <summary>
    /// Gets the withholding code (e.g., "RETE_FUENTE", "RETE_ICA", "ISR").
    /// </summary>
    public string Code { get; private set; }

    /// <summary>
    /// Gets the human-readable name of the withholding.
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// Gets the withholding rate expressed in basis points (e.g., 350 = 3.50%).
    /// Must be between 0 and 100000.
    /// </summary>
    public int RateBasisPoints { get; private set; }

    /// <summary>
    /// Gets the minimum transaction base amount (in minor units) required to apply this withholding.
    /// Zero means it applies to all amounts.
    /// </summary>
    public long MinimumBase { get; private set; }

    /// <summary>
    /// Gets the ISO 4217 currency code for the minimum base amount (e.g., "COP", "MXN").
    /// </summary>
    public string Currency { get; private set; }

    [JsonConstructor]
    private WithholdingDefinition(string code, string name, int rateBasisPoints, long minimumBase, string currency)
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
        MinimumBase = minimumBase;
        Currency = normalizedCurrency;
    }

    /// <summary>
    /// Creates a new immutable instance of the <see cref="WithholdingDefinition"/> value object.
    /// </summary>
    /// <param name="code">The withholding code (e.g., "RETE_FUENTE"). Normalized to uppercase.</param>
    /// <param name="name">The human-readable name.</param>
    /// <param name="rateBasisPoints">The rate in basis points (e.g., 350 = 3.50%).</param>
    /// <param name="minimumBase">Minimum transaction amount in minor units to apply this withholding. Zero = always applies.</param>
    /// <param name="currency">ISO 4217 currency code for the minimum base.</param>
    /// <returns>A new <see cref="WithholdingDefinition"/> instance.</returns>
    public static WithholdingDefinition Create(string code, string name, int rateBasisPoints, long minimumBase, string currency)
    {
        return new WithholdingDefinition(code, name, rateBasisPoints, minimumBase, currency);
    }

    /// <summary>
    /// Returns the withholding rate as a decimal factor (e.g., 350 basis points → 0.035m).
    /// </summary>
    /// <returns>The rate as a decimal factor.</returns>
    public decimal ToDecimalRate() => RateBasisPoints / 10000m;

    /// <summary>
    /// Determines whether the withholding applies to the given base amount.
    /// </summary>
    /// <param name="baseAmountMinorUnits">The transaction base in minor units.</param>
    /// <returns>True if the base meets the minimum threshold; otherwise, false.</returns>
    public bool AppliesTo(long baseAmountMinorUnits) => baseAmountMinorUnits >= MinimumBase;

    /// <summary>
    /// Calculates the withholding amount for a given base in minor units.
    /// Returns zero if the base is below the minimum threshold.
    /// </summary>
    /// <param name="baseAmountMinorUnits">The base amount in minor units.</param>
    /// <returns>The calculated withholding amount in minor units, or zero if the threshold is not met.</returns>
    public long CalculateWithholdingAmount(long baseAmountMinorUnits)
    {
        if (!AppliesTo(baseAmountMinorUnits))
            return 0L;

        return (long)Math.Round(baseAmountMinorUnits * ToDecimalRate(), MidpointRounding.AwayFromZero);
    }

    /// <summary>
    /// Returns true if two <see cref="WithholdingDefinition"/> instances are equal.
    /// </summary>
    /// <param name="a">The first instance.</param>
    /// <param name="b">The second instance.</param>
    /// <returns>True if the instances are equal; otherwise, false.</returns>
    public static bool operator ==(WithholdingDefinition? a, WithholdingDefinition? b)
    {
        if (ReferenceEquals(a, b)) return true;
        if (a is null || b is null) return false;
        return a.Equals(b);
    }

    /// <summary>
    /// Returns true if two <see cref="WithholdingDefinition"/> instances are not equal.
    /// </summary>
    /// <param name="a">The first instance.</param>
    /// <param name="b">The second instance.</param>
    /// <returns>True if the instances are not equal; otherwise, false.</returns>
    public static bool operator !=(WithholdingDefinition? a, WithholdingDefinition? b) => !(a == b);

    /// <summary>
    /// Returns true if this instance is equal to another <see cref="WithholdingDefinition"/>.
    /// </summary>
    /// <param name="other">The other instance to compare to.</param>
    /// <returns>True if the instances are equal; otherwise, false.</returns>
    public bool Equals(WithholdingDefinition? other)
    {
        if (other is null) return false;
        return Code == other.Code &&
               Name == other.Name &&
               RateBasisPoints == other.RateBasisPoints &&
               MinimumBase == other.MinimumBase &&
               Currency == other.Currency;
    }

    /// <summary>
    /// Returns true if this instance is equal to another object.
    /// </summary>
    /// <param name="obj">The object to compare to.</param>
    /// <returns>True if the objects are equal; otherwise, false.</returns>
    public override bool Equals(object? obj) => obj is WithholdingDefinition other && Equals(other);

    /// <summary>
    /// Returns the hash code for this instance.
    /// </summary>
    /// <returns>The hash code.</returns>
    public override int GetHashCode() => HashCode.Combine(Code, Name, RateBasisPoints, MinimumBase, Currency);
}
