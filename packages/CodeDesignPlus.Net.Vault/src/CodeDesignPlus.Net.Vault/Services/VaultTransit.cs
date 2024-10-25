using System.Text;
using CodeDesignPlus.Net.Vault.Abstractions.Options;
using VaultSharp;
using VaultSharp.V1.SecretsEngines.Transit;

namespace CodeDesignPlus.Net.Vault.Services;

/// <summary>
/// Implementation of the IVaultTransit interface for Vault Transit operations, providing methods for encryption and decryption.
/// </summary>
public class VaultTransit(IVaultClient client, IOptions<VaultOptions> options) : IVaultTransit
{

    /// <summary>
    /// Decrypts the specified ciphertext using the given key name and context.
    /// </summary>
    /// <param name="keyName">The name of the key to use for decryption.</param>
    /// <param name="ciphertext">The ciphertext to decrypt.</param>
    /// <param name="context">The context for decryption.</param>
    /// <exception cref="ArgumentNullException">keyName, ciphertext or context is null.</exception>
    /// <returns>A task that represents the asynchronous operation. The task result contains the decrypted plaintext.</returns>
    public async Task<string> DecryptAsync(string keyName, string ciphertext, string context)
    {
        ArgumentNullException.ThrowIfNull(keyName);
        ArgumentNullException.ThrowIfNull(ciphertext);
        ArgumentNullException.ThrowIfNull(context);

        var decryptOptions = new DecryptRequestOptions
        {
            CipherText = ciphertext,
            Base64EncodedContext = Convert.ToBase64String(Encoding.UTF8.GetBytes(context)),
        };

        var decryptionResponse = await client.V1.Secrets.Transit.DecryptAsync(keyName, decryptOptions, mountPoint: $"{options.Value.Solution}-transit");

        var value = Convert.ToString(Encoding.UTF8.GetString(Convert.FromBase64String(decryptionResponse.Data.Base64EncodedPlainText)));

        return value;
    }

    /// <summary>
    /// Decrypts a list of ciphertexts using the given key name and context.
    /// </summary>
    /// <param name="keyName">The name of the key to use for decryption.</param>
    /// <param name="cipherstext">The list of ciphertexts to decrypt.</param>
    /// <param name="context">The context for decryption.</param>
    /// <exception cref="ArgumentNullException">keyName, cipherstext or context is null.</exception>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of decrypted plaintexts.</returns>
    public async Task<List<string>> DecryptAsync(string keyName, List<string> cipherstext, string context)
    {
        ArgumentNullException.ThrowIfNull(keyName);
        ArgumentNullException.ThrowIfNull(cipherstext);
        ArgumentNullException.ThrowIfNull(context);

        if (cipherstext.Count == 0)
            return [];

        var items = cipherstext.Select(ciphertext => new DecryptionItem
        {
            Base64EncodedContext = Convert.ToBase64String(Encoding.UTF8.GetBytes(context)),
            CipherText = ciphertext
        }).ToList();

        var decryptOptions = new DecryptRequestOptions
        {
            BatchedDecryptionItems = items
        };

        var decryptionResponse = await client.V1.Secrets.Transit.DecryptAsync(keyName, decryptOptions, mountPoint: $"{options.Value.Solution}-transit");

        var values = decryptionResponse.Data.BatchedResults.Select(x =>
        {
            return Convert.ToString(Encoding.UTF8.GetString(Convert.FromBase64String(x.Base64EncodedPlainText)));
        }).ToList();

        return values;
    }

    /// <summary>
    /// Encrypts the specified plaintext using the given context.
    /// </summary>
    /// <param name="plaintext">The plaintext to encrypt.</param>
    /// <param name="context">The context for encryption.</param>
    /// <exception cref="ArgumentNullException">plaintext or context is null.</exception>
    /// <returns>A task that represents the asynchronous operation. The task result contains a tuple with the key name and the encrypted ciphertext.</returns>
    public async Task<(string, string)> EncryptAsync(string plaintext, string context)
    {
        ArgumentNullException.ThrowIfNull(plaintext);
        ArgumentNullException.ThrowIfNull(context);


        var keyName = Guid.NewGuid().ToString();

        var encryptOptions = new EncryptRequestOptions
        {
            Base64EncodedPlainText = Convert.ToBase64String(Encoding.UTF8.GetBytes(plaintext)),
            Base64EncodedContext = Convert.ToBase64String(Encoding.UTF8.GetBytes(context)),
            KeyType = options.Value.Transit.KeyType
        };

        var encryptionResponse = await client.V1.Secrets.Transit.EncryptAsync(keyName, encryptOptions, $"{options.Value.Solution}-transit");

        return (keyName, encryptionResponse.Data.CipherText);
    }

    /// <summary>
    /// Encrypts a list of plaintexts using the given context.
    /// </summary>
    /// <param name="plaintext">The list of plaintexts to encrypt.</param>
    /// <param name="context">The context for encryption.</param>
    /// <exception cref="ArgumentNullException">plaintext or context is null.</exception>
    /// <returns>A task that represents the asynchronous operation. The task result contains a tuple with the key name and a list of encrypted ciphertexts.</returns>
    public async Task<(string, List<string>)> EncryptAsync(List<string> plaintext, string context)
    {
        ArgumentNullException.ThrowIfNull(plaintext);
        ArgumentNullException.ThrowIfNull(context);

        if (plaintext.Count == 0)
            return (Guid.Empty.ToString(), []);

        var keyName = Guid.NewGuid().ToString();

        var items = plaintext.Select(x => new EncryptionItem
        {
            Base64EncodedContext = Convert.ToBase64String(Encoding.UTF8.GetBytes(context)),
            Base64EncodedPlainText = Convert.ToBase64String(Encoding.UTF8.GetBytes(x)),
            KeyType = options.Value.Transit.KeyType
        }).ToList();

        var encryptOptions = new EncryptRequestOptions
        {
            BatchedEncryptionItems = items
        };

        var encryptionResponse = await client.V1.Secrets.Transit.EncryptAsync(keyName, encryptOptions, mountPoint: $"{options.Value.Solution}-transit");

        return (keyName, encryptionResponse.Data.BatchedResults.Select(x => x.CipherText).ToList());
    }
}
