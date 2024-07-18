namespace CodeDesignPlus.Net.Kafka.Extensions;

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
            services.TryAddSingleton<IMessage, KafkaPubSub>();
            services.AddSingleton<IKafkaPubSub, KafkaPubSub>();

            services.AddSingleton(x =>
            {
                var producerBuilder = new ProducerBuilder<string, IDomainEvent>(options.ProducerConfig);

                producerBuilder.SetValueSerializer(new Serializer.JsonSystemTextSerializer<IDomainEvent>());

                return producerBuilder.BuildWithInstrumentation();
            });
        }


        return services;
    }

}
