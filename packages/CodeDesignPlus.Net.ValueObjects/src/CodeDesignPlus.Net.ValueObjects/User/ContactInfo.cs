using System.Text.RegularExpressions;
using CodeDesignPlus.Net.Exceptions.Guards;

namespace CodeDesignPlus.Net.ValueObjects.User;

/// <summary>
/// Represents contact information (phone and email) for a person or organization.
/// Used across residents, organizations, emergency contacts, and similar entities.
/// Both email addresses are normalized to lowercase.
/// </summary>
public sealed partial class ContactInfo : IEquatable<ContactInfo>
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
        var normalizedPhone = phone?.Trim() ?? string.Empty;
        Guard.IsNullOrEmpty(normalizedPhone, Exceptions.Layer.None, "000 : Phone cannot be null or empty.");
        Guard.IsFalse(PhoneRegex().IsMatch(normalizedPhone), Exceptions.Layer.None, "001 : Phone format is invalid.");

        var normalizedEmail = email?.Trim().ToLowerInvariant() ?? string.Empty;
        Guard.IsNullOrEmpty(normalizedEmail, Exceptions.Layer.None, "002 : Email cannot be null or empty.");
        Guard.IsFalse(EmailRegex().IsMatch(normalizedEmail), Exceptions.Layer.None, "003 : Email format is invalid.");

        if (alternatePhone is not null)
        {
            var normalizedAltPhone = alternatePhone.Trim();
            Guard.IsNullOrEmpty(normalizedAltPhone, Exceptions.Layer.None, "004 : AlternatePhone cannot be empty when provided.");
            Guard.IsFalse(PhoneRegex().IsMatch(normalizedAltPhone), Exceptions.Layer.None, "005 : AlternatePhone format is invalid.");
            AlternatePhone = normalizedAltPhone;
        }

        if (alternateEmail is not null)
        {
            var normalizedAltEmail = alternateEmail.Trim().ToLowerInvariant();
            Guard.IsNullOrEmpty(normalizedAltEmail, Exceptions.Layer.None, "006 : AlternateEmail cannot be empty when provided.");
            Guard.IsFalse(EmailRegex().IsMatch(normalizedAltEmail), Exceptions.Layer.None, "007 : AlternateEmail format is invalid.");
            AlternateEmail = normalizedAltEmail;
        }

        Phone = normalizedPhone;
        Email = normalizedEmail;
    }

    /// <summary>
    /// Creates a <see cref="ContactInfo"/> with only primary phone and email.
    /// </summary>
    /// <param name="phone">The primary phone number (format: +?\d{7,15}).</param>
    /// <param name="email">The primary email address.</param>
    /// <returns>A new <see cref="ContactInfo"/> instance.</returns>
    public static ContactInfo Create(string phone, string email)
        => new(phone, email, null, null);

    /// <summary>
    /// Creates a <see cref="ContactInfo"/> with primary and optional alternate contact details.
    /// </summary>
    /// <param name="phone">The primary phone number.</param>
    /// <param name="email">The primary email address.</param>
    /// <param name="alternatePhone">An optional alternate phone number.</param>
    /// <param name="alternateEmail">An optional alternate email address.</param>
    /// <returns>A new <see cref="ContactInfo"/> instance.</returns>
    public static ContactInfo Create(string phone, string email, string? alternatePhone, string? alternateEmail)
        => new(phone, email, alternatePhone, alternateEmail);

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
