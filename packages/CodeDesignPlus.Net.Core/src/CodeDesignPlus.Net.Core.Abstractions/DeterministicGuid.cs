namespace CodeDesignPlus.Net.Core.Abstractions;

/// <summary>
/// Creates name-based identifiers (UUID version 5, RFC 4122 section 4.3).
/// </summary>
/// <remarks>
/// The same namespace and name always produce the same identifier, which is what makes end-to-end
/// idempotency possible without storing anything: a retry computes the same value and the primary key
/// rejects the duplicate on its own. It is the alternative to keeping a table of already-seen messages.
/// <para>
/// Every caller must define its own namespace so that two unrelated concepts can never collide, and must
/// build the name from the natural key of what it identifies. A monthly charge, for example, keys on
/// tenant, source and period: leaving the period out lets the source be charged only once in its lifetime.
/// </para>
/// <para>
/// Beware of byte order: the first three fields of a .NET <see cref="Guid"/> are little-endian and RFC 4122
/// defines them big-endian. Without the conversion, two implementations of the same algorithm produce
/// different identifiers for the same input.
/// </para>
/// </remarks>
public static class DeterministicGuid
{
    /// <summary>
    /// Creates a version 5 UUID from a namespace and a name.
    /// </summary>
    /// <param name="namespaceId">The namespace that scopes the name, unique per concept.</param>
    /// <param name="name">The natural key of what is being identified, encoded as UTF-8.</param>
    /// <returns>The deterministic identifier for the given namespace and name.</returns>
    /// <exception cref="ArgumentException">Thrown when name is null or empty.</exception>
    public static Guid Create(Guid namespaceId, string name)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);

        var namespaceBytes = namespaceId.ToByteArray();
        SwapByteOrder(namespaceBytes);

        var nameBytes = Encoding.UTF8.GetBytes(name);

        var payload = new byte[namespaceBytes.Length + nameBytes.Length];
        namespaceBytes.CopyTo(payload, 0);
        nameBytes.CopyTo(payload, namespaceBytes.Length);

        // SHA-1 is mandated by the RFC for version 5. It is not used here for any cryptographic purpose.
        var hash = SHA1.HashData(payload);

        var result = new byte[16];
        Array.Copy(hash, 0, result, 0, 16);

        result[6] = (byte)((result[6] & 0x0F) | 0x50);
        result[8] = (byte)((result[8] & 0x3F) | 0x80);

        SwapByteOrder(result);

        return new Guid(result);
    }

    /// <summary>
    /// Creates a version 5 UUID from a namespace and the parts of a natural key.
    /// </summary>
    /// <param name="namespaceId">The namespace that scopes the name, unique per concept.</param>
    /// <param name="parts">The parts of the natural key, joined with a separator that cannot appear in them.</param>
    /// <returns>The deterministic identifier for the given namespace and key.</returns>
    /// <exception cref="ArgumentException">Thrown when parts is null or empty.</exception>
    /// <remarks>
    /// The separator matters: concatenating the parts without one makes two different keys collapse into the
    /// same name, and therefore into the same identifier.
    /// </remarks>
    public static Guid Create(Guid namespaceId, params string[] parts)
    {
        if (parts is null || parts.Length == 0)
        {
            throw new ArgumentException("At least one part is required to build the name.", nameof(parts));
        }

        return Create(namespaceId, string.Join('|', parts));
    }

    /// <summary>
    /// Converts the first three fields of a GUID between little-endian and big-endian.
    /// </summary>
    /// <param name="guid">The 16 bytes of the identifier, modified in place.</param>
    private static void SwapByteOrder(byte[] guid)
    {
        (guid[0], guid[3]) = (guid[3], guid[0]);
        (guid[1], guid[2]) = (guid[2], guid[1]);
        (guid[4], guid[5]) = (guid[5], guid[4]);
        (guid[6], guid[7]) = (guid[7], guid[6]);
    }
}
