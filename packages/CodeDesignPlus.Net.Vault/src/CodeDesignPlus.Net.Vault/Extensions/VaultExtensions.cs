namespace CodeDesignPlus.Net.Vault.Extensions;

/// <summary>
/// Provides extension methods for configuring Vault services and settings.
/// </summary>
public static class VaultExtensions
{
    /// <summary>
    /// Adds Vault services to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="configuration">The <see cref="IConfiguration"/> containing Vault configuration settings.</param>
    /// <returns>The updated <see cref="IServiceCollection"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="services"/> or <paramref name="configuration"/> is null.</exception>
    /// <exception cref="VaultException">Thrown if the Vault configuration section does not exist.</exception>
    public static IServiceCollection AddVault(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        var section = configuration.GetSection(VaultOptions.Section);

        if (!section.Exists())
            throw new VaultException($"The section {VaultOptions.Section} is required.");

        services
            .AddOptions<VaultOptions>()
            .Bind(section)
            .ValidateDataAnnotations();

        var options = section.Get<VaultOptions>();

        if (options.Enable)
        {
            services.AddSingleton<IVaultTransit, VaultTransit>();
            services.AddSingleton(x => VaultClientFactory.Create(options));
        }

        return services;
    }

    /// <summary>
    /// Adds Vault configuration to the specified <see cref="IConfigurationBuilder"/>.
    /// </summary>
    /// <param name="builder">The <see cref="IConfigurationBuilder"/> to add configuration to.</param>
    /// <param name="options">An action to configure the <see cref="VaultOptions"/>.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="builder"/> is null.</exception>
    /// <exception cref="VaultException">Thrown if the Vault configuration section does not exist.</exception>
    /// <returns>The updated <see cref="IConfigurationBuilder"/>.</returns>
    public static IConfigurationBuilder AddVault(this IConfigurationBuilder builder, Action<VaultOptions> options = null)
    {
        ArgumentNullException.ThrowIfNull(builder);

        var configuration = builder.Build();

        var vaultOptions = configuration.GetSection(VaultOptions.Section).Get<VaultOptions>();

        options?.Invoke(vaultOptions);

        if(vaultOptions.Enable)
            builder.Add(new VaultConfigurationSource(vaultOptions));

        return builder;
    }
}