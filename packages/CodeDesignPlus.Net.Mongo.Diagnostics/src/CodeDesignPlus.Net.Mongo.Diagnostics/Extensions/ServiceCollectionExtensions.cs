namespace CodeDesignPlus.Net.Mongo.Diagnostics.Extensions
{
    /// <summary>
    /// Provides a set of extension methods for configuring MongoDB diagnostics in CodeDesignPlus.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds MongoDB diagnostics configuration options to the specified IServiceCollection.
        /// </summary>
        /// <param name="services">The Microsoft.Extensions.DependencyInjection.IServiceCollection to add the service to.</param>
        /// <param name="configuration">The configuration being bound.</param>
        /// <returns>The Microsoft.Extensions.DependencyInjection.IServiceCollection so that additional calls can be chained.</returns>
        /// <exception cref="ArgumentNullException">Thrown when services or configuration is null.</exception>
        /// <exception cref="MongoDiagnosticsException">Thrown when the required configuration section is missing.</exception>
        public static IServiceCollection AddMongoDiagnostics(this IServiceCollection services, IConfiguration configuration)
        {
            ArgumentNullException.ThrowIfNull(services);
            ArgumentNullException.ThrowIfNull(configuration);

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

        /// <summary>
        /// Adds MongoDB diagnostics configuration options to the specified IServiceCollection using a setup action.
        /// </summary>
        /// <param name="services">The Microsoft.Extensions.DependencyInjection.IServiceCollection to add the service to.</param>
        /// <param name="setupAction">The action used to configure the MongoDiagnosticsOptions.</param>
        /// <returns>The Microsoft.Extensions.DependencyInjection.IServiceCollection so that additional calls can be chained.</returns>
        /// <exception cref="ArgumentNullException">Thrown when services or setupAction is null.</exception>
        public static IServiceCollection AddMongoDiagnostics(this IServiceCollection services, Action<MongoDiagnosticsOptions> setupAction)
        {
            ArgumentNullException.ThrowIfNull(services);
            ArgumentNullException.ThrowIfNull(setupAction);

            var options = new MongoDiagnosticsOptions();
            setupAction(options);

            if (options.Enable)
            {
                services.AddOptions<MongoDiagnosticsOptions>()
                        .Configure(setupAction)
                        .ValidateDataAnnotations();

                services.AddSingleton<IActivityService, ActivitySourceService>();
                services.AddSingleton<DiagnosticsActivityEventSubscriber>();
            }

            return services;
        }
    }
}