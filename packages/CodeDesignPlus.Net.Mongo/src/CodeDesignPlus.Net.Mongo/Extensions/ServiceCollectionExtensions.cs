
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
    public static IServiceCollection AddMongo<TStartup>(this IServiceCollection services, IConfiguration configuration) where TStartup : IStartupServices
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        var section = configuration.GetSection(MongoOptions.Section);

        if (!section.Exists())
            throw new Exceptions.MongoException($"The section {MongoOptions.Section} is required.");

        var options = section.Get<MongoOptions>();

        services
            .AddOptions<MongoOptions>()
            .Bind(section)
            .ValidateDataAnnotations();

        if (options.Diagnostic.Enable)
            services.AddMongoDiagnostics(x =>
            {
                x.Enable = options.Diagnostic.Enable;
                x.EnableCommandText = options.Diagnostic.EnableCommandText;
            });

        services.AddSingleton<IMongoClient>((serviceProvider) =>
        {
            var mongoUrl = MongoUrl.Create(options.ConnectionString);

            var clientSettings = MongoClientSettings.FromUrl(mongoUrl);

            if (options.Diagnostic.Enable)
                clientSettings.ClusterConfigurator = builder =>
                {
                    builder.SubscribeDiagnosticsActivityEventSubscriber(serviceProvider);
                };

            var mongoClient = new MongoClient(clientSettings);

            return mongoClient;
        });

        if (options.RegisterAutomaticRepositories)
            services.AddRepositories<TStartup>();

        return services;
    }

    /// <summary>
    /// Add all repositories that implement the <see cref="IRepositoryBase{TKey, TUserKey}"/> interface
    /// </summary>
    /// <typeparam name="TStartup">The type of the startup class that implements the <see cref="IStartupServices"/> interface.</typeparam>
    /// <param name="services">The IServiceCollection to add services to.</param>
    public static void AddRepositories<TStartup>(this IServiceCollection services) where TStartup : IStartupServices
    {
        var types = typeof(TStartup).Assembly.GetTypes();

        var repositories = types.Where(x => !x.IsNested && !x.IsInterface && typeof(IRepositoryBase).IsAssignableFrom(x));

        foreach (var repository in @repositories)
        {
            var @interface = repository.GetInterface($"I{repository.Name}", false);

            services.AddSingleton(@interface, repository);
        }
    }

}
