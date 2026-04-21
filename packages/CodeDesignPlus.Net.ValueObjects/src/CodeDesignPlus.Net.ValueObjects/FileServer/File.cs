using CodeDesignPlus.Net.Exceptions.Guards;

namespace CodeDesignPlus.Net.ValueObjects.FileServer;

/// <summary>
/// Represents an immutable reference to a file stored on a file server.
/// Captures the metadata needed by aggregates to locate and describe the file
/// without coupling them to the storage infrastructure.
/// </summary>
public sealed class File : IEquatable<File>
{
    /// <summary>
    /// Gets the unique identifier of the file within the file server.
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Gets the storage target (e.g., bucket, container, or folder path) where the file resides.
    /// </summary>
    public string Target { get; private set; }

    /// <summary>
    /// Gets the original file name including its extension (e.g., "invoice.pdf").
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// Gets the file extension in lowercase without the leading dot (e.g., "pdf", "jpg").
    /// </summary>
    public string Extension { get; private set; }

    /// <summary>
    /// Gets the MIME type of the file (e.g., "application/pdf", "image/jpeg").
    /// </summary>
    public string MimeType { get; private set; }

    /// <summary>
    /// Gets the size of the file in bytes.
    /// </summary>
    public long Size { get; private set; }

    [JsonConstructor]
    private File(Guid id, string target, string name, string extension, string mimeType, long size)
    {
        Guard.GuidIsEmpty(id, Exceptions.Layer.None, "000 : File Id cannot be empty.");

        var normalizedTarget = target?.Trim() ?? string.Empty;
        Guard.IsNullOrEmpty(normalizedTarget, Exceptions.Layer.None, "001 : Target cannot be null or empty.");
        Guard.IsGreaterThan(normalizedTarget.Length, 512, Exceptions.Layer.None, "002 : Target cannot exceed 512 characters.");

        var normalizedName = name?.Trim() ?? string.Empty;
        Guard.IsNullOrEmpty(normalizedName, Exceptions.Layer.None, "003 : Name cannot be null or empty.");
        Guard.IsGreaterThan(normalizedName.Length, 255, Exceptions.Layer.None, "004 : Name cannot exceed 255 characters.");

        var normalizedExtension = extension?.Trim().TrimStart('.').ToLowerInvariant() ?? string.Empty;
        Guard.IsNullOrEmpty(normalizedExtension, Exceptions.Layer.None, "005 : Extension cannot be null or empty.");
        Guard.IsGreaterThan(normalizedExtension.Length, 20, Exceptions.Layer.None, "006 : Extension cannot exceed 20 characters.");

        var normalizedMimeType = mimeType?.Trim().ToLowerInvariant() ?? string.Empty;
        Guard.IsNullOrEmpty(normalizedMimeType, Exceptions.Layer.None, "007 : MimeType cannot be null or empty.");
        Guard.IsGreaterThan(normalizedMimeType.Length, 127, Exceptions.Layer.None, "008 : MimeType cannot exceed 127 characters.");

        Guard.IsLessThan(size, 0L, Exceptions.Layer.None, "009 : Size cannot be negative.");

        Id = id;
        Target = normalizedTarget;
        Name = normalizedName;
        Extension = normalizedExtension;
        MimeType = normalizedMimeType;
        Size = size;
    }

    /// <summary>
    /// Creates a new immutable reference to a file stored on the file server.
    /// </summary>
    /// <param name="id">The unique identifier of the file.</param>
    /// <param name="target">The storage target (bucket, container, or folder path).</param>
    /// <param name="name">The original file name including its extension.</param>
    /// <param name="extension">The file extension (with or without the leading dot).</param>
    /// <param name="mimeType">The MIME type of the file.</param>
    /// <param name="size">The size of the file in bytes.</param>
    /// <returns>A new <see cref="File"/> instance.</returns>
    public static File Create(Guid id, string target, string name, string extension, string mimeType, long size)
    {
        return new File(id, target, name, extension, mimeType, size);
    }

    /// <summary>
    /// Determines whether two <see cref="File"/> instances are equal.
    /// </summary>
    public static bool operator ==(File? a, File? b)
    {
        if (ReferenceEquals(a, b))
            return true;

        if (a is null || b is null)
            return false;

        return a.Equals(b);
    }

    /// <summary>
    /// Determines whether two <see cref="File"/> instances are not equal.
    /// </summary>
    public static bool operator !=(File? a, File? b) => !(a == b);

    /// <summary>
    /// Determines whether the specified <see cref="File"/> is equal to the current instance.
    /// </summary>
    public bool Equals(File? other)
    {
        if (other is null)
            return false;

        return Id == other.Id &&
               Target == other.Target &&
               Name == other.Name &&
               Extension == other.Extension &&
               MimeType == other.MimeType &&
               Size == other.Size;
    }

    /// <summary>
    /// Determines whether the specified object is equal to the current <see cref="File"/> instance.
    /// </summary>
    public override bool Equals(object? obj) => obj is File other && Equals(other);

    /// <summary>
    /// Returns a hash code for the current <see cref="File"/> instance.
    /// </summary>
    public override int GetHashCode() => HashCode.Combine(Id, Target, Name, Extension, MimeType, Size);
}
