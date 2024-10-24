using CodeDesignPlus.Net.Vault.Abstractions.Options;
using CodeDesignPlus.Net.Vault.Exceptions;
using CodeDesignPlus.Net.Vault.Providers;
using CodeDesignPlus.Net.Vault.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CodeDesignPlus.Net.Vault.Extensions;

public static class VaultExtensions
{
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

        services
            .AddSingleton<IVaultTransit, VaultTransit>()
            .AddSingleton(x => VaultClientFactory.Create(options));


        return services;
    }

    public static IConfigurationBuilder AddVault(this IConfigurationBuilder builder, Action<VaultOptions> options)
    {
        var configuration = builder.Build();

        var vaultOptions = configuration.GetSection(VaultOptions.Section).Get<VaultOptions>();

        options.Invoke(vaultOptions);

        var source = new VaultConfigurationSource(vaultOptions);

        builder.Add(source);

        return builder;
    }
}