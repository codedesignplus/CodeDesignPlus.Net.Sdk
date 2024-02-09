using CodeDesignPlus.Net.PubSub.Abstractions;
using CodeDesignPlus.Net.PubSub.Extensions;
using CodeDesignPlus.Net.Kafka.Options;
using CodeDesignPlus.Net.Kafka.Services;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using CodeDesignPlus.Net.Core.Abstractions;

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

        var options = section.Get<KafkaOptions>();

        // services.AddSingleton(x => {

        //     var a = new ProducerBuilder<string, UserCreatedEvent2>(options.ProducerConfig);

        //     var b = a.Build();

        //     return b;
        // });

        return services;
    }

}
