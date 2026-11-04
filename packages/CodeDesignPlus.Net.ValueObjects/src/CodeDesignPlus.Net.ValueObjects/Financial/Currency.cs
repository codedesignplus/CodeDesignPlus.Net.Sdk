using CodeDesignPlus.Net.Exceptions.Guards;

namespace CodeDesignPlus.Net.ValueObjects.Financial;

/// <summary>
/// Represents a currency with its associated properties such as code, symbol, and decimal digits.
/// This immutable Value Object enforces ISO 4217 standards to prevent precision loss in financial calculations.
/// </summary>
/// <remarks>
/// The constructor only hydrates: Mongo, the cache and the deserialization of events and requests use it, and a read never
/// validates. The factory methods validate with <see cref="EnsureValid"/>.
/// </remarks>
public sealed class Currency : IEquatable<Currency>, IValidatableValueObject
{
    /// <summary>
    /// Gets the unique identifier for the currency.
    /// </summary>
    public Guid CurrencyId { get; private set; }

    /// <summary>
    /// Gets the alphabetic currency code according to the ISO 4217 standard (e.g., USD, COP, EUR).
    /// </summary>
    public string Code { get; private set; }

    /// <summary>
    /// Gets the three-digit numeric currency code according to the ISO 4217 standard (e.g., 840 for USD).
    /// </summary>
    public short NumericCode { get; private set; }

    /// <summary>
    /// Gets the exponent or number of decimal digits for the currency.
    /// Defines the minor unit used for financial calculations to prevent precision loss.
    /// E.g., USD = 2 (cents), JPY = 0 (yen), KWD = 3 (fils).
    /// </summary>
    public short DecimalDigits { get; private set; }

    /// <summary>
    /// Gets the graphical symbol commonly used for the currency (e.g., $, €, ¥).
    /// </summary>
    public string Symbol { get; private set; }

    /// <summary>
    /// Gets the human-readable name of the currency (e.g., US Dollar, Colombian Peso).
    /// </summary>
    public string Name { get; private set; }

    [JsonConstructor]
    private Currency(Guid currencyId, string name, string code, string symbol, short decimalDigits, short numericCode)
    {
        this.CurrencyId = currencyId;
        this.Name = name;
        this.Code = code?.Trim().ToUpperInvariant() ?? string.Empty;
        this.Symbol = symbol;
        this.DecimalDigits = decimalDigits;
        this.NumericCode = numericCode;
    }

    /// <summary>
    /// Checks the identifier, the name, the ISO 4217 codes, the symbol and the decimal digits of the currency.
    /// </summary>
    /// <exception cref="Exceptions.CodeDesignPlusException">Thrown with the error of the first invariant that does not hold.</exception>
    public void EnsureValid()
    {
        Guard.GuidIsEmpty(CurrencyId, Exceptions.Layer.None, Errors.CurrencyIdIsEmpty);
        Guard.IsNullOrEmpty(Name, Exceptions.Layer.None, Errors.NameIsRequired);

        Guard.IsNullOrEmpty(Code, Exceptions.Layer.None, Errors.CurrencyCodeIsRequired.With(Name, Code));
        Guard.IsFalse(Code.Length == 3, Exceptions.Layer.None, Errors.CurrencyCodeLengthIsInvalid.With(Name, Code));

        Guard.IsNullOrEmpty(Symbol, Exceptions.Layer.None, Errors.CurrencySymbolIsRequired.With(Name, Code));

        Guard.IsNotInRange(NumericCode, 1, 999, Exceptions.Layer.None, Errors.CurrencyNumericCodeIsInvalid.With(Name, Code));

        Guard.IsLessThan(DecimalDigits, 0, Exceptions.Layer.None, Errors.CurrencyDecimalDigitsAreInvalid.With(Name, Code));
    }

    /// <summary>
    /// Creates a new immutable valid instance of the Currency value object.
    /// </summary>
    public static Currency Create(Guid currencyId, string name, string code, string symbol, short decimalDigits, short numericCode)
    {
        var currency = new Currency(currencyId, name, code, symbol, decimalDigits, numericCode);

        currency.EnsureValid();

        return currency;
    }

    /// <summary>
    /// Determines whether two Currency instances are equal.
    /// </summary>
    /// <param name="a">The first Currency instance.</param>
    /// <param name="b">The second Currency instance.</param>
    /// <returns>True if the instances are equal; otherwise, false.</returns>
    public static bool operator ==(Currency? a, Currency? b)
    {
        if (ReferenceEquals(a, b)) return true;
        if (a is null || b is null) return false;

        return a.Equals(b);
    }

    /// <summary>
    /// Determines whether two Currency instances are not equal.
    /// </summary>
    /// <param name="a">The first Currency instance.</param>
    /// <param name="b">The second Currency instance.</param>
    /// <returns>True if the instances are not equal; otherwise, false.</returns>
    public static bool operator !=(Currency? a, Currency? b) => !(a == b);

    /// <summary>
    /// Determines whether the current Currency instance is equal to another Currency instance.
    /// </summary>
    /// <param name="other">The Currency instance to compare with the current instance.</param>
    /// <returns>True if the instances are equal; otherwise, false.</returns>
    public bool Equals(Currency? other)
    {
        if (other is null) return false;

        return this.CurrencyId == other.CurrencyId &&
               this.Code == other.Code &&
               this.NumericCode == other.NumericCode &&
               this.DecimalDigits == other.DecimalDigits &&
               this.Symbol == other.Symbol &&
               this.Name == other.Name;
    }

    /// <summary>
    /// Determines whether the current Currency instance is equal to another object.
    /// </summary>
    /// <param name="obj">The object to compare with the current instance.</param>
    /// <returns>True if the instances are equal; otherwise, false.</returns>
    public override bool Equals(object? obj) => obj is Currency other && Equals(other);

    /// <summary>
    /// Returns a hash code for the current Currency instance.
    /// </summary>
    /// <returns>A hash code for the current instance.</returns>
    public override int GetHashCode()
    {
        return HashCode.Combine(CurrencyId, Code, NumericCode, DecimalDigits, Symbol, Name);
    }
}