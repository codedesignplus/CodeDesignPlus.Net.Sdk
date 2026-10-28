namespace CodeDesignPlus.Net.Vault.Services;

/// <summary>
/// Implementation of the IVaultTransit interface for Vault Transit operations, providing methods for encryption and decryption.
/// </summary>
public class VaultTransit(VaultClientProvider clientProvider, IOptions<VaultOptions> options) : IVaultTransit
{
    /// <summary>
    /// Fixed keys already confirmed as derived in this process, so the check costs one round trip per key and not per call.
    /// </summary>
    private readonly ConcurrentDictionary<string, Lazy<Task>> derivedKeys = new();

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

        var decryptionResponse = await clientProvider.Client.V1.Secrets.Transit.DecryptAsync(keyName, decryptOptions, mountPoint: $"{options.Value.Solution}-transit");

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

        var decryptionResponse = await clientProvider.Client.V1.Secrets.Transit.DecryptAsync(keyName, decryptOptions, mountPoint: $"{options.Value.Solution}-transit");

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

        var encryptionResponse = await clientProvider.Client.V1.Secrets.Transit.EncryptAsync(keyName, encryptOptions, $"{options.Value.Solution}-transit");

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

        var encryptionResponse = await clientProvider.Client.V1.Secrets.Transit.EncryptAsync(keyName, encryptOptions, mountPoint: $"{options.Value.Solution}-transit");

        return (keyName, encryptionResponse.Data.BatchedResults.Select(x => x.CipherText).ToList());
    }

    /// <summary>
    /// Encrypts the specified plaintext with a fixed, derived key, creating the key the first time it is used.
    /// </summary>
    /// <param name="keyName">The name of the key, shared by every owner of this kind of secret.</param>
    /// <param name="plaintext">The plaintext to encrypt.</param>
    /// <param name="context">The context the key is derived from, for example the tenant; it is required to decrypt.</param>
    /// <exception cref="ArgumentNullException">keyName, plaintext or context is null.</exception>
    /// <exception cref="ArgumentException">keyName or context is empty.</exception>
    /// <exception cref="VaultException">The key exists in Vault without key derivation.</exception>
    /// <returns>A task that represents the asynchronous operation. The task result contains the encrypted ciphertext.</returns>
    public async Task<string> EncryptAsync(string keyName, string plaintext, string context)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(keyName);
        ArgumentNullException.ThrowIfNull(plaintext);
        ArgumentException.ThrowIfNullOrWhiteSpace(context);

        await EnsureDerivedKeyAsync(keyName);

        var encryptOptions = new EncryptRequestOptions
        {
            Base64EncodedPlainText = Convert.ToBase64String(Encoding.UTF8.GetBytes(plaintext)),
            Base64EncodedContext = Convert.ToBase64String(Encoding.UTF8.GetBytes(context))
        };

        var encryptionResponse = await clientProvider.Client.V1.Secrets.Transit.EncryptAsync(keyName, encryptOptions, MountPoint);

        return encryptionResponse.Data.CipherText;
    }

    /// <summary>
    /// Encrypts a list of plaintexts with a fixed, derived key, creating the key the first time it is used.
    /// </summary>
    /// <param name="keyName">The name of the key, shared by every owner of this kind of secret.</param>
    /// <param name="plaintexts">The list of plaintexts to encrypt.</param>
    /// <param name="context">The context the key is derived from, for example the tenant; it is required to decrypt.</param>
    /// <exception cref="ArgumentNullException">keyName, plaintexts or context is null.</exception>
    /// <exception cref="ArgumentException">keyName or context is empty.</exception>
    /// <exception cref="VaultException">The key exists in Vault without key derivation.</exception>
    /// <returns>A task that represents the asynchronous operation. The task result contains the ciphertexts, in the same order.</returns>
    public async Task<List<string>> EncryptAsync(string keyName, List<string> plaintexts, string context)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(keyName);
        ArgumentNullException.ThrowIfNull(plaintexts);
        ArgumentException.ThrowIfNullOrWhiteSpace(context);

        if (plaintexts.Count == 0)
        {
            return [];
        }

        await EnsureDerivedKeyAsync(keyName);

        var base64Context = Convert.ToBase64String(Encoding.UTF8.GetBytes(context));

        var encryptOptions = new EncryptRequestOptions
        {
            BatchedEncryptionItems = [.. plaintexts.Select(x => new EncryptionItem
            {
                Base64EncodedContext = base64Context,
                Base64EncodedPlainText = Convert.ToBase64String(Encoding.UTF8.GetBytes(x))
            })]
        };

        var encryptionResponse = await clientProvider.Client.V1.Secrets.Transit.EncryptAsync(keyName, encryptOptions, MountPoint);

        return [.. encryptionResponse.Data.BatchedResults.Select(x => x.CipherText)];
    }

    /// <summary>
    /// Gets the mount point of the Transit engine of the solution.
    /// </summary>
    private string MountPoint => $"{options.Value.Solution}-transit";

    /// <summary>
    /// Makes sure the fixed key exists with key derivation enabled, once per key and process.
    /// </summary>
    /// <remarks>
    /// The key is created explicitly instead of trusting the upsert of the encrypt endpoint: the upsert only creates a
    /// derived key because the request carries a context, and a key that already exists without derivation would ignore
    /// the context and let any tenant decrypt another's secret. A failed attempt is forgotten so the next call retries.
    /// </remarks>
    private async Task EnsureDerivedKeyAsync(string keyName)
    {
        var ensure = derivedKeys.GetOrAdd(keyName, name => new Lazy<Task>(() => CreateDerivedKeyAsync(name)));

        try
        {
            await ensure.Value;
        }
        catch
        {
            derivedKeys.TryRemove(new KeyValuePair<string, Lazy<Task>>(keyName, ensure));

            throw;
        }
    }

    /// <summary>
    /// Creates the key with key derivation enabled and checks that the one Vault keeps is derived.
    /// </summary>
    private async Task CreateDerivedKeyAsync(string keyName)
    {
        var transit = clientProvider.Client.V1.Secrets.Transit;

        // Creating a key that already exists leaves it untouched, so two replicas creating it at once end with one key.
        await transit.CreateEncryptionKeyAsync(keyName, new CreateKeyRequestOptions
        {
            Type = options.Value.Transit.KeyType,
            Derived = true
        }, MountPoint);

        var key = await transit.ReadEncryptionKeyAsync(keyName, MountPoint);

        if (!key.Data.Derived)
        {
            throw new VaultException($"The transit key '{keyName}' exists without key derivation, so the context would not isolate its secrets.");
        }
    }
}
