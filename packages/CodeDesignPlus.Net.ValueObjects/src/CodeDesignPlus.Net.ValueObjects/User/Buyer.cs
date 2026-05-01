using System.Text.RegularExpressions;
using CodeDesignPlus.Net.Exceptions.Guards;

namespace CodeDesignPlus.Net.ValueObjects.User;

/// <summary>
/// Represents the snapshot of a buyer's information at the exact moment a payment is initiated.
/// This immutable Value Object guarantees that contact and identification details remain valid and unchanged.
/// The buyer is the person who makes the purchase and receives order notifications.
/// </summary>
/// <remarks>
/// <para><strong>Usage Examples:</strong></para>
///
/// <para><strong>Example 1: Physical Product with Shipping (E-commerce)</strong></para>
/// <code>
/// var typeDoc = TypeDocument.Create("CC", "Cédula de Ciudadanía");
/// var shippingAddr = Address.Create(
///     street: "Carrera 15 #89-20",
///     country: "CO",
///     state: "Cundinamarca",
///     city: "Bogotá",
///     postalCode: "110221"
/// );
///
/// var buyer = Buyer.Create(
///     buyerId: userId,
///     name: "Juan Pérez",
///     phone: "+573001234567",
///     email: "juan@email.com",
///     typeDocument: typeDoc,
///     document: "1234567890",
///     shippingAddress: shippingAddr
/// );
/// // Complete buyer for physical product delivery
/// </code>
///
/// <para><strong>Example 2: Digital Product/Service (No Shipping)</strong></para>
/// <code>
/// var typeDoc = TypeDocument.Create("CC", "Cédula de Ciudadanía");
/// var buyer = Buyer.CreateWithoutShipping(
///     buyerId: userId,
///     name: "María López",
///     phone: "+573101234567",
///     email: "maria@email.com",
///     typeDocument: typeDoc,
///     document: "9876543210"
/// );
/// // buyer.ShippingAddress = null (digital product - no delivery needed)
/// </code>
///
/// <para><strong>Example 3: International Digital Service (Minimal)</strong></para>
/// <code>
/// var buyer = Buyer.CreateMinimal(
///     buyerId: userId,
///     name: "John Doe",
///     phone: "+12125551234",
///     email: "john@email.com"
/// );
/// // buyer.TypeDocument = null
/// // buyer.Document = null
/// // buyer.ShippingAddress = null
/// // For SaaS subscriptions, streaming services, etc.
/// </code>
/// </remarks>
public sealed partial class Buyer : IEquatable<Buyer>
{
    [GeneratedRegex(@"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$", RegexOptions.Compiled)]
    private static partial Regex EmailRegex();

    [GeneratedRegex(@"^\+?\d{7,15}$", RegexOptions.Compiled)]
    private static partial Regex PhoneRegex();

    /// <summary>
    /// Gets the unique identifier of the user/buyer in the core system.
    /// </summary>
    public Guid BuyerId { get; private set; }
    /// <summary>
    /// Gets the name of the buyer.
    /// </summary>
    public string Name { get; private set; }
    /// <summary>
    /// Gets the phone number of the buyer.
    /// </summary>
    public string Phone { get; private set; }
    /// <summary>
    /// Gets the email address of the buyer.
    /// </summary>
    public string Email { get; private set; }
    /// <summary>
    /// Gets the type of document of the buyer (optional, required for LATAM markets).
    /// </summary>
    public TypeDocument? TypeDocument { get; private set; }
    /// <summary>
    /// Gets the document number of the buyer (optional, required for LATAM markets).
    /// </summary>
    public string? Document { get; private set; }
    /// <summary>
    /// Gets the shipping address where physical products will be delivered (optional, not needed for digital products/services).
    /// </summary>
    public Address? ShippingAddress { get; private set; }

    [JsonConstructor]
    private Buyer(Guid buyerId, string name, string phone, string email, TypeDocument? typeDocument, string? document, Address? shippingAddress)
    {
        Guard.GuidIsEmpty(buyerId, Exceptions.Layer.None, "000 : BuyerId cannot be empty");

        var normalizedName = name?.Trim() ?? string.Empty;
        Guard.IsNullOrEmpty(normalizedName, Exceptions.Layer.None, "001 : Name cannot be null or empty");
        Guard.IsGreaterThan(normalizedName.Length, 124, Exceptions.Layer.None, "002 : Name cannot be greater than 124 characters");

        var normalizedPhone = phone?.Trim() ?? string.Empty;
        Guard.IsNullOrEmpty(normalizedPhone, Exceptions.Layer.None, "003 : Phone cannot be null or empty");
        Guard.IsFalse(PhoneRegex().IsMatch(normalizedPhone), Exceptions.Layer.None, "004 : Phone contains invalid characters");

        var normalizedEmail = email?.Trim().ToLowerInvariant() ?? string.Empty;
        Guard.IsNullOrEmpty(normalizedEmail, Exceptions.Layer.None, "005 : Email cannot be null or empty");
        Guard.IsFalse(EmailRegex().IsMatch(normalizedEmail), Exceptions.Layer.None, "006 : Email contains invalid characters");

        // Optional fields validation - only validate if provided
        if (document != null)
        {
            var normalizedDocument = document.Trim();
            Guard.IsNullOrEmpty(normalizedDocument, Exceptions.Layer.None, "008 : Document cannot be empty when provided");
            Guard.IsGreaterThan(normalizedDocument.Length, 20, Exceptions.Layer.None, "009 : Document cannot be greater than 20 characters");
            this.Document = normalizedDocument;
        }

        this.BuyerId = buyerId;
        this.Name = normalizedName;
        this.Phone = normalizedPhone;
        this.Email = normalizedEmail;
        this.TypeDocument = typeDocument;
        this.ShippingAddress = shippingAddress;
    }

    /// <summary>
    /// Creates a new immutable snapshot of the buyer's details with all fields including shipping address.
    /// Use this for physical product purchases requiring full identification and delivery (LATAM compliance).
    /// </summary>
    /// <param name="buyerId">The unique identifier of the buyer.</param>
    /// <param name="name">The name of the buyer.</param>
    /// <param name="phone">The phone number of the buyer.</param>
    /// <param name="email">The email address of the buyer.</param>
    /// <param name="typeDocument">The type of document of the buyer.</param>
    /// <param name="document">The document number of the buyer.</param>
    /// <param name="shippingAddress">The shipping address for physical product delivery.</param>
    /// <returns>A new instance of the <see cref="Buyer"/> class.</returns>
    public static Buyer Create(Guid buyerId, string name, string phone, string email, TypeDocument typeDocument, string document, Address shippingAddress)
    {
        return new Buyer(buyerId, name, phone, email, typeDocument, document, shippingAddress);
    }

    /// <summary>
    /// Creates a buyer snapshot with identification but without shipping address.
    /// Use this for digital products/services that don't require physical delivery.
    /// </summary>
    /// <param name="buyerId">The unique identifier of the buyer.</param>
    /// <param name="name">The name of the buyer.</param>
    /// <param name="phone">The phone number of the buyer.</param>
    /// <param name="email">The email address of the buyer.</param>
    /// <param name="typeDocument">The type of document of the buyer.</param>
    /// <param name="document">The document number of the buyer.</param>
    /// <returns>A new instance of the <see cref="Buyer"/> class.</returns>
    public static Buyer CreateWithoutShipping(Guid buyerId, string name, string phone, string email, TypeDocument typeDocument, string document)
    {
        return new Buyer(buyerId, name, phone, email, typeDocument, document, null);
    }

    /// <summary>
    /// Creates a minimal buyer snapshot without document identification or shipping address.
    /// Use this for digital products/services in international markets (Stripe, PayPal).
    /// TypeDocument, Document, and ShippingAddress will be null.
    /// </summary>
    /// <param name="buyerId">The unique identifier of the buyer.</param>
    /// <param name="name">The name of the buyer.</param>
    /// <param name="phone">The phone number of the buyer.</param>
    /// <param name="email">The email address of the buyer.</param>
    /// <returns>A new instance of the <see cref="Buyer"/> class.</returns>
    public static Buyer CreateMinimal(Guid buyerId, string name, string phone, string email)
    {
        return new Buyer(buyerId, name, phone, email, null, null, null);
    }

    /// <summary>
    /// Determines whether two <see cref="Buyer"/> instances are equal.
    /// </summary>
    /// <param name="a">The first <see cref="Buyer"/> instance.</param>
    /// <param name="b">The second <see cref="Buyer"/> instance.</param>
    /// <returns><c>true</c> if the instances are equal; otherwise, <c>false</c>.</returns>
    public static bool operator ==(Buyer? a, Buyer? b)
    {
        if (ReferenceEquals(a, b)) 
            return true;

        if (a is null || b is null) 
            return false;

        return a.Equals(b);
    }

    /// <summary>
    /// Determines whether two <see cref="Buyer"/> instances are not equal.
    /// </summary>
    /// <param name="a">The first <see cref="Buyer"/> instance.</param>
    /// <param name="b">The second <see cref="Buyer"/> instance.</param>
    /// <returns><c>true</c> if the instances are not equal; otherwise, <c>false</c>.</returns>
    public static bool operator !=(Buyer? a, Buyer? b) => !(a == b);

    /// <summary>
    /// Determines whether the specified <see cref="Buyer"/> is equal to the current <see cref="Buyer"/>.
    /// </summary>
    /// <param name="other">The <see cref="Buyer"/> to compare with the current <see cref="Buyer"/>.</param>
    /// <returns><c>true</c> if the specified <see cref="Buyer"/> is equal to the current <see cref="Buyer"/>; otherwise, <c>false</c>.</returns>
    public bool Equals(Buyer? other)
    {
        if (other is null) 
            return false;

        return this.BuyerId == other.BuyerId &&
               this.Name == other.Name &&
               this.Phone == other.Phone &&
               this.Email == other.Email &&
               Equals(this.TypeDocument, other.TypeDocument) &&
               this.Document == other.Document &&
               Equals(this.ShippingAddress, other.ShippingAddress);
    }

    /// <summary>
    /// Determines whether the specified object is equal to the current <see cref="Buyer"/>.
    /// </summary>
    /// <param name="obj">The object to compare with the current <see cref="Buyer"/>.</param>
    /// <returns><c>true</c> if the specified object is equal to the current <see cref="Buyer"/>; otherwise, <c>false</c>.</returns>
    public override bool Equals(object? obj) => obj is Buyer other && Equals(other);

    /// <summary>
    /// Serves as the default hash function.
    /// </summary>
    /// <returns>A hash code for the current <see cref="Buyer"/>.</returns>
    public override int GetHashCode()
    {
        return HashCode.Combine(BuyerId, Name, Phone, Email, TypeDocument, Document, ShippingAddress);
    }
}