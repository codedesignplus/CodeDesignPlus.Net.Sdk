﻿using CodeDesignPlus.Net.Mongo.Diagnostics.Exceptions;
using CodeDesignPlus.Net.Mongo.Diagnostics.Options;
using CodeDesignPlus.Net.Mongo.Diagnostics.Services;
using CodeDesignPlus.Net.Mongo.Diagnostics.Subscriber;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver.Core.Configuration;

namespace CodeDesignPlus.Net.Mongo.Diagnostics.Extensions;

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
    public static IServiceCollection AddMongoDiagnostics(this IServiceCollection services, IConfiguration configuration)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (configuration == null)
            throw new ArgumentNullException(nameof(configuration));

        var section = configuration.GetSection(MongoDiagnosticsOptions.Section);

        if (!section.Exists())
            throw new MongoDiagnosticsException($"The section {MongoDiagnosticsOptions.Section} is required.");

        var options = section.Get<MongoDiagnosticsOptions>();

        services
            .AddOptions<MongoDiagnosticsOptions>()
            .Bind(section)
            .ValidateDataAnnotations();

        if (options.Enable)
        {
            services.AddSingleton<IActivityService, ActivitySourceService>();
            services.AddSingleton<DiagnosticsActivityEventSubscriber>();
        }

        return services;
    }
}