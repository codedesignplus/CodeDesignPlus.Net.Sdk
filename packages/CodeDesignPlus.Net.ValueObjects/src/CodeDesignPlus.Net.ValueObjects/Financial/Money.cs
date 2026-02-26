using CodeDesignPlus.Net.Exceptions.Guards;

namespace CodeDesignPlus.Net.ValueObjects.Financial;

/// <summary>
/// Represents a monetary amount with its currency using the Minor Unit Pattern.
/// Uses 'long' internally to represent the smallest currency unit (e.g., cents)
/// to prevent floating-point precision loss in distributed systems and payment gateways.
/// Currency properties are based on the ISO 4217 international standard.
/// </summary>
public sealed class Money : IEquatable<Money>
{
    /// <summary>
    /// The amount of money in the minor unit (e.g., 12345 cents instead of 123.45).
    /// </summary>
    public long Amount { get; private set; }

    /// <summary>
    /// The 3-letter currency code, according to ISO 4217 (e.g., "COP", "USD").
    /// </summary>
    public string Currency { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Money"/> class directly from a minor unit amount.
    /// </summary>
    /// <param name="amount">The amount of money in the minor unit.</param>
    /// <param name="currency">The currency code.</param>
    [JsonConstructor]
    private Money(long amount, string currency)
    {
        var normalizedCurrency = currency?.Trim().ToUpperInvariant() ?? string.Empty;
        
        Guard.IsNullOrEmpty(normalizedCurrency, Exceptions.Layer.None, "000 : Currency code cannot be null or empty.");
        Guard.IsFalse(normalizedCurrency.Length == 3, Exceptions.Layer.None, "001 : Currency code must be exactly 3 characters (ISO 4217).");

        Amount = amount;
        Currency = normalizedCurrency;
    }
    /// <summary>
    /// Creates a Money instance from a decimal value representing the main unit (e.g., 10.50 USD).
    /// Multiplies the decimal by the specified factor to store it as a long minor unit.
    /// </summary>
    /// <param name="amountInMainUnit">The amount in the main unit (e.g., 10.50m).</param>
    /// <param name="currency">The currency code (ISO 4217).</param>
    /// <param name="decimalPlaces">The number of decimal places for the currency (e.g., 2 for USD).</param>
    /// <returns>A new Money instance.</returns>
    public static Money FromDecimal(decimal amountInMainUnit, string currency, short decimalPlaces)
    {
        Guard.IsLessThan(decimalPlaces, (short)0, Exceptions.Layer.None, "002 : Decimal places cannot be negative.");

        var factor = (decimal)Math.Pow(10, decimalPlaces);
        var minorUnitAmount = (long)Math.Round(amountInMainUnit * factor, MidpointRounding.AwayFromZero);

        return new Money(minorUnitAmount, currency);
    }

    /// <summary>
    /// Creates a Money instance from a long representing the minor unit (e.g., 1050 cents).
    /// </summary>
    /// <param name="amountInMinorUnit">The amount in the minor unit.</param>
    /// <param name="currency">The currency code (ISO 4217).</param>
    /// <returns>A new Money instance.</returns>
    public static Money FromLong(long amountInMinorUnit, string currency)
    {        
        return new Money(amountInMinorUnit, currency);
    }

    /// <summary>
    /// Creates a Money instance with zero value for an unspecified currency.
    /// Uses "XXX" which is the ISO 4217 standard code for "No currency".
    /// </summary>
    /// <summary>
    /// Creates a Money instance with zero value for an unspecified currency.
    /// Uses "XXX" which is the ISO 4217 standard code for "No currency".
    /// </summary>
    /// <returns>A Money instance with zero value and "XXX" currency.</returns>
    public static Money Zero()
    {
        return new Money(0L, "XXX");
    }

    /// <summary>
    /// Creates a Money instance with zero value for a specific currency.
    /// </summary>
    /// <summary>
    /// Creates a Money instance with zero value for a specific currency.
    /// </summary>
    /// <param name="currency">The currency code (ISO 4217).</param>
    /// <returns>A Money instance with zero value and the specified currency.</returns>
    public static Money Zero(string currency)
    {
        return new Money(0L, currency);
    }

    /// <summary>
    /// Returns the current amount as a 'long' representing its minor unit (e.g., 1050).
    /// Ideal for database storage or sending to payment gateways (like Stripe or PayU).
    /// </summary>
    /// <returns>A 'long' integer.</returns>
    /// <summary>
    /// Returns the current amount as a long representing its minor unit (e.g., 1050).
    /// Ideal for database storage or sending to payment gateways (like Stripe or PayU).
    /// </summary>
    /// <returns>A long integer representing the minor unit amount.</returns>
    public long ToLong()
    {
        return this.Amount;
    }

    /// <summary>
    /// Converts the internally stored minor unit back to a decimal main unit (e.g., from 1050 to 10.50m).
    /// Useful for displaying the value to the user in frontends or reports.
    /// </summary>
    /// <param name="decimalPlaces">The number of decimal places for the currency.</param>
    /// <returns>A decimal representing the main unit.</returns>
    public decimal ToDecimal(short decimalPlaces)
    {
        Guard.IsLessThan(decimalPlaces, (short)0, Exceptions.Layer.None, "002 : Decimal places cannot be negative.");

        var factor = (decimal)Math.Pow(10, decimalPlaces);
        return this.Amount / factor;
    }

    /// <summary>
    /// Subtracts two Money instances. Both must have the same currency.
    /// </summary>
    /// <summary>
    /// Subtracts two Money instances. Both must have the same currency.
    /// </summary>
    /// <param name="a">The first Money instance.</param>
    /// <param name="b">The second Money instance.</param>
    /// <returns>A new Money instance representing the result.</returns>
    public static Money operator -(Money a, Money b)
    {
        Guard.IsTrue(a.Currency != b.Currency, Exceptions.Layer.None, "003 : Cannot subtract amounts with different currencies.");
        return new Money(a.Amount - b.Amount, a.Currency);
    }

    /// <summary>
    /// Adds two Money instances. Both must have the same currency.
    /// </summary>
    /// <summary>
    /// Adds two Money instances. Both must have the same currency.
    /// </summary>
    /// <param name="a">The first Money instance.</param>
    /// <param name="b">The second Money instance.</param>
    /// <returns>A new Money instance representing the sum.</returns>
    public static Money operator +(Money a, Money b)
    {
        Guard.IsTrue(a.Currency != b.Currency, Exceptions.Layer.None, "003 : Cannot add amounts with different currencies.");
        return new Money(a.Amount + b.Amount, a.Currency);
    }

    /// <summary>
    /// Multiplies a Money instance by a decimal factor (e.g., for taxes).
    /// The result is safely rounded back to a long minor unit.
    /// </summary>
    /// <summary>
    /// Multiplies a Money instance by a decimal factor (e.g., for taxes).
    /// The result is safely rounded back to a long minor unit.
    /// </summary>
    /// <param name="money">The Money instance.</param>
    /// <param name="factor">The decimal factor.</param>
    /// <returns>A new Money instance representing the result.</returns>
    public static Money operator *(Money money, decimal factor)
    {
        long newAmount = (long)Math.Round(money.Amount * factor, MidpointRounding.AwayFromZero);
        return new Money(newAmount, money.Currency);
    }

    /// <summary>
    /// Multiplies a Money instance by an integer/long factor (e.g., quantity of items).
    /// </summary>
    /// <summary>
    /// Multiplies a Money instance by an integer/long factor (e.g., quantity of items).
    /// </summary>
    /// <param name="money">The Money instance.</param>
    /// <param name="factor">The integer/long factor.</param>
    /// <returns>A new Money instance representing the result.</returns>
    public static Money operator *(Money money, long factor)
    {
        return new Money(money.Amount * factor, money.Currency);
    }

    /// <summary>
    /// Multiplies a Money instance by a decimal factor (commutative).
    /// </summary>
    /// <param name="factor">The decimal factor.</param>
    /// <param name="money">The Money instance.</param>
    /// <returns>A new Money instance representing the result.</returns>
    public static Money operator *(decimal factor, Money money) => money * factor;

    /// <summary>
    /// Multiplies a Money instance by a long factor (commutative).
    /// </summary>
    /// <param name="factor">The long factor.</param>
    /// <param name="money">The Money instance.</param>
    /// <returns>A new Money instance representing the result.</returns>
    public static Money operator *(long factor, Money money) => money * factor;


    /// <summary>
    /// Determines whether two Money instances are equal.
    /// </summary>
    /// <param name="a">The first Money instance.</param>
    /// <param name="b">The second Money instance.</param>
    /// <returns>True if both are equal; otherwise, false.</returns>
    public static bool operator ==(Money? a, Money? b)
    {
        if (ReferenceEquals(a, b)) 
            return true;

        if (a is null || b is null) 
            return false;

        return a.Equals(b);
    }

    /// <summary>
    /// Determines whether two Money instances are not equal.
    /// </summary>
    /// <param name="a">The first Money instance.</param>
    /// <param name="b">The second Money instance.</param>
    /// <returns>True if both are not equal; otherwise, false.</returns>
    public static bool operator !=(Money? a, Money? b) => !(a == b);

    /// <summary>
    /// Determines whether the specified Money instance is equal to the current instance.
    /// </summary>
    /// <param name="other">The Money instance to compare.</param>
    /// <returns>True if equal; otherwise, false.</returns>
    public bool Equals(Money? other)
    {
        if (other is null) 
            return false;

        return this.Amount == other.Amount && this.Currency == other.Currency;
    }

    /// <summary>
    /// Determines whether the specified object is equal to the current Money instance.
    /// </summary>
    /// <param name="obj">The object to compare.</param>
    /// <returns>True if equal; otherwise, false.</returns>
    public override bool Equals(object? obj) => obj is Money other && Equals(other);

    /// <summary>
    /// Returns the hash code for the current Money instance.
    /// </summary>
    /// <returns>The hash code.</returns>
    public override int GetHashCode() => HashCode.Combine(Amount, Currency);

    /// <summary>
    /// Returns the Money instance with the smaller amount. Both must have the same currency.
    /// </summary>
    /// <param name="a">The first Money instance.</param>
    /// <param name="b">The second Money instance.</param>
    /// <returns>The Money instance with the smaller amount.</returns>
    public static Money Min(Money a, Money b)
    {
        Guard.IsTrue(a.Currency != b.Currency, Exceptions.Layer.None, "004 : Cannot compare amounts with different currencies.");
        return a.Amount < b.Amount ? a : b;
    }

    /// <summary>
    /// Returns the Money instance with the greater amount. Both must have the same currency.
    /// </summary>
    /// <param name="a">The first Money instance.</param>
    /// <param name="b">The second Money instance.</param>
    /// <returns>The Money instance with the greater amount.</returns>
    public static Money Max(Money a, Money b)
    {
        Guard.IsTrue(a.Currency != b.Currency, Exceptions.Layer.None, "004 : Cannot compare amounts with different currencies.");
        return a.Amount > b.Amount ? a : b;
    }
}