using System;
using System.Text;
using CodeDesignPlus.Net.Vault.Abstractions.Options;
using VaultSharp;
using VaultSharp.V1.SecretsEngines.Transit;

namespace CodeDesignPlus.Net.Vault.Services;

public class VaultTransit(IVaultClient client, IOptions<VaultOptions> options) : IVaultTransit
{

    public async Task<string> DecryptAsync(string keyName, string ciphertext)
    {
        var encodedContext = Convert.ToBase64String(Encoding.UTF8.GetBytes(options.Value.AppName));

        var decryptOptions = new DecryptRequestOptions
        {
            CipherText = ciphertext,
            Base64EncodedContext = encodedContext,
        };

        var decryptionResponse = await client.V1.Secrets.Transit.DecryptAsync(keyName, decryptOptions, mountPoint: $"{options.Value.Solution}-transit");
        return decryptionResponse.Data.Base64EncodedPlainText;
    }

    public async Task<(string, string)> EncryptAsync(string plaintext)
    {
        var keyName = Guid.NewGuid().ToString();

        var encodedPlainText = Convert.ToBase64String(Encoding.UTF8.GetBytes(plaintext));
        var encodedContext = Convert.ToBase64String(Encoding.UTF8.GetBytes(options.Value.AppName));

        var encryptOptions = new EncryptRequestOptions
        {
            Base64EncodedPlainText = encodedPlainText,
            Base64EncodedContext = encodedContext,
        };

        var encryptionResponse = await client.V1.Secrets.Transit.EncryptAsync(keyName, encryptOptions, $"{options.Value.Solution}-transit");

        return (keyName, encryptionResponse.Data.CipherText);
    }
}
