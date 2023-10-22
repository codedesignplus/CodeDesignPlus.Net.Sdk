﻿using CodeDesignPlus.Net.Kafka.Test.Helpers.Events;
using CodeDesignPlus.Net.xUnit.Helpers;
using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;

namespace CodeDesignPlus.Net.Kafka.Extensions;

public class ServiceCollectionExtensionsTest
{
    [Fact]
    public void AddKafka_ServiceCollectionIsNull_ArgumentNullException()
    {
        // Arrange
        ServiceCollection? serviceCollection = null;

        // Act
        var exception = Assert.Throws<ArgumentNullException>(() => serviceCollection.AddKafka<StartupLogic>(null));

        // Assert
        Assert.Equal("Value cannot be null. (Parameter 'services')", exception.Message);
    }

    [Fact]
    public void AddKafka_ConfigurationIsNull_ArgumentNullException()
    {
        // Arrange
        var serviceCollection = new ServiceCollection();

        // Act
        var exception = Assert.Throws<ArgumentNullException>(() => serviceCollection.AddKafka<StartupLogic>(null));

        // Assert
        Assert.Equal("Value cannot be null. (Parameter 'configuration')", exception.Message);
    }

    [Fact]
    public void AddKafka_SectionNotExist_KafkaException()
    {
        // Arrange
        var configuration = ConfigurationUtil.GetConfiguration(new object() { });

        var serviceCollection = new ServiceCollection();

        // Act
        var exception = Assert.Throws<Exceptions.KafkaException>(() => serviceCollection.AddKafka<StartupLogic>(configuration));

        // Assert
        Assert.Equal($"The section {KafkaOptions.Section} is required.", exception.Message);
    }

    [Fact]
    public void AddKafka_CheckServices_Success()
    {
        // Arrange
        var configuration = ConfigurationUtil.GetConfiguration(new { Kafka = OptionUtils.KafkaOptions });

        var serviceCollection = new ServiceCollection();

        // Act
        serviceCollection.AddKafka<StartupLogic>(configuration);

        // Assert
        var libraryService = serviceCollection.FirstOrDefault(x => x.ServiceType == typeof(IKafkaEventBus));

        Assert.NotNull(libraryService);
        Assert.Equal(ServiceLifetime.Singleton, libraryService.Lifetime);
        Assert.Equal(typeof(KafkaEventBus), libraryService.ImplementationType);
    }

    [Fact]
    public void AddKafka_SameOptions_Success()
    {
        // Arrange
        var configuration = ConfigurationUtil.GetConfiguration(new { Kafka = OptionUtils.KafkaOptions });

        var serviceCollection = new ServiceCollection();

        // Act
        serviceCollection.AddKafka<StartupLogic>(configuration);

        // Assert
        var serviceProvider = serviceCollection.BuildServiceProvider();

        var options = serviceProvider.GetService<IOptions<KafkaOptions>>();
        var value = options?.Value;

        Assert.NotNull(options);
        Assert.NotNull(value);
    }


    [Fact]
    public void RegisterProducers_CreateProducersWithEvents_Producers()
    {
        // Arrange
        var configuration = ConfigurationUtil.GetConfiguration(new { Kafka = OptionUtils.KafkaOptions });

        var serviceCollection = new ServiceCollection();

        // Act
        serviceCollection.AddKafka<StartupLogic>(configuration);

        // Assert
        var producer = serviceCollection.FirstOrDefault(x => x.ServiceType == typeof(IProducer<string, UserCreatedEvent>));

        Assert.NotNull(producer);
        Assert.Equal(ServiceLifetime.Singleton, producer.Lifetime);
        Assert.NotNull(producer.ImplementationInstance);
    }


    [Fact]
    public void RegisterConsumers_CreateConsumersWithEvents_Consumers()
    {
        // Arrange
        var configuration = ConfigurationUtil.GetConfiguration(new { Kafka = OptionUtils.KafkaOptions });

        var serviceCollection = new ServiceCollection();

        // Act
        serviceCollection.AddKafka<StartupLogic>(configuration);

        // Assert
        var consumer = serviceCollection.FirstOrDefault(x => x.ServiceType == typeof(IConsumer<string, UserCreatedEvent>));

        Assert.NotNull(consumer);
        Assert.Equal(ServiceLifetime.Singleton, consumer.Lifetime);
        Assert.NotNull(consumer.ImplementationInstance);
    }
}
