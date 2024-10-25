using CodeDesignPlus.Net.Vault.Abstractions.Options;
using VaultSharp;
using VaultSharp.V1.AuthMethods.AppRole;
using VaultSharp.V1.AuthMethods.Kubernetes;

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

        var kubernetesHost = Environment.GetEnvironmentVariable("KUBERNETES_SERVICE_HOST");
        var kubernetesPort = Environment.GetEnvironmentVariable("KUBERNETES_SERVICE_PORT");

        var vaultClientSettings = new VaultClientSettings(
            options.Address,
            new AppRoleAuthMethodInfo(options.RoleId, options.SecretId)
        );

        if (!options.Kubernetes.Enable)
        {
            var jwt = Path.Combine("/var/run/secrets/kubernetes.io/serviceaccount", "token");

            vaultClientSettings = new VaultClientSettings(
                $"http://{kubernetesHost}:{kubernetesPort}",
                new KubernetesAuthMethodInfo($"{options.AppName}-{options.Kubernetes.RoleSufix}", jwt)
            );
        }

        return new VaultClient(vaultClientSettings);
    }

}
