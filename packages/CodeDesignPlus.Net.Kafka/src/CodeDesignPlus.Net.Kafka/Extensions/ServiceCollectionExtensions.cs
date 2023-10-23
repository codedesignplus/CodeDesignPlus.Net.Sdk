using System.Reflection;
using CodeDesignPlus.Net.Core.Abstractions;
using CodeDesignPlus.Net.Event.Bus.Abstractions;
using CodeDesignPlus.Net.Event.Bus.Extensions;
using CodeDesignPlus.Net.Kafka.Abstractions;
using CodeDesignPlus.Net.Kafka.Exceptions;
using CodeDesignPlus.Net.Kafka.Options;
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
    public static IServiceCollection AddKafka<TStartupLogic>(this IServiceCollection services, IConfiguration configuration)
        where TStartupLogic : IStartupServices
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
        services.RegisterProducers<TStartupLogic>(kafkaOptions);
        services.RegisterConsumers<TStartupLogic>(kafkaOptions);

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
    public static IServiceCollection RegisterProducers<TStartupLogic>(this IServiceCollection services, KafkaOptions options)
        where TStartupLogic : IStartupServices
    {
        var config = new ProducerConfig
        {
            BootstrapServers = options.BootstrapServers
        };

        var events = AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(t => t.IsSubclassOf(typeof(EventBase)) && !t.IsAbstract)
            .ToList();

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
    /// <typeparam name="TStartupLogic">The type representing the startup logic.</typeparam>
    /// <param name="services">The service collection to add the services to.</param>
    /// <param name="options">The Kafka options to use for configuration.</param>
    /// <returns>The same service collection so that multiple calls can be chained.</returns>
    public static IServiceCollection RegisterConsumers<TStartupLogic>(this IServiceCollection services, KafkaOptions options)
        where TStartupLogic : IStartupServices
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = options.BootstrapServers,
            GroupId = options.NameMicroservice,
            AutoOffsetReset = AutoOffsetReset.Earliest,
        };

        var eventsHandlers = EventBusExtensions.GetEventHandlers();

        foreach (var eventHandler in eventsHandlers)
        {
            var interfaceEventHandlerGeneric = GetGenericInterface(eventHandler, typeof(IEventHandler<>));

            if (interfaceEventHandlerGeneric == null)
                continue;

            var eventType = GetEventTypeFromGenericInterface(interfaceEventHandlerGeneric);

            if (eventType == null)
                continue;

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

    /// <summary>
    /// Gets the generic interface from a type that matches the specified generic interface type.
    /// </summary>
    /// <param name="type">The type to inspect.</param>
    /// <param name="genericInterfaceType">The generic interface type to match against.</param>
    /// <returns>The matched generic interface type, or null if not found.</returns>
    private static Type GetGenericInterface(Type type, Type genericInterfaceType)
    {
        return type.GetInterfaces().FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == genericInterfaceType);
    }

    /// <summary>
    /// Gets the event type from a generic interface that derives from <see cref="EventBase"/>.
    /// </summary>
    /// <param name="genericInterface">The generic interface to inspect.</param>
    /// <returns>The event type, or null if not found.</returns>
    private static Type GetEventTypeFromGenericInterface(Type genericInterface)
    {
        return genericInterface.GetGenericArguments().FirstOrDefault(x => x.IsClass && !x.IsAbstract && x.IsSubclassOf(typeof(EventBase)));
    }
}

/// <summary>
/// Provides JSON serialization and deserialization using System.Text.Json for Kafka messages.
/// </summary>
/// /// <typeparam name="T">The type of the message to serialize or deserialize.</typeparam>
public class JsonSystemTextSerializer<T> : ISerializer<T>, IDeserializer<T>
{
    /// <summary>
    /// Serializes the specified data to a byte array using JSON.
    /// </summary>
    /// <param name="data">The data to serialize.</param>
    /// <param name="context">The context for the serialization operation.</param>
    /// <returns>The serialized byte array, or null if the data is null.</returns>
    public byte[] Serialize(T data, SerializationContext context)
    {
        if (data == null) return null;
        return System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(data);
    }

    /// <summary>
    /// Deserializes the specified byte array to an object using JSON.
    /// </summary>
    /// <param name="data">The byte array to deserialize.</param>
    /// <param name="isNull">Indicates whether the data is null.</param>
    /// <param name="context">The context for the deserialization operation.</param>
    /// <returns>The deserialized object of type <typeparamref name="T"/>, or the default value if the data is null.</returns>
    public T Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
    {
        if (isNull) return default;
        return System.Text.Json.JsonSerializer.Deserialize<T>(data);
    }
}
