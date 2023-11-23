using CodeDesignPlus.Net.Mongo.Abstractions.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace CodeDesignPlus.Net.Mongo.Extensions;

/// <summary>
/// Extension methods for setting up MongoDB services in an <see cref="IServiceCollection" />.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds MongoDB services to the specified <see cref="IServiceCollection" />.
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
            throw new Exceptions.MongoException($"The section {MongoOptions.Section} is required.");

        var options = section.Get<MongoOptions>();

        services
            .AddOptions<MongoOptions>()
            .Bind(section)
            .ValidateDataAnnotations();

        services.AddSingleton<IMongoClient>(x => new MongoClient(options.ConnectionString));

        return services;
    }

    
    /// <summary>
    /// Add all repositories that implement the <see cref="IRepositoryBase{TKey, TUserKey}"/> interface
    /// </summary>
    /// <typeparam name="TKey">Type of data that will identify the record</typeparam>
    /// <typeparam name="TUserKey">Type of data that the user will identify</typeparam>
    /// <param name="services">The IServiceCollection to add services to.</param>
    public static void AddRepositories<TKey, TUserKey>(this IServiceCollection services)
    {
        var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes());

        var repositories = types.Where(x => !x.IsNested && !x.IsInterface && typeof(IRepositoryBase<TKey, TUserKey>).IsAssignableFrom(x));

        foreach (var repository in @repositories)
        {
            var @interface = repository.GetInterface($"I{repository.Name}", false);

            services.AddSingleton(@interface, repository);
        }
    }

}
