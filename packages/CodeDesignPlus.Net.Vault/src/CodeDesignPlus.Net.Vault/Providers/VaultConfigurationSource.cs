using CodeDesignPlus.Net.Vault.Abstractions.Options;
using Microsoft.Extensions.Configuration;

namespace CodeDesignPlus.Net.Vault.Providers;

/// <summary>
/// Builds the <see cref="IConfigurationProvider"/> for the Vault configuration source.
/// /// Represents a configuration source for Vault.
/// </summary>
/// <param name="options">The options used to configure the Vault.</param>
public class VaultConfigurationSource(VaultOptions options) : IConfigurationSource
{
    /// <summary>
    /// The configuration options for the Vault.
    /// </summary>
    /// <param name="options">The options used to configure the Vault.</param>
    /// <returns>The configuration options for the Vault.</returns>
    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        return new VaultConfigurationProvider(options);
    }
}
