using CodeDesignPlus.Net.Exceptions.Guards;

namespace CodeDesignPlus.Net.ValueObjects.Payment;

/// <summary>
/// Represents the selected payment method. 
/// Acts as a discriminated union ensuring only one payment type (e.g., PSE XOR CreditCard) is active at a time.
/// </summary>
/// <remarks>
/// The constructor only hydrates: Mongo, the cache and the deserialization of events and requests use it, and a read never
/// validates. The factory methods validate with <see cref="EnsureValid"/>.
/// </remarks>
public sealed class PaymentMethod : IEquatable<PaymentMethod>, IValidatableValueObject
{
    /// <summary>
    /// The type representing the payment method.
    /// </summary>
    public string Type { get; private set; }
    /// <summary>
    /// The PSE payment details, if applicable.
    /// </summary>
    public Pse? Pse { get; private set; }
    /// <summary>
    /// The credit card payment details, if applicable.
    /// </summary>
    public CreditCard? CreditCard { get; private set; }

    [JsonConstructor]
    private PaymentMethod(string type, Pse? pse, CreditCard? creditCard)
    {
        Type = type?.Trim().ToUpperInvariant() ?? string.Empty;
        Pse = pse;
        CreditCard = creditCard;
    }

    /// <summary>
    /// Checks the type, that exactly one of the payment details is present, and the details themselves.
    /// </summary>
    /// <exception cref="Exceptions.CodeDesignPlusException">Thrown with the error of the first invariant that does not hold.</exception>
    public void EnsureValid()
    {
        Guard.IsNullOrEmpty(Type, Exceptions.Layer.None, Errors.TypeOfThePaymentMethodCannotBeNull);

        bool bothAreNull = Pse == null && CreditCard == null;
        Guard.IsTrue(bothAreNull, Exceptions.Layer.None, Errors.PaymentMethodDetailsCannotBeNull);

        bool bothAreProvided = Pse != null && CreditCard != null;
        Guard.IsTrue(bothAreProvided, Exceptions.Layer.None, Errors.OnlyOnePaymentMethodIsAllowed);

        Pse?.EnsureValid();
        CreditCard?.EnsureValid();
    }

    /// <summary>
    /// Creates a new instance of the PaymentMethod class.
    /// </summary>
    /// <param name="type">The type representing the payment method.</param>
    /// <param name="pse">The PSE payment details, if applicable.</param>
    /// <param name="creditCard">The credit card payment details, if applicable.</param>
    /// <returns>A new instance of the PaymentMethod class.</returns>
    public static PaymentMethod Create(string type, Pse? pse, CreditCard? creditCard)
    {
        var paymentMethod = new PaymentMethod(type, pse, creditCard);

        paymentMethod.EnsureValid();

        return paymentMethod;
    }

    /// <summary>
    /// Determines whether two payment method instances are equal.
    /// </summary>
    /// <param name="a">The first payment method instance.</param>
    /// <param name="b">The second payment method instance.</param>
    /// <returns>True if the instances are equal; otherwise, false.</returns>
    public static bool operator ==(PaymentMethod? a, PaymentMethod? b)
    {
        if (ReferenceEquals(a, b)) return true;
        if (a is null || b is null) return false;
        return a.Equals(b);
    }

    /// <summary>
    /// Determines whether two payment method instances are not equal.
    /// </summary>
    /// <param name="a">The first payment method instance.</param>
    /// <param name="b">The second payment method instance.</param>
    /// <returns>True if the instances are not equal; otherwise, false.</returns>
    public static bool operator !=(PaymentMethod? a, PaymentMethod? b) => !(a == b);

    /// <summary>
    /// Determines whether the current payment method instance is equal to another payment method instance.
    /// </summary>
    /// <param name="other">The payment method instance to compare with the current instance.</param>
    /// <returns>True if the instances are equal; otherwise, false.</returns>
    public bool Equals(PaymentMethod? other)
    {
        if (other is null) 
            return false;
        
        return Type == other.Type 
        && EqualityComparer<Pse?>.Default.Equals(Pse, other.Pse) 
        && EqualityComparer<CreditCard?>.Default.Equals(CreditCard, other.CreditCard);
    }

    /// <summary>
    /// Determines whether the current payment method instance is equal to another object.
    /// </summary>
    /// <param name="obj">The object to compare with the current instance.</param>
    /// <returns>True if the object is a payment method instance and is equal to the current instance; otherwise, false.</returns>
    public override bool Equals(object? obj) => obj is PaymentMethod other && Equals(other);

    /// <summary>
    /// Returns a hash code for the current payment method instance.
    /// </summary>
    /// <returns>A hash code for the current payment method instance.</returns>
    public override int GetHashCode() => HashCode.Combine(Type, Pse, CreditCard);
}