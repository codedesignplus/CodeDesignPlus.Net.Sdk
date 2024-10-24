namespace CodeDesignPlus.Net.Vault.Abstractions;

public interface IVaultTransit
{
    Task<(string, string)> EncryptAsync(string plaintext);

    Task<string> DecryptAsync(string key, string ciphertext);
}
