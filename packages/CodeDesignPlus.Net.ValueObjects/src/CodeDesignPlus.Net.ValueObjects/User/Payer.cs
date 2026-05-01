using System.Text.RegularExpressions;
using CodeDesignPlus.Net.Exceptions.Guards;

namespace CodeDesignPlus.Net.ValueObjects.User;

/// <summary>
/// Represents the snapshot of a payer's information at the exact moment a payment is initiated.
/// This immutable Value Object captures billing details and fiscal identification for payment processing.
/// The payer is the person or entity that provides payment, which may differ from the buyer.
/// </summary>
/// <remarks>
/// <para><strong>Buyer vs Payer:</strong></para>
/// <list type="bullet">
/// <item><description><strong>Buyer:</strong> Person who makes the purchase (receives order notifications)</description></item>
/// <item><description><strong>Payer:</strong> Person/entity who provides payment (appears on invoice/receipt)</description></item>
/// </list>
///
/// <para><strong>Common Scenarios:</strong></para>
/// <list type="number">
/// <item><description>Same person: Parent buys and pays with their own card</description></item>
/// <item><description>Different people: Child buys, parent pays with their card</description></item>
/// <item><description>Corporate: Employee buys, company pays (company is payer)</description></item>
/// </list>
///
/// <para><strong>Usage Examples:</strong></para>
///
/// <para><strong>Example 1: PayU Colombia (Complete billing info)</strong></para>
/// <code>
/// var typeDoc = TypeDocument.Create("CC", "Cédula de Ciudadanía");
/// var address = Address.Create(
///     street: "Calle 123 #45-67",
///     country: "CO",
///     state: "Cundinamarca",
///     city: "Bogotá",
///     postalCode: "110111"
/// );
///
/// var payer = Payer.CreateFromBuyer(
///     fullName: "Juan Pérez García",
///     buyer: buyer,
///     typeDocument: typeDoc,
///     documentNumber: "1234567890",
///     billingAddress: address
/// );
/// // Reuses buyer's email and phone for billing notifications
/// </code>
///
/// <para><strong>Example 2: Stripe International (Minimal)</strong></para>
/// <code>
/// var address = Address.CreateMinimal(
///     street: "123 Main St",
///     country: "US",
///     city: "New York"
/// );
///
/// var typeDoc = TypeDocument.Create("TIN", "Tax Identification Number");
/// var payer = Payer.CreateMinimal(
///     fullName: "John Doe",
///     typeDocument: typeDoc,
///     documentNumber: "123-45-6789",
///     billingAddress: address
/// );
/// // payer.EmailAddress = null (use buyer's email separately)
/// // payer.ContactPhone = null (use buyer's phone separately)
/// </code>
///
/// <para><strong>Example 3: Corporate Purchase</strong></para>
/// <code>
/// var address = Address.Create(
///     street: "Av. Libertador 1234",
///     country: "AR",
///     state: "Buenos Aires",
///     city: "CABA",
///     postalCode: "C1001"
/// );
///
/// var typeDoc = TypeDocument.Create("CUIT", "Clave Única de Identificación Tributaria");
/// var payer = Payer.Create(
///     fullName: "Tech Solutions S.A.",
///     emailAddress: "billing@techsolutions.com",
///     contactPhone: "+5491145678900",
///     typeDocument: typeDoc,
///     documentNumber: "30-12345678-9",
///     billingAddress: address
/// );
/// // Company is the payer, employee (buyer) is different
/// </code>
/// </remarks>
public sealed partial class Payer : IEquatable<Payer>
{
    [GeneratedRegex(@"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$", RegexOptions.Compiled)]
    private static partial Regex EmailRegex();

    [GeneratedRegex(@"^\+?\d{7,15}$", RegexOptions.Compiled)]
    private static partial Regex PhoneRegex();

    /// <summary>
    /// Gets the full legal name of the payer (person or company).
    /// </summary>
    public string FullName { get; private set; }

    /// <summary>
    /// Gets the email address for billing notifications (optional, can use buyer's email).
    /// </summary>
    public string? EmailAddress { get; private set; }

    /// <summary>
    /// Gets the contact phone number (optional, can use buyer's phone).
    /// </summary>
    public string? ContactPhone { get; private set; }

    /// <summary>
    /// Gets the type of identification document.
    /// </summary>
    public TypeDocument TypeDocument { get; private set; }

    /// <summary>
    /// Gets the identification document number (DNI, NIT, Tax ID, etc.).
    /// </summary>
    public string DocumentNumber { get; private set; }

    /// <summary>
    /// Gets the billing address.
    /// </summary>
    public Address BillingAddress { get; private set; }

    [JsonConstructor]
    private Payer(string fullName, string? emailAddress, string? contactPhone, TypeDocument typeDocument, string documentNumber, Address billingAddress)
    {
        var normalizedFullName = fullName?.Trim() ?? string.Empty;
        var normalizedDocument = documentNumber?.Trim() ?? string.Empty;

        Guard.IsNullOrEmpty(normalizedFullName, Exceptions.Layer.None, "000 : Full name cannot be null or empty");
        Guard.IsGreaterThan(normalizedFullName.Length, 150, Exceptions.Layer.None, "001 : Full name cannot be greater than 150 characters");

        Guard.IsNull(typeDocument, Exceptions.Layer.None, "007 : Type document cannot be null");

        Guard.IsNullOrEmpty(normalizedDocument, Exceptions.Layer.None, "008 : Document number cannot be null or empty");
        Guard.IsGreaterThan(normalizedDocument.Length, 20, Exceptions.Layer.None, "009 : Document number cannot be greater than 20 characters");

        Guard.IsNull(billingAddress, Exceptions.Layer.None, "010 : Billing address cannot be null");

        // Optional fields validation - only validate if provided
        if (emailAddress != null)
        {
            var normalizedEmail = emailAddress.Trim().ToLowerInvariant();
            Guard.IsNullOrEmpty(normalizedEmail, Exceptions.Layer.None, "002 : Email address cannot be empty when provided");
            Guard.IsGreaterThan(normalizedEmail.Length, 255, Exceptions.Layer.None, "003 : Email address cannot be greater than 255 characters");
            Guard.IsFalse(EmailRegex().IsMatch(normalizedEmail), Exceptions.Layer.None, "004 : Email address format is invalid");
            this.EmailAddress = normalizedEmail;
        }

        if (contactPhone != null)
        {
            var normalizedPhone = contactPhone.Trim();
            Guard.IsNullOrEmpty(normalizedPhone, Exceptions.Layer.None, "005 : Contact phone cannot be empty when provided");
            Guard.IsFalse(PhoneRegex().IsMatch(normalizedPhone), Exceptions.Layer.None, "006 : Contact phone format is invalid");
            this.ContactPhone = normalizedPhone;
        }

        this.FullName = normalizedFullName;
        this.TypeDocument = typeDocument;
        this.DocumentNumber = normalizedDocument;
        this.BillingAddress = billingAddress;
    }

    /// <summary>
    /// Creates a new immutable snapshot of the payer's details with all fields (LATAM compliance).
    /// Use this for markets requiring complete billing information with fiscal identification.
    /// </summary>
    /// <param name="fullName">The full legal name of the payer.</param>
    /// <param name="emailAddress">The email address for billing notifications.</param>
    /// <param name="contactPhone">The contact phone number.</param>
    /// <param name="typeDocument">The type of identification document.</param>
    /// <param name="documentNumber">The identification document number.</param>
    /// <param name="billingAddress">The billing address.</param>
    /// <returns>A new instance of the <see cref="Payer"/> class.</returns>
    public static Payer Create(string fullName, string emailAddress, string contactPhone, TypeDocument typeDocument, string documentNumber, Address billingAddress)
    {
        return new Payer(fullName, emailAddress, contactPhone, typeDocument, documentNumber, billingAddress);
    }

    /// <summary>
    /// Creates a minimal payer snapshot using buyer's contact information.
    /// Use this when the payer is the same as the buyer and you want to reuse their contact info.
    /// Email and phone are derived from the buyer's data.
    /// </summary>
    /// <param name="fullName">The full legal name of the payer.</param>
    /// <param name="buyer">The buyer instance to extract email and phone from.</param>
    /// <param name="typeDocument">The type of identification document.</param>
    /// <param name="documentNumber">The identification document number.</param>
    /// <param name="billingAddress">The billing address.</param>
    /// <returns>A new instance of the <see cref="Payer"/> class.</returns>
    public static Payer CreateFromBuyer(string fullName, Buyer buyer, TypeDocument typeDocument, string documentNumber, Address billingAddress)
    {
        return new Payer(fullName, buyer.Email, buyer.Phone, typeDocument, documentNumber, billingAddress);
    }

    /// <summary>
    /// Creates a payer snapshot with only essential fiscal information.
    /// Use this when you only need billing identification without separate contact info.
    /// Email and ContactPhone will be null.
    /// </summary>
    /// <param name="fullName">The full legal name of the payer.</param>
    /// <param name="typeDocument">The type of identification document.</param>
    /// <param name="documentNumber">The identification document number.</param>
    /// <param name="billingAddress">The billing address.</param>
    /// <returns>A new instance of the <see cref="Payer"/> class.</returns>
    public static Payer CreateMinimal(string fullName, TypeDocument typeDocument, string documentNumber, Address billingAddress)
    {
        return new Payer(fullName, null, null, typeDocument, documentNumber, billingAddress);
    }

    /// <summary>
    /// Determines whether two Payer instances are equal.
    /// </summary>
    public static bool operator ==(Payer? a, Payer? b)
    {
        if (ReferenceEquals(a, b))
            return true;

        if (a is null || b is null)
            return false;

        return a.Equals(b);
    }

    /// <summary>
    /// Determines whether two Payer instances are not equal.
    /// </summary>
    public static bool operator !=(Payer? a, Payer? b) => !(a == b);

    /// <summary>
    /// Determines whether the specified Payer is equal to the current Payer.
    /// </summary>
    public bool Equals(Payer? other)
    {
        if (other is null)
            return false;

        return this.FullName == other.FullName &&
               this.EmailAddress == other.EmailAddress &&
               this.ContactPhone == other.ContactPhone &&
               Equals(this.TypeDocument, other.TypeDocument) &&
               this.DocumentNumber == other.DocumentNumber &&
               Equals(this.BillingAddress, other.BillingAddress);
    }

    /// <summary>
    /// Determines whether the specified object is equal to the current Payer.
    /// </summary>
    public override bool Equals(object? obj) => obj is Payer other && Equals(other);

    /// <summary>
    /// Returns a hash code for the current Payer.
    /// </summary>
    public override int GetHashCode()
    {
        return HashCode.Combine(FullName, EmailAddress, ContactPhone, TypeDocument, DocumentNumber, BillingAddress);
    }
}
