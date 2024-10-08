﻿namespace CodeDesignPlus.Net.RabbitMQ.Extensions;

/// <summary>
/// Extension methods for setting up RabbitMQ services in an <see cref="IServiceCollection"/>.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds RabbitMQ services to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <param name="configuration">The configuration to bind the RabbitMQ options.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="services"/> or <paramref name="configuration"/> is null.</exception>
    /// <exception cref="RabbitMQException">Thrown if the RabbitMQ configuration section does not exist.</exception>
    public static IServiceCollection AddRabbitMQ(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        var section = configuration.GetSection(RabbitMQOptions.Section);

        if (!section.Exists())
            throw new RabbitMQException($"The section {RabbitMQOptions.Section} is required.");

        services
            .AddOptions<RabbitMQOptions>()
            .Bind(section)
            .ValidateDataAnnotations();

        var options = section.Get<RabbitMQOptions>();

        if (options.Enable)
        {
            services.AddPubSub(configuration, x =>
            {
                x.EnableDiagnostic = options.EnableDiagnostic;
                x.RegisterAutomaticHandlers = options.RegisterAutomaticHandlers;
                x.SecondsWaitQueue = options.SecondsWaitQueue;
                x.UseQueue = options.UseQueue;
            });
            services.TryAddSingleton<IMessage, RabbitPubSubService>();
            services.TryAddSingleton<IRabbitPubSubService, RabbitPubSubService>();
            services.TryAddSingleton<IRabbitConnection, RabbitConnection>();
            services.TryAddSingleton<IChannelProvider, ChannelProvider>();
            services.AddHostedService<InitializeBackgroundService>();
        }

        return services;
    }
}