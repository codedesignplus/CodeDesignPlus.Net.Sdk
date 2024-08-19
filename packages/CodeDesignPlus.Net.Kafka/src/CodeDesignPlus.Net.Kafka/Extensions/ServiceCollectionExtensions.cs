namespace CodeDesignPlus.Net.Kafka.Extensions;

/// <summary>
/// Provides extension methods for adding Kafka services to the dependency injection container.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds Kafka services to the specified IServiceCollection.
    /// </summary>
    /// <param name="services">The IServiceCollection to add the services to.</param>
    /// <param name="configuration">The configuration being bound.</param>
    /// <returns>The IServiceCollection so that additional calls can be chained.</returns>
    /// <exception cref="Exceptions.KafkaException">Thrown when the Kafka configuration section is missing.</exception>
    public static IServiceCollection AddKafka(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        var section = configuration.GetSection(KafkaOptions.Section);

        if (!section.Exists())
            throw new Exceptions.KafkaException($"The section {KafkaOptions.Section} is required.");

        services
            .AddOptions<KafkaOptions>()
            .Bind(section)
            .ValidateDataAnnotations();

        var options = section.Get<KafkaOptions>();

        if (options.Enable)
        {
            services.AddPubSub(configuration, x =>
            {
                x.EnableDiagnostic = options.EnableDiagnostic;
                x.RegisterAutomaticHandlers = options.RegisterAutomaticHandlers;
                x.SecondsWaitQueue = options.SecondsWaitQueue;
                x.UseQueue = options.UseQueue;
            });
            services.TryAddSingleton<IMessage, KafkaPubSub>();
            services.TryAddSingleton<IKafkaPubSub, KafkaPubSub>();

            services.TryAddSingleton(x =>
            {
                var producerBuilder = new ProducerBuilder<string, IDomainEvent>(options.ProducerConfig);

                producerBuilder.SetValueSerializer(new JsonSystemTextSerializer<IDomainEvent>());

                return producerBuilder.BuildWithInstrumentation();
            });
        }

        return services;
    }
}