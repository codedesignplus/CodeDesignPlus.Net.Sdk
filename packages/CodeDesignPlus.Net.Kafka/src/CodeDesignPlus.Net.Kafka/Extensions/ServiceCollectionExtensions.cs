using CodeDesignPlus.Net.PubSub.Abstractions;
using CodeDesignPlus.Net.PubSub.Extensions;
using CodeDesignPlus.Net.Kafka.Options;
using CodeDesignPlus.Net.Kafka.Serializer;
using CodeDesignPlus.Net.Kafka.Services;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (configuration == null)
            throw new ArgumentNullException(nameof(configuration));

        var section = configuration.GetSection(KafkaOptions.Section);

        if (!section.Exists())
            throw new Exceptions.KafkaException($"The section {KafkaOptions.Section} is required.");

        services
            .AddOptions<KafkaOptions>()
            .Bind(section)
            .ValidateDataAnnotations();

        services.AddSingleton<IKafkaEventBus, KafkaEventBus>();

        var kafkaOptions = section.Get<KafkaOptions>();
        services.RegisterProducers(kafkaOptions);
        services.RegisterConsumers(kafkaOptions);

        return services;
    }

    /// <summary>
    /// Registra productores Kafka para todos los eventos que heredan de <see cref="EventBase"/>.
    /// </summary>
    /// <param name="services">La colección de servicios en la que se registran los productores.</param>
    /// <param name="options">Las opciones de configuración para Kafka.</param>
    /// <returns>La colección de servicios actualizada con los productores registrados.</returns>
    /// <remarks>
    /// Este método busca todas las clases que heredan de <see cref="EventBase"/> en el ensamblado en ejecución y
    /// registra un productor Kafka para cada una de ellas en la colección de servicios proporcionada.
    /// </remarks>
    public static IServiceCollection RegisterProducers(this IServiceCollection services, KafkaOptions options)
    {
        var config = new ProducerConfig
        {
            BootstrapServers = options.BootstrapServers
        };

        var events = PubSubExtensions.GetEvents();

        foreach (var eventType in events)
        {
            var producerService = typeof(IProducer<,>).MakeGenericType(typeof(string), eventType);
            var builderType = typeof(ProducerBuilder<,>).MakeGenericType(typeof(string), eventType);

            var serializer = Activator.CreateInstance(typeof(JsonSystemTextSerializer<>).MakeGenericType(eventType));
            var builder = Activator.CreateInstance(builderType, config);

            builder
                .GetType()
                .GetMethod("SetValueSerializer", new[] { typeof(ISerializer<>).MakeGenericType(builderType.GetGenericArguments()[1]) })
                .Invoke(builder, new object[] { serializer });

            var producer = builderType.GetMethod("Build").Invoke(builder, null);

            services.AddSingleton(producerService, producer);
        }

        return services;
    }

    /// <summary>
    /// Registers Kafka consumers in the service collection based on the specified Kafka options.
    /// </summary>
    /// <param name="services">The service collection to add the services to.</param>
    /// <param name="options">The Kafka options to use for configuration.</param>
    /// <returns>The same service collection so that multiple calls can be chained.</returns>
    public static IServiceCollection RegisterConsumers(this IServiceCollection services, KafkaOptions options)
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = options.BootstrapServers,
            GroupId = options.NameMicroservice,
            AutoOffsetReset = AutoOffsetReset.Earliest,
        };

        var eventsHandlers = PubSubExtensions.GetEventHandlers();

        foreach (var eventHandler in eventsHandlers)
        {
            var interfaceEventHandlerGeneric = eventHandler.GetInterfaceEventHandlerGeneric();

            var eventType = interfaceEventHandlerGeneric.GetEventType();

            var consumerService = typeof(IConsumer<,>).MakeGenericType(typeof(string), eventType);
            var consumerBuilderType = typeof(ConsumerBuilder<,>).MakeGenericType(typeof(string), eventType);
            var serializer = Activator.CreateInstance(typeof(JsonSystemTextSerializer<>).MakeGenericType(eventType));
            var builder = Activator.CreateInstance(consumerBuilderType, config);

            builder
                .GetType()
                .GetMethod("SetValueDeserializer", new[] { typeof(IDeserializer<>).MakeGenericType(consumerBuilderType.GetGenericArguments()[1]) })
                .Invoke(builder, new object[] { serializer });

            var consumer = consumerBuilderType.GetMethod("Build").Invoke(builder, null);

            services.AddSingleton(consumerService, consumer);
        }

        return services;
    }
}