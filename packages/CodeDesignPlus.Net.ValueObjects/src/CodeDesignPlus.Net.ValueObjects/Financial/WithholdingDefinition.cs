using CodeDesignPlus.Net.Exceptions.Guards;

namespace CodeDesignPlus.Net.ValueObjects.Financial;

/// <summary>
/// Represents a withholding tax definition (retención) applicable in certain jurisdictions.
/// Common examples: ReteFuente (Colombia), ISR (Mexico), Ganancias (Argentina).
/// The rate is stored in basis points to avoid floating-point precision issues
/// (e.g., 350 = 3.50%).
/// </summary>
/// <remarks>
/// The constructor only hydrates: Mongo, the cache and the deserialization of events and requests use it, and a read never
/// validates. The factory methods validate with <see cref="EnsureValid"/>.
/// </remarks>
public sealed class WithholdingDefinition : IEquatable<WithholdingDefinition>, IValidatableValueObject
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

    /// <summary>
    /// Gets the amount the rate applies to: the subtotal or the tax of the lines that form the base.
    /// </summary>
    /// <remarks>
    /// It is a member and not a constructor argument because the definitions stored before it existed do not carry it, and
    /// the driver discards a constructor whose arguments are not all in the document. The attribute lets the JSON
    /// deserialization of events, the cache and requests write it through its private setter.
    /// </remarks>
    [JsonProperty]
    public WithholdingBasis Basis { get; private set; }

    [JsonConstructor]
    private WithholdingDefinition(string code, string name, int rateBasisPoints, long minimumBase, string currency)
    {
        Code = code?.Trim().ToUpperInvariant() ?? string.Empty;
        Name = name;
        RateBasisPoints = rateBasisPoints;
        MinimumBase = minimumBase;
        Currency = currency?.Trim().ToUpperInvariant() ?? string.Empty;
    }

    /// <summary>
    /// Checks the code, the name, the rate, the minimum base, the currency and the basis of the withholding.
    /// </summary>
    /// <exception cref="Exceptions.CodeDesignPlusException">Thrown with the error of the first invariant that does not hold.</exception>
    public void EnsureValid()
    {
        Guard.IsNullOrEmpty(Code, Exceptions.Layer.None, Errors.CodeCannotBeNullOrEmpty);
        Guard.IsNullOrEmpty(Name, Exceptions.Layer.None, Errors.NameCannotBeNullOrEmpty);
        Guard.IsNotInRange(RateBasisPoints, 0, 100000, Exceptions.Layer.None, Errors.RateBasisPointsMustBeBetween0And100000);
        Guard.IsLessThan(MinimumBase, 0L, Exceptions.Layer.None, Errors.MinimumBaseCannotBeNegative);
        Guard.IsNullOrEmpty(Currency, Exceptions.Layer.None, Errors.CurrencyCannotBeNullOrEmpty);
        Guard.IsFalse(Currency.Length == 3, Exceptions.Layer.None, Errors.CurrencyMustBeExactly3CharactersISO4217);
        Guard.IsFalse(Enum.IsDefined(Basis), Exceptions.Layer.None, Errors.WithholdingBasisIsInvalid);
    }

    /// <summary>
    /// Creates a new immutable instance of the <see cref="WithholdingDefinition"/> value object.
    /// </summary>
    /// <param name="code">The withholding code (e.g., "RETE_FUENTE"). Normalized to uppercase.</param>
    /// <param name="name">The human-readable name.</param>
    /// <param name="rateBasisPoints">The rate in basis points (e.g., 350 = 3.50%).</param>
    /// <param name="minimumBase">Minimum transaction amount in minor units to apply this withholding. Zero = always applies.</param>
    /// <param name="currency">ISO 4217 currency code for the minimum base.</param>
    /// <param name="basis">The amount the rate applies to: the subtotal or the tax.</param>
    /// <returns>A new <see cref="WithholdingDefinition"/> instance.</returns>
    public static WithholdingDefinition Create(string code, string name, int rateBasisPoints, long minimumBase, string currency, WithholdingBasis basis)
    {
        var withholdingDefinition = new WithholdingDefinition(code, name, rateBasisPoints, minimumBase, currency)
        {
            Basis = basis
        };

        withholdingDefinition.EnsureValid();

        return withholdingDefinition;
    }

    /// <summary>
    /// Returns the withholding rate as a decimal factor (e.g., 350 basis points → 0.035m).
    /// </summary>
    /// <returns>The rate as a decimal factor.</returns>
    public decimal ToDecimalRate() => RateBasisPoints / 10000m;

    /// <summary>
    /// Determines whether the withholding applies to a document with the given subtotal.
    /// </summary>
    /// <param name="subtotalMinorUnits">The subtotal (value before taxes) of the lines that form the base, in minor units.</param>
    /// <returns>True if the subtotal meets the minimum threshold; otherwise, false.</returns>
    /// <remarks>
    /// The threshold is always compared against the subtotal, also when the rate applies to the tax: the minimum base of the
    /// VAT withholding is the value of the operation without VAT (DUR 1625 art. 1.3.2.1.12).
    /// </remarks>
    public bool AppliesTo(long subtotalMinorUnits) => subtotalMinorUnits >= MinimumBase;

    /// <summary>
    /// Calculates the withholding amount for the lines that form its base.
    /// Returns zero if the subtotal is below the minimum threshold.
    /// </summary>
    /// <param name="subtotalMinorUnits">The subtotal (value before taxes) of the lines that form the base, in minor units.</param>
    /// <param name="taxMinorUnits">The tax charged on those same lines, in minor units.</param>
    /// <returns>The calculated withholding amount in minor units, or zero if the threshold is not met.</returns>
    /// <exception cref="Exceptions.CodeDesignPlusException">Thrown when the stored basis is not a known one.</exception>
    public long CalculateWithholdingAmount(long subtotalMinorUnits, long taxMinorUnits)
    {
        if (!AppliesTo(subtotalMinorUnits))
            return 0L;

        var baseAmountMinorUnits = Basis switch
        {
            WithholdingBasis.Subtotal => subtotalMinorUnits,
            WithholdingBasis.Tax => taxMinorUnits,
            _ => throw new Exceptions.CodeDesignPlusException(Exceptions.Layer.None, Errors.WithholdingBasisIsInvalid)
        };

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
               Currency == other.Currency &&
               Basis == other.Basis;
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
    public override int GetHashCode() => HashCode.Combine(Code, Name, RateBasisPoints, MinimumBase, Currency, Basis);
}
