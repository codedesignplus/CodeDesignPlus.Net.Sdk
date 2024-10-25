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

        var vaultClientSettings = new VaultClientSettings(
            options.Address,
            new AppRoleAuthMethodInfo(options.RoleId, options.SecretId)
        );

        if (options.Kubernetes.Enable)
        {
            var jwt = File.ReadAllText(options.Kubernetes.PathTokenKubernetes);

            vaultClientSettings = new VaultClientSettings(
                options.Address,
                new KubernetesAuthMethodInfo($"{options.AppName}-{options.Kubernetes.RoleSufix}", jwt)
            );
        }

        return new VaultClient(vaultClientSettings);
    }

}
