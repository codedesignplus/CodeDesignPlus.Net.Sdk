using VaultSharp.V1.AuthMethods.Token;

namespace CodeDesignPlus.Net.Vault.Services;

/// <summary>
/// Factory for creating instances of <see cref="IVaultClient"/>.
/// </summary>
public static class VaultClientFactory
{
    /// <summary>
    /// Create a new instance of <see cref="IVaultClient"/> with the specified options.
    /// </summary>
    /// <param name="options">The options used to configure the Vault client.</param>
    /// <exception cref="ArgumentNullException">options is null.</exception>
    /// <returns>A new instance of <see cref="IVaultClient"/>.</returns>
    public static IVaultClient Create(VaultOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        if (options.TypeAuth == TypeAuth.Token)
            return new VaultClient(new VaultClientSettings(options.Address, new TokenAuthMethodInfo(options.Token)));

        if (options.TypeAuth == TypeAuth.AppRole)
            return new VaultClient(new VaultClientSettings(options.Address, new AppRoleAuthMethodInfo(options.RoleId, options.SecretId)));

        if (options.TypeAuth == TypeAuth.Kubernetes)
        {
            var jwt = File.ReadAllText(options.Kubernetes.PathTokenKubernetes);

            return new VaultClient(new VaultClientSettings(
                options.Address,
                new KubernetesAuthMethodInfo($"{options.AppName}-{options.Kubernetes.RoleSufix}", jwt)
            ));
        }

        throw new VaultException("The authentication type is not defined.");
    }

}
