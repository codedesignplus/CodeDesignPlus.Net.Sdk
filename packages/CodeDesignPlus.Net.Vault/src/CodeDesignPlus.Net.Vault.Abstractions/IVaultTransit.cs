namespace CodeDesignPlus.Net.Vault.Abstractions;

/// <summary>
/// Interface for Vault Transit operations, providing methods for encryption and decryption.
/// </summary>
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
}
