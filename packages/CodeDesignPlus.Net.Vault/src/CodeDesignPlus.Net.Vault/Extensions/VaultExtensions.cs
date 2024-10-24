using CodeDesignPlus.Net.Vault.Abstractions.Options;
using CodeDesignPlus.Net.Vault.Providers;
using Microsoft.Extensions.Configuration;

namespace CodeDesignPlus.Net.Vault.Extensions;

public static class VaultExtensions
{
    public static IConfigurationBuilder AddVault(this IConfigurationBuilder configuration, Action<VaultOptions> options)
    {
        var vaultOptions = new VaultConfigurationSource(options);
        
        configuration.Add(vaultOptions);

        return configuration;
    }
}