namespace CodeDesignPlus.Net.Vault.Providers;

/// <summary>
/// Builds the <see cref="IConfigurationProvider"/> for the Vault configuration source.
/// /// Represents a configuration source for Vault.
/// </summary>
/// <param name="options">The options used to configure the Vault.</param>
public class VaultConfigurationSource(VaultOptions options) : IConfigurationSource
{
    /// <summary>
    /// Builds the <see cref="IConfigurationProvider"/> for this source.
    /// </summary>
    /// <param name="builder">The <see cref="IConfigurationBuilder"/>.</param>
    /// <returns>An <see cref="IConfigurationProvider"/></returns>
    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        return new VaultConfigurationProvider(options);
    }
}
