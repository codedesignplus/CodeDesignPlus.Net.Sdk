namespace CodeDesignPlus.Net.Vault.Abstractions;

/// <summary>
/// Interface for Vault Transit operations, providing methods for encryption and decryption.
/// </summary>
/// <remarks>
/// <para>
/// There are two ways to choose the key:
/// </para>
/// <list type="bullet">
/// <item><description><b>A new key per call</b> (<c>EncryptAsync(plaintext, context)</c>): Vault creates a key named
/// with a new <see cref="Guid"/> and returns its name, which the caller stores next to the ciphertext. It suits a
/// handful of long-lived secrets. Every call leaves one more key in Vault, so it does not suit secrets written for many
/// tenants or rotated: the keys pile up, and the ones whose ciphertext was replaced are orphaned.</description></item>
/// <item><description><b>A fixed key</b> (<c>EncryptAsync(keyName, plaintext, context)</c>): one key the caller names,
/// created the first time with key derivation enabled. Vault derives a different key from each context, so the context
/// (for example, the tenant) isolates the secrets: a ciphertext only decrypts with the context it was encrypted with.
/// Use it when the same kind of secret is encrypted for many owners.</description></item>
/// </list>
/// <para>
/// Both are decrypted with <see cref="DecryptAsync(string, string, string)"/>, passing the key name and the same
/// context.
/// </para>
/// </remarks>
public interface IVaultTransit
{
    /// <summary>
    /// Decrypts the specified ciphertext using the given key name and context.
    /// </summary>
    /// <param name="keyName">The name of the key to use for decryption.</param>
    /// <param name="ciphertext">The ciphertext to decrypt.</param>
    /// <param name="context">The context for decryption.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the decrypted plaintext.</returns>
    Task<string> DecryptAsync(string keyName, string ciphertext, string context);

    /// <summary>
    /// Decrypts a list of ciphertexts using the given key name and context.
    /// </summary>
    /// <param name="keyName">The name of the key to use for decryption.</param>
    /// <param name="cipherstext">The list of ciphertexts to decrypt.</param>
    /// <param name="context">The context for decryption.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of decrypted plaintexts.</returns>
    Task<List<string>> DecryptAsync(string keyName, List<string> cipherstext, string context);

    /// <summary>
    /// Encrypts the specified plaintext using the given context.
    /// </summary>
    /// <param name="plaintext">The plaintext to encrypt.</param>
    /// <param name="context">The context for encryption.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a tuple with the key name and the encrypted ciphertext.</returns>
    Task<(string, string)> EncryptAsync(string plaintext, string context);
    
    /// <summary>
    /// Encrypts a list of plaintexts using the given context.
    /// </summary>
    /// <param name="plaintext">The list of plaintexts to encrypt.</param>
    /// <param name="context">The context for encryption.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a tuple with the key name and a list of encrypted ciphertexts.</returns>
    Task<(string, List<string>)> EncryptAsync(List<string> plaintext, string context);

    /// <summary>
    /// Encrypts the specified plaintext with a fixed, derived key, creating the key the first time it is used.
    /// </summary>
    /// <remarks>
    /// The key is created with key derivation enabled, so <paramref name="context"/> selects the key that encrypts and is
    /// required again to decrypt. Use it instead of <see cref="EncryptAsync(string, string)"/> when the same kind of
    /// secret is encrypted for many owners or rotated, so Vault keeps one key instead of one per call.
    /// </remarks>
    /// <param name="keyName">The name of the key, shared by every owner of this kind of secret.</param>
    /// <param name="plaintext">The plaintext to encrypt.</param>
    /// <param name="context">The context the key is derived from, for example the tenant; it is required to decrypt.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the encrypted ciphertext.</returns>
    Task<string> EncryptAsync(string keyName, string plaintext, string context);

    /// <summary>
    /// Encrypts a list of plaintexts with a fixed, derived key, creating the key the first time it is used.
    /// </summary>
    /// <remarks>
    /// Same key handling as <see cref="EncryptAsync(string, string, string)"/>; every item uses the same context.
    /// </remarks>
    /// <param name="keyName">The name of the key, shared by every owner of this kind of secret.</param>
    /// <param name="plaintexts">The list of plaintexts to encrypt.</param>
    /// <param name="context">The context the key is derived from, for example the tenant; it is required to decrypt.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the ciphertexts, in the same order.</returns>
    Task<List<string>> EncryptAsync(string keyName, List<string> plaintexts, string context);
}
