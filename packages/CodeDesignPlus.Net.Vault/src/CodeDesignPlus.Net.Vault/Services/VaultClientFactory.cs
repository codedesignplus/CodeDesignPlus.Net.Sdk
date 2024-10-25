namespace CodeDesignPlus.Net.Vault.Services;

/// <summary>
/// Factory for creating instances of <see cref="IVaultClient"/>.
/// </summary>
public class VaultClientFactory
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
            var host = Environment.GetEnvironmentVariable("KUBERNETES_SERVICE_HOST");
            var port = Environment.GetEnvironmentVariable("KUBERNETES_SERVICE_PORT");

            var jwt = Path.Combine("/var/run/secrets/kubernetes.io/serviceaccount", "token");

            vaultClientSettings = new VaultClientSettings(
                $"http://{host}:{port}",
                new KubernetesAuthMethodInfo($"{options.AppName}-{options.Kubernetes.RoleSufix}", jwt)
            );
        }

        return new VaultClient(vaultClientSettings);
    }

}
