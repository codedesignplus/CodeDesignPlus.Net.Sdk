﻿using CodeDesignPlus.Net.PubSub.Abstractions;
using CodeDesignPlus.Net.RabbitMQ.Abstractions.Options;
using CodeDesignPlus.Net.RabbitMQ.Exceptions;
using CodeDesignPlus.Net.RabbitMQ.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace CodeDesignPlus.Net.RabbitMQ.Extensions;

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
            services.TryAddSingleton<IMessage, RabbitPubSubService>();
            services.AddSingleton<IRabbitPubSubService, RabbitPubSubService>();
            services.AddSingleton<IRabbitConnection, RabbitConnection>();
        }

        return services;
    }

}