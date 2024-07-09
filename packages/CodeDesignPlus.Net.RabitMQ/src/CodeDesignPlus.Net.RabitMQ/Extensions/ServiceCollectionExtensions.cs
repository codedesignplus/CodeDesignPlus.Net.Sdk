using CodeDesignPlus.Net.RabitMQ.Abstractions;
using CodeDesignPlus.Net.RabitMQ.Exceptions;
using CodeDesignPlus.Net.RabitMQ.Abstractions.Options;
using CodeDesignPlus.Net.RabitMQ.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using CodeDesignPlus.Net.PubSub.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace CodeDesignPlus.Net.RabitMQ.Extensions;

/// <summary>
/// Provides a set of extension methods for CodeDesignPlus.EFCore
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Add CodeDesignPlus.EFCore configuration options
    /// </summary>
    /// <param name="services">The Microsoft.Extensions.DependencyInjection.IServiceCollection to add the service to.</param>
    /// <param name="configuration">The configuration being bound.</param>
    /// <returns>The Microsoft.Extensions.DependencyInjection.IServiceCollection so that additional calls can be chained.</returns>
    public static IServiceCollection AddRabitMQ(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        var section = configuration.GetSection(RabitMQOptions.Section);

        if (!section.Exists())
            throw new RabitMQException($"The section {RabitMQOptions.Section} is required.");

        services
            .AddOptions<RabitMQOptions>()
            .Bind(section)
            .ValidateDataAnnotations();

        var options = section.Get<RabitMQOptions>();

        if (options.Enable)
        {
            services.TryAddSingleton<IMessage, RabitPubSubService>();
            services.AddSingleton<IRabitPubSubService, RabitPubSubService>();
            services.AddSingleton<IRabitConnection, RabitConnection>();
        }

        return services;
    }

}
