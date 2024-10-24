using CodeDesignPlus.Net.Vault.Abstractions.Options;
using Microsoft.Extensions.Configuration;

namespace CodeDesignPlus.Net.Vault.Providers;

public class VaultConfigurationSource(VaultOptions options) : IConfigurationSource
{
    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        return new VaultConfigurationProvider(options);
    }
}
