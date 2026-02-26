using System.Text.RegularExpressions;
using CodeDesignPlus.Net.Exceptions.Guards;

namespace CodeDesignPlus.Net.ValueObjects.Payment;

/// <summary>
/// Represents a PSE (Pagos Seguros en Línea) payment configuration.
/// </summary>
public sealed partial class Pse : IEquatable<Pse>
{
    [GeneratedRegex(@"^https?:\/\/([a-zA-Z0-9\-\.]+)(:[0-9]+)?(\/[^\s]*)?$", RegexOptions.Compiled)]
    private static partial Regex UrlRegex();

    /// <summary>
    /// The PSE code.
    /// </summary>
    public string PseCode { get; private set; }
    /// <summary>
    /// The type of person (e.g., 'N' for natural, 'J' for juridical).
    /// </summary>
    public string TypePerson { get; private set; }
    /// <summary>
    /// The URL for PSE response.
    /// </summary>
    public string PseResponseUrl { get; private set; }

    [JsonConstructor]
    private Pse(string pseCode, string typePerson, string pseResponseUrl)
    {
        var normalizedPseCode = pseCode?.Trim() ?? string.Empty;
        var normalizedTypePerson = typePerson?.Trim().ToUpperInvariant() ?? string.Empty; 
        var normalizedUrl = pseResponseUrl?.Trim() ?? string.Empty;

        Guard.IsNullOrEmpty(normalizedPseCode, Exceptions.Layer.None, "000 : PseCode cannot be null or empty");
        Guard.IsGreaterThan(normalizedPseCode.Length, 34, Exceptions.Layer.None, "001 : PseCode cannot be greater than 34 characters");

        Guard.IsNullOrEmpty(normalizedTypePerson, Exceptions.Layer.None, "002 : TypePerson cannot be null or empty");
        Guard.IsGreaterThan(normalizedTypePerson.Length, 1, Exceptions.Layer.None, "003 : TypePerson cannot be greater than 1 character"); 

        Guard.IsNullOrEmpty(normalizedUrl, Exceptions.Layer.None, "004 : PseResponseUrl cannot be null or empty");
        Guard.IsGreaterThan(normalizedUrl.Length, 200, Exceptions.Layer.None, "005 : PseResponseUrl cannot be greater than 200 characters");
        Guard.IsFalse(UrlRegex().IsMatch(normalizedUrl), Exceptions.Layer.None, "006 : PseResponseUrl must be a valid format");

        this.PseCode = normalizedPseCode;
        this.TypePerson = normalizedTypePerson;
        this.PseResponseUrl = normalizedUrl;
    }

    /// <summary>
    /// Creates a new instance of the Pse class.
    /// </summary>
    /// <param name="pseCode">The PSE code.</param>
    /// <param name="typePerson">The type of person (e.g., 'N' for natural, 'J' for juridical).</param>
    /// <param name="pseResponseUrl">The URL for PSE response.</param>
    /// <returns>A new instance of the Pse class.</returns>
    public static Pse Create(string pseCode, string typePerson, string pseResponseUrl)
    {
        return new Pse(pseCode, typePerson, pseResponseUrl);
    }

    /// <summary>
    /// Determines whether two Pse instances are equal.
    /// </summary>
    /// <param name="a">The first Pse instance.</param>
    /// <param name="b">The second Pse instance.</param>
    /// <returns>True if the instances are equal; otherwise, false.</returns>
    public static bool operator ==(Pse? a, Pse? b)
    {
        if (ReferenceEquals(a, b)) return true;
        if (a is null || b is null) return false;
        return a.Equals(b);
    }

    /// <summary>
    /// Determines whether two Pse instances are not equal.
    /// </summary>
    /// <param name="a">The first Pse instance.</param>
    /// <param name="b">The second Pse instance.</param>
    /// <returns>True if the instances are not equal; otherwise, false.</returns>
    public static bool operator !=(Pse? a, Pse? b) => !(a == b);

    /// <summary>
    /// Determines whether the current Pse instance is equal to another Pse instance.
    /// </summary>
    /// <param name="other">The Pse instance to compare with the current instance.</param>
    /// <returns>True if the instances are equal; otherwise, false.</returns>
    public bool Equals(Pse? other)
    {
        if (other is null) return false;
        return PseCode == other.PseCode &&
               TypePerson == other.TypePerson &&
               PseResponseUrl == other.PseResponseUrl;
    }

    /// <summary>
    /// Determines whether the current Pse instance is equal to another object.
    /// </summary>
    /// <param name="obj">The object to compare with the current instance.</param>
    /// <returns>True if the object is a Pse instance and is equal to the current instance; otherwise, false.</returns>
    public override bool Equals(object? obj) => obj is Pse other && Equals(other);

    /// <summary>
    /// Returns a hash code for the current Pse instance.
    /// </summary>
    /// <returns>A hash code for the current Pse instance.</returns>
    public override int GetHashCode() => HashCode.Combine(PseCode, TypePerson, PseResponseUrl);
}