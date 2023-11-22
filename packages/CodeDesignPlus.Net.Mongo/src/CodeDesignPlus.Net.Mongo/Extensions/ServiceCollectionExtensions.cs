using CodeDesignPlus.Net.Mongo.Exceptions;
using CodeDesignPlus.Net.Mongo.Abstractions.Options;
using CodeDesignPlus.Net.Mongo.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CodeDesignPlus.Net.Mongo.Extensions;

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
    public static IServiceCollection AddMongo(this IServiceCollection services, IConfiguration configuration)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (configuration == null)
            throw new ArgumentNullException(nameof(configuration));

        var section = configuration.GetSection(MongoOptions.Section);

        if (!section.Exists())
            throw new MongoException($"The section {MongoOptions.Section} is required.");

        services
            .AddOptions<MongoOptions>()
            .Bind(section)
            .ValidateDataAnnotations();

        services.AddSingleton<IMongoService, MongoService>();

        return services;
    }

}
