using CodeDesignPlus.Net.Exceptions.Guards;

namespace CodeDesignPlus.Net.ValueObjects.User;

/// <summary>
/// Represents the type of identification document (e.g., CC, NIT, Passport).
/// This immutable Value Object ensures document types are standardized across the ecosystem.
/// </summary>
public sealed class TypeDocument : IEquatable<TypeDocument>
{
    /// <summary>
    /// Gets the standard abbreviation or code for the document type (e.g., "CC", "NIT").
    /// </summary>
    public string Code { get; private set; } 

    /// <summary>
    /// Gets the full descriptive name of the document type (e.g., "Cédula de Ciudadanía").
    /// </summary>
    public string Name { get; private set; }

    [JsonConstructor]
    private TypeDocument(string code, string name)
    {
        // Normalización: Aseguramos que el código siempre esté en mayúsculas sostenidas
        var normalizedCode = code?.Trim().ToUpperInvariant() ?? string.Empty;
        var normalizedName = name?.Trim() ?? string.Empty;

        Guard.IsNullOrEmpty(normalizedCode, Exceptions.Layer.None, "000 : Code cannot be null or empty");
        Guard.IsGreaterThan(normalizedCode.Length, 3, Exceptions.Layer.None, "000 : Code cannot be greater than 3 characters");
        
        Guard.IsNullOrEmpty(normalizedName, Exceptions.Layer.None, "000 : Name cannot be null or empty");

        this.Code = normalizedCode;
        this.Name = normalizedName;
    }

    /// <summary>
    /// Creates a new immutable valid instance of the TypeDocument value object.
    /// </summary>
    /// <param name="code">The standard abbreviation or code for the document type (e.g., "CC", "NIT").</param>
    /// <param name="name">The full descriptive name of the document type (e.g., "Cédula de Ciudadanía").</param>
    /// <returns>A new instance of the <see cref="TypeDocument"/> class.</returns>
    public static TypeDocument Create(string code, string name)
    {
        return new TypeDocument(code, name);
    }


    /// <summary>
    /// Determines whether two <see cref="TypeDocument"/> instances are equal.
    /// </summary>
    /// <param name="a">The first <see cref="TypeDocument"/> instance.</param>
    /// <param name="b">The second <see cref="TypeDocument"/> instance.</param>
    /// <returns><c>true</c> if the instances are equal; otherwise, <c>false</c>.</returns>
    public static bool operator ==(TypeDocument? a, TypeDocument? b)
    {
        if (ReferenceEquals(a, b)) return true;
        if (a is null || b is null) return false;

        return a.Equals(b);
    }

    /// <summary>
    /// Determines whether two <see cref="TypeDocument"/> instances are not equal.
    /// </summary>
    /// <param name="a">The first <see cref="TypeDocument"/> instance.</param>
    /// <param name="b">The second <see cref="TypeDocument"/> instance.</param>
    /// <returns><c>true</c> if the instances are not equal; otherwise, <c>false</c>.</returns>
    public static bool operator !=(TypeDocument? a, TypeDocument? b) => !(a == b);

    /// <summary>
    /// Determines whether the specified <see cref="TypeDocument"/> is equal to the current <see cref="TypeDocument"/>.
    /// </summary>
    /// <param name="other">The <see cref="TypeDocument"/> to compare with the current <see cref="TypeDocument"/>.</param>
    /// <returns><c>true</c> if the specified <see cref="TypeDocument"/> is equal to the current <see cref="TypeDocument"/>; otherwise, <c>false</c>.</returns>
    public bool Equals(TypeDocument? other)
    {
        if (other is null) return false;

        return this.Code == other.Code && 
               this.Name == other.Name;
    }

    /// <summary>
    /// Determines whether the specified object is equal to the current <see cref="TypeDocument"/>.
    /// </summary>
    /// <param name="obj">The object to compare with the current <see cref="TypeDocument"/>.</param>
    /// <returns><c>true</c> if the specified object is equal to the current <see cref="TypeDocument"/>; otherwise, <c>false</c>.</returns>
    public override bool Equals(object? obj) => obj is TypeDocument other && Equals(other);

    /// <summary>
    /// Serves as the default hash function.
    /// </summary>
    /// <returns>A hash code for the current <see cref="TypeDocument"/>.</returns>
    public override int GetHashCode()
    {
        return HashCode.Combine(Code, Name);
    }
}