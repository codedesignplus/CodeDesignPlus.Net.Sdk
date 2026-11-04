using System.Text.RegularExpressions;
using CodeDesignPlus.Net.Exceptions.Guards;

namespace CodeDesignPlus.Net.ValueObjects.User;

/// <summary>
/// Represents contact information (phone and email) for a person or organization.
/// Used across residents, organizations, emergency contacts, and similar entities.
/// Both email addresses are normalized to lowercase.
/// </summary>
/// <remarks>
/// The constructor only hydrates: Mongo, the cache and the deserialization of events and requests use it, and a read never
/// validates. The factory methods validate with <see cref="EnsureValid"/>.
/// </remarks>
public sealed partial class ContactInfo : IEquatable<ContactInfo>, IValidatableValueObject
{
    [GeneratedRegex(@"^\+?\d{7,15}$", RegexOptions.Compiled)]
    private static partial Regex PhoneRegex();

    [GeneratedRegex(@"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$", RegexOptions.Compiled)]
    private static partial Regex EmailRegex();

    /// <summary>
    /// Gets the primary phone number (format: +?\d{7,15}).
    /// </summary>
    public string Phone { get; private set; }

    /// <summary>
    /// Gets the primary email address (normalized to lowercase).
    /// </summary>
    public string Email { get; private set; }

    /// <summary>
    /// Gets the alternate phone number, if provided.
    /// </summary>
    public string? AlternatePhone { get; private set; }

    /// <summary>
    /// Gets the alternate email address, if provided (normalized to lowercase).
    /// </summary>
    public string? AlternateEmail { get; private set; }

    [JsonConstructor]
    private ContactInfo(string phone, string email, string? alternatePhone, string? alternateEmail)
    {
        Phone = phone?.Trim() ?? string.Empty;
        Email = email?.Trim().ToLowerInvariant() ?? string.Empty;
        AlternatePhone = alternatePhone?.Trim();
        AlternateEmail = alternateEmail?.Trim().ToLowerInvariant();
    }

    /// <summary>
    /// Checks the primary phone and email, and the alternate ones when they are present.
    /// </summary>
    /// <exception cref="Exceptions.CodeDesignPlusException">Thrown with the error of the first invariant that does not hold.</exception>
    public void EnsureValid()
    {
        Guard.IsNullOrEmpty(Phone, Exceptions.Layer.None, Errors.PhoneCannotBeNullOrEmpty2);
        Guard.IsFalse(PhoneRegex().IsMatch(Phone), Exceptions.Layer.None, Errors.PhoneFormatIsInvalid);

        Guard.IsNullOrEmpty(Email, Exceptions.Layer.None, Errors.EmailCannotBeNullOrEmpty2);
        Guard.IsFalse(EmailRegex().IsMatch(Email), Exceptions.Layer.None, Errors.EmailFormatIsInvalid);

        if (AlternatePhone is not null)
        {
            Guard.IsNullOrEmpty(AlternatePhone, Exceptions.Layer.None, Errors.AlternatePhoneCannotBeEmptyWhenProvided);
            Guard.IsFalse(PhoneRegex().IsMatch(AlternatePhone), Exceptions.Layer.None, Errors.AlternatePhoneFormatIsInvalid);
        }

        if (AlternateEmail is not null)
        {
            Guard.IsNullOrEmpty(AlternateEmail, Exceptions.Layer.None, Errors.AlternateEmailCannotBeEmptyWhenProvided);
            Guard.IsFalse(EmailRegex().IsMatch(AlternateEmail), Exceptions.Layer.None, Errors.AlternateEmailFormatIsInvalid);
        }
    }

    /// <summary>
    /// Creates a <see cref="ContactInfo"/> with only primary phone and email.
    /// </summary>
    /// <param name="phone">The primary phone number (format: +?\d{7,15}).</param>
    /// <param name="email">The primary email address.</param>
    /// <returns>A new <see cref="ContactInfo"/> instance.</returns>
    public static ContactInfo Create(string phone, string email)
    {
        var contactInfo = new ContactInfo(phone, email, null, null);

        contactInfo.EnsureValid();

        return contactInfo;
    }

    /// <summary>
    /// Creates a <see cref="ContactInfo"/> with primary and optional alternate contact details.
    /// </summary>
    /// <param name="phone">The primary phone number.</param>
    /// <param name="email">The primary email address.</param>
    /// <param name="alternatePhone">An optional alternate phone number.</param>
    /// <param name="alternateEmail">An optional alternate email address.</param>
    /// <returns>A new <see cref="ContactInfo"/> instance.</returns>
    public static ContactInfo Create(string phone, string email, string? alternatePhone, string? alternateEmail)
    {
        var contactInfo = new ContactInfo(phone, email, alternatePhone, alternateEmail);

        contactInfo.EnsureValid();

        return contactInfo;
    }

    /// <summary>
    /// Returns true if two <see cref="ContactInfo"/> instances are equal.
    /// </summary>
    /// <param name="a">The first instance.</param>
    /// <param name="b">The second instance.</param>
    /// <returns>True if the instances are equal; otherwise, false.</returns>
    public static bool operator ==(ContactInfo? a, ContactInfo? b)
    {
        if (ReferenceEquals(a, b)) return true;
        if (a is null || b is null) return false;
        return a.Equals(b);
    }

    /// <summary>
    /// Returns true if two <see cref="ContactInfo"/> instances are not equal.
    /// </summary>
    /// <param name="a">The first instance.</param>
    /// <param name="b">The second instance.</param>
    /// <returns>True if the instances are not equal; otherwise, false.</returns>
    public static bool operator !=(ContactInfo? a, ContactInfo? b) => !(a == b);

    /// <summary>
    /// Returns true if this instance is equal to another <see cref="ContactInfo"/>.
    /// </summary>
    /// <param name="other">The other instance to compare to.</param>
    /// <returns>True if the instances are equal; otherwise, false.</returns>
    public bool Equals(ContactInfo? other)
    {
        if (other is null) return false;
        return Phone == other.Phone &&
               Email == other.Email &&
               AlternatePhone == other.AlternatePhone &&
               AlternateEmail == other.AlternateEmail;
    }

    /// <summary>
    /// Returns true if this instance is equal to another object.
    /// </summary>
    /// <param name="obj">The object to compare to.</param>
    /// <returns>True if the objects are equal; otherwise, false.</returns>
    public override bool Equals(object? obj) => obj is ContactInfo other && Equals(other);

    /// <summary>
    /// Returns the hash code for this instance.
    /// </summary>
    /// <returns>The hash code.</returns>
    public override int GetHashCode() => HashCode.Combine(Phone, Email, AlternatePhone, AlternateEmail);
}
