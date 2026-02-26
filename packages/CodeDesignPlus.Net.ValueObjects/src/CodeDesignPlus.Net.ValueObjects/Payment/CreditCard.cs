using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using CodeDesignPlus.Net.Exceptions.Guards;

namespace CodeDesignPlus.Net.ValueObjects.Payment;

/// <summary>
/// Represents a tokenized, PCI-DSS compliant credit card snapshot.
/// Stores only non-sensitive data safe for database persistence.
/// </summary>
public sealed partial class CreditCard : IEquatable<CreditCard>
{
    // Asumiendo formato YYYY/MM según tu diseño original
    [GeneratedRegex(@"^\d{4}/\d{2}$", RegexOptions.Compiled)]
    private static partial Regex ExpirationDateRegex();

    [GeneratedRegex(@"^\d{4}$", RegexOptions.Compiled)]
    private static partial Regex Last4DigitsRegex();

    /// <summary>
    /// The secure token provided by the payment gateway (e.g., PayU) to process future charges.
    /// </summary>
    public string Token { get; private set; }

    /// <summary>
    /// The last 4 digits of the card, used strictly for display and identification purposes.
    /// </summary>
    public string Last4Digits { get; private set; }

    /// <summary>
    /// The expiration date of the credit card in YYYY/MM format.
    /// </summary>
    public string ExpirationDate { get; private set; } = string.Empty;
    
    /// <summary>
    /// The name of the cardholder.
    /// </summary>
    public string CardHolderName { get; private set; } = string.Empty;

    [JsonConstructor]
    private CreditCard(string token, string last4Digits, string expirationDate, string cardHolderName)
    {
        var normalizedToken = token?.Trim() ?? string.Empty;
        var normalizedLast4 = last4Digits?.Trim() ?? string.Empty;
        var normalizedExpirationDate = expirationDate?.Trim() ?? string.Empty;
        var normalizedCardHolderName = cardHolderName?.Trim().ToUpperInvariant() ?? string.Empty;

        Guard.IsNullOrEmpty(normalizedToken, Exceptions.Layer.None, "000 : Credit Card Token cannot be null or empty");

        Guard.IsNullOrEmpty(normalizedLast4, Exceptions.Layer.None, "001 : Credit Card Last 4 digits cannot be null or empty");
        Guard.IsFalse(Last4DigitsRegex().IsMatch(normalizedLast4), Exceptions.Layer.None, "002 : Credit Card Last 4 digits must be exactly 4 digits");

        Guard.IsNullOrEmpty(normalizedExpirationDate, Exceptions.Layer.None, "003 : Credit Card Expiration Date cannot be null or empty");
        Guard.IsFalse(ExpirationDateRegex().IsMatch(normalizedExpirationDate), Exceptions.Layer.None, "004 : Credit Card Expiration Date must be in valid format");

        Guard.IsNullOrEmpty(normalizedCardHolderName, Exceptions.Layer.None, "005 : Credit Card Holder Name cannot be null or empty");

        this.Token = normalizedToken;
        this.Last4Digits = normalizedLast4;
        this.ExpirationDate = normalizedExpirationDate;
        this.CardHolderName = normalizedCardHolderName;
    }
    
    /// <summary>
    /// Creates a new immutable snapshot of the credit card's details.
    /// </summary>
    /// <param name="token">The secure token provided by the payment gateway.</param>
    /// <param name="last4Digits">The last 4 digits of the credit card.</param>
    /// <param name="expirationDate">The expiration date of the credit card in YYYY/MM format.</param>
    /// <param name="cardHolderName">The name of the cardholder.</param>
    /// <returns>A new immutable snapshot of the credit card's details.</returns>
    public static CreditCard Create(string token, string last4Digits, string expirationDate, string cardHolderName)
    {
        return new CreditCard(token, last4Digits, expirationDate, cardHolderName);
    }

    /// <summary>
    /// Determines whether two credit card instances are equal.
    /// </summary>
    /// <param name="a">The first credit card instance.</param>
    /// <param name="b">The second credit card instance.</param>
    /// <returns>True if the instances are equal; otherwise, false.</returns>
    public static bool operator ==(CreditCard? a, CreditCard? b)
    {
        if (ReferenceEquals(a, b)) return true;
        if (a is null || b is null) return false;
        return a.Equals(b);
    }

    /// <summary>
    /// Determines whether two credit card instances are not equal.
    /// </summary>
    /// <param name="a">The first credit card instance.</param>
    /// <param name="b">The second credit card instance.</param>
    /// <returns>True if the instances are not equal; otherwise, false.</returns>
    public static bool operator !=(CreditCard? a, CreditCard? b) => !(a == b);

    /// <summary>
    /// Determines whether the current credit card instance is equal to another credit card instance.
    /// </summary>
    /// <param name="other">The credit card instance to compare with the current instance.</param>
    /// <returns>True if the instances are equal; otherwise, false.</returns>
    public bool Equals(CreditCard? other)
    {
        if (other is null) return false;
        
        return Token == other.Token && 
               Last4Digits == other.Last4Digits && 
               ExpirationDate == other.ExpirationDate && 
               CardHolderName == other.CardHolderName;
    }

    /// <summary>
    /// Determines whether the current credit card instance is equal to another object.
    /// </summary>
    /// <param name="obj">The object to compare with the current instance.</param>
    /// <returns>True if the object is a credit card instance and is equal to the current instance; otherwise, false.</returns>
    public override bool Equals(object? obj) => obj is CreditCard other && Equals(other);
    
    /// <summary>
    /// Returns a hash code for the current credit card instance.
    /// </summary>
    /// <returns>A hash code for the current credit card instance.</returns>
    public override int GetHashCode() => HashCode.Combine(Token, Last4Digits, ExpirationDate, CardHolderName);
}