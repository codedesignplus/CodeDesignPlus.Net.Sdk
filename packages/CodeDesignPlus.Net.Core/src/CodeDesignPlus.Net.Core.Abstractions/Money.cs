namespace CodeDesignPlus.Net.Core.Abstractions;

/// <summary>
/// Represents a monetary amount with its currency.
/// Uses 'decimal' internally to maintain maximum precision in calculations,
/// following best practices for financial software. Currency properties,
/// such as code and number of decimals, are based on the ISO 4217 international standard.
/// </summary>
public sealed class Money : IEquatable<Money>
{
    /// <summary>
    /// The amount of money in the main unit (e.g., 123.45 instead of 12345).
    /// Can contain fractional values for precise intermediate calculations.
    /// </summary>
    public decimal Amount { get; private set; }

    /// <summary>
    /// The 3-letter currency code, according to ISO 4217 (e.g., "COP", "USD").
    /// </summary>
    public string Currency { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Money"/> class.
    /// </summary>
    /// <param name="amount">The amount of money.</param>
    /// <param name="currency">The currency code.</param>
    public Money(decimal amount, string currency)
    {
        var normalizedCurrency = currency?.Trim().ToUpperInvariant() ?? string.Empty;
        
        ArgumentNullException.ThrowIfNull(currency, nameof(currency));

        Amount = amount;
        Currency = normalizedCurrency;
    }

    /// <summary>
    /// Creates a Money instance from a 'decimal' value representing the main unit (e.g., 10.50 USD).
    /// </summary>
    /// <param name="amount">The amount of money.</param>
    /// <param name="currency">The currency code.</param>
    /// <returns>A new Money instance.</returns>
    public static Money FromDecimal(decimal amount, string currency)
    {
        return new Money(amount, currency);
    }

    /// <summary>
    /// Creates a Money instance from a 'long' representing the minor unit (e.g., 1050 cents).
    /// Requires the decimal places defined by the currency's ISO 4217 standard.
    /// </summary>
    /// <param name="amountInMinorUnit">The amount in the minor unit.</param>
    /// <param name="currency">The currency code.</param>
    /// <param name="decimalPlaces">The number of decimal places for the currency.</param>
    /// <returns>A new Money instance.</returns>
    public static Money FromLong(long amountInMinorUnit, string currency, short decimalPlaces)
    {
        if(decimalPlaces < 0)
            throw new ArgumentOutOfRangeException(nameof(decimalPlaces), "Decimal places cannot be negative.");
        
        var factor = (decimal)Math.Pow(10, decimalPlaces);
        var amount = amountInMinorUnit / factor;

        return new Money(amount, currency);
    }

    /// <summary>
    /// Creates a Money instance with zero value for an unspecified currency.
    /// Uses "XXX" which is the ISO 4217 standard code for "No currency".
    /// </summary>
    public static Money Zero()
    {
        return new Money(0m, "XXX");
    }

    /// <summary>
    /// Creates a Money instance with zero value for a specific currency.
    /// </summary>
    public static Money Zero(string currency)
    {
        return new Money(0m, currency);
    }

    /// <summary>
    /// Converts and rounds the current amount to a 'long' representing its minor unit (e.g., from 10.50 to 1050).
    /// Ideal for database storage or sending to payment gateways (like Stripe or Dian).
    /// Uses MidpointRounding.AwayFromZero for standard financial rounding.
    /// </summary>
    /// <param name="decimalPlaces">The number of decimal places for the currency.</param>
    /// <returns>A 'long' integer.</returns>
    public long ToLong(short decimalPlaces)
    {
        if(decimalPlaces < 0)
            throw new ArgumentOutOfRangeException(nameof(decimalPlaces), "Decimal places cannot be negative.");

        var factor = (decimal)Math.Pow(10, decimalPlaces);
        var roundedAmount = Math.Round(Amount * factor, MidpointRounding.AwayFromZero);

        return (long)roundedAmount;
    }

    /// <summary>
    /// Returns the amount as a 'decimal' in its main unit (e.g., 10.50).
    /// Note: This simply returns the Amount property, but is added for API symmetry with ToLong().
    /// </summary>
    public decimal ToDecimal()
    {
        return this.Amount;
    }

    /// <summary>
    /// Subtracts two Money instances. Both must have the same currency.
    /// </summary>
    /// <param name="a">The first Money instance.</param>
    /// <param name="b">The second Money instance.</param>
    /// <returns>A new Money instance representing the difference.</returns>
    public static Money operator -(Money a, Money b)
    {
        if (a.Currency != b.Currency)
            throw new InvalidOperationException("Cannot subtract amounts with different currencies.");

        return new Money(a.Amount - b.Amount, a.Currency);
    }

    /// <summary>
    /// Adds two Money instances. Both must have the same currency.
    /// </summary>
    /// <param name="a">The first Money instance.</param>
    /// <param name="b">The second Money instance.</param>
    public static Money operator +(Money a, Money b)
    {
        if (a.Currency != b.Currency)
            throw new InvalidOperationException("Cannot add amounts with different currencies.");

        return new Money(a.Amount + b.Amount, a.Currency);
    }

    /// <summary>
    /// Multiplies a Money instance by a decimal factor.
    /// </summary>
    /// <param name="money">The Money instance.</param>
    /// <param name="factor">The decimal factor.</param>
    /// <returns>A new Money instance with the multiplied amount.</returns>
    public static Money operator *(Money money, decimal factor)
    {
        return new Money(money.Amount * factor, money.Currency);
    }

    /// <summary>
    /// Multiplies a Money instance by a long factor.
    /// </summary>
    /// <param name="money">The Money instance.</param>
    /// <param name="factor">The long factor.</param>
    /// <returns>A new Money instance with the multiplied amount.</returns>
    public static Money operator *(Money money, long factor)
    {
        return new Money(money.Amount * factor, money.Currency);
    }

    /// <summary>
    /// Multiplies a decimal factor by a Money instance.
    /// </summary>
    /// <param name="factor">The decimal factor.</param>
    /// <param name="money">The Money instance.</param>
    /// <returns>A new Money instance with the multiplied amount.</returns>
    public static Money operator *(decimal factor, Money money) => money * factor;

    /// <summary>
    /// Multiplies a long factor by a Money instance.
    /// </summary>
    /// <param name="factor">The long factor.</param>
    /// <param name="money">The Money instance.</param>
    /// <returns>A new Money instance with the multiplied amount.</returns>
    public static Money operator *(long factor, Money money) => money * factor;

    /// <summary>
    /// Checks if two Money instances are equal in value and currency.
    /// </summary>
    /// <param name="a">The first Money instance.</param>
    /// <param name="b">The second Money instance.</param>
    /// <returns>True if the instances are equal; otherwise, false.</returns>
    public static bool operator ==(Money? a, Money? b)
    {
        if (ReferenceEquals(a, b)) 
            return true;

        if (a is null || b is null) 
            return false;

        return a.Equals(b);
    }

    /// <summary>
    /// Checks if two Money instances are not equal in value or currency.
    /// </summary>
    /// <param name="a">The first Money instance.</param>
    /// <param name="b">The second Money instance.</param>
    /// <returns>True if the instances are not equal; otherwise, false.</returns>
    public static bool operator !=(Money? a, Money? b) => !(a == b);

    /// <summary>
    /// Indicates whether the current Money instance is equal to another.
    /// </summary>
    /// <param name="other">The Money instance to compare with the current instance.</param>
    /// <returns>True if the instances are equal; otherwise, false.</returns>
    public bool Equals(Money? other)
    {
        if (other is null) 
            return false;

        return this.Amount == other.Amount && this.Currency == other.Currency;
    }

    /// <summary>
    /// Indicates whether the current Money instance is equal to another object.
    /// </summary>
    /// <param name="obj">The object to compare with the current Money instance.</param>
    /// <returns>True if the object is a Money instance and is equal to the current instance; otherwise, false.</returns>
    public override bool Equals(object? obj) => obj is Money other && Equals(other);

    /// <summary>
    /// Returns the hash code for this Money instance.
    /// </summary>
    public override int GetHashCode() => HashCode.Combine(Amount, Currency);

    /// <summary>
    /// Static method to get the minimum value between two Money amounts.
    /// </summary>
    /// <param name="a">The first Money instance.</param>
    /// <param name="b">The second Money instance.</param>
    /// <returns>The Money instance with the smaller amount.</returns>
    public static Money Min(Money a, Money b)
    {
        if (a.Currency != b.Currency)
            throw new InvalidOperationException("Cannot compare amounts with different currencies.");
        
        return a.Amount < b.Amount ? a : b;
    }

    /// <summary>
    /// Static method to get the maximum value between two Money amounts.
    /// </summary>
    /// <param name="a">The first Money instance.</param>
    /// <param name="b">The second Money instance.</param>
    /// <returns>The Money instance with the larger amount.</returns>
    public static Money Max(Money a, Money b)
    {
        if (a.Currency != b.Currency)
            throw new InvalidOperationException("Cannot compare amounts with different currencies.");
        
        return a.Amount > b.Amount ? a : b;
    }
}