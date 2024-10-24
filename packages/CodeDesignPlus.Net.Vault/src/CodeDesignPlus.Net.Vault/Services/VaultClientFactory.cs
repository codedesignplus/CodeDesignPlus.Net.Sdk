using System;
using CodeDesignPlus.Net.Vault.Abstractions.Options;
using VaultSharp;
using VaultSharp.V1.AuthMethods.AppRole;
using VaultSharp.V1.AuthMethods.Kubernetes;

namespace CodeDesignPlus.Net.Vault.Services;

public class VaultClientFactory
{
    public static IVaultClient Create(VaultOptions options)
    {
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
