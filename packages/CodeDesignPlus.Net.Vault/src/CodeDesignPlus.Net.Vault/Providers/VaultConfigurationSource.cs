using CodeDesignPlus.Net.Vault.Abstractions.Options;
using Microsoft.Extensions.Configuration;

namespace CodeDesignPlus.Net.Vault.Providers;

public class VaultConfigurationSource(Action<VaultOptions> action) : IConfigurationSource
{
    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        var options = new VaultOptions();
        action.Invoke(options);

        return new VaultConfigurationProvider(options);
    }
}
