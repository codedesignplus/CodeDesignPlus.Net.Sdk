using System.Security.Authentication;

namespace CodeDesignPlus.Net.Mongo.Extensions;

/// <summary>
/// Provides extension methods for adding and configuring MongoDB services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds MongoDB services to the specified IServiceCollection.
    /// </summary>
    /// <typeparam name="TStartup">The type of the startup class.</typeparam>
    /// <param name="services">The IServiceCollection to add the services to.</param>
    /// <param name="configuration">The configuration being bound.</param>
    /// <returns>The IServiceCollection so that additional calls can be chained.</returns>
    /// <exception cref="ArgumentNullException">Thrown when services or configuration is null.</exception>
    /// <exception cref="Exceptions.MongoException">Thrown when the MongoOptions section is missing.</exception>
    public static IServiceCollection AddMongo<TStartup>(this IServiceCollection services, IConfiguration configuration) where TStartup : IStartup
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

        if(!options.Enable)
            return services;

        if (options.Diagnostic.Enable)
            services.AddMongoDiagnostics(x =>
            {
                x.Enable = options.Diagnostic.Enable;
                x.EnableCommandText = options.Diagnostic.EnableCommandText;
            });

        services.AddSingleton<IMongoClient>((serviceProvider) =>
        {
            var mongoOptions = serviceProvider.GetRequiredService<IOptions<MongoOptions>>().Value;

            MongoSerializerRegistration.RegisterSerializers();

            var mongoUrl = MongoUrl.Create(mongoOptions.ConnectionString);

            var clientSettings = MongoClientSettings.FromUrl(mongoUrl);

            if (mongoOptions.Diagnostic.Enable)
                clientSettings.ClusterConfigurator = builder =>
                {
                    builder.SubscribeDiagnosticsActivityEventSubscriber(serviceProvider);
                };

            clientSettings.SslSettings = new SslSettings
            {
                EnabledSslProtocols = mongoOptions.SslProtocols
            };

            var mongoClient = new MongoClient(clientSettings);

            return mongoClient;
        });

        if (options.RegisterAutomaticRepositories)
            services.AddRepositories<TStartup>();

        if (options.RegisterHealthCheck)
            services
                .AddHealthChecks()
                .AddMongoDb(x =>
                {
                    var mongoClient = x.GetRequiredService<IMongoClient>();

                    return mongoClient;
                }, name: "MongoDB", tags: ["ready"]);

        return services;
    }

    /// <summary>
    /// Adds repositories to the specified IServiceCollection.
    /// </summary>
    /// <typeparam name="TStartup">The type of the startup class.</typeparam>
    /// <param name="services">The IServiceCollection to add the repositories to.</param>
    /// <exception cref="ArgumentNullException">Thrown when services is null.</exception>
    public static void AddRepositories<TStartup>(this IServiceCollection services) where TStartup : IStartup
    {
        var types = typeof(TStartup).Assembly.GetTypes();

        var repositories = types.Where(x => !x.IsNested && !x.IsInterface && typeof(IRepositoryBase).IsAssignableFrom(x));

        foreach (var repository in repositories)
        {
            var @interface = repository.GetInterfaces().FirstOrDefault(x => x.Name == $"I{repository.Name}");

            Exceptions.MongoException.ThrowIfNull(@interface, $"The interface I{repository.Name} is required.");

            services.AddSingleton(@interface, repository);
        }
    }
}