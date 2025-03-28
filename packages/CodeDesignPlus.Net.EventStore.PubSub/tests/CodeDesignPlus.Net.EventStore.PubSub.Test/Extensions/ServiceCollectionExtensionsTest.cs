﻿using CodeDesignPlus.Net.EventStore.PubSub.Extensions;
using CodeDesignPlus.Net.xUnit.Extensions;

namespace CodeDesignPlus.Net.EventStore.PubSub.Test.Extensions;

public class ServiceCollectionExtensionsTest
{
    [Fact]
    public void AddEventStorePubSub_ServiceCollectionIsNull_ArgumentNullException()
    {
        // Arrange
        ServiceCollection? serviceCollection = null;

        // Act
        var exception = Assert.Throws<ArgumentNullException>(() => serviceCollection.AddEventStorePubSub(null));

        // Assert
        Assert.Equal("Value cannot be null. (Parameter 'services')", exception.Message);
    }

    [Fact]
    public void AddEventStorePubSub_ConfigurationIsNull_ArgumentNullException()
    {
        // Arrange
        var serviceCollection = new ServiceCollection();

        // Act
        var exception = Assert.Throws<ArgumentNullException>(() => serviceCollection.AddEventStorePubSub(null));

        // Assert
        Assert.Equal("Value cannot be null. (Parameter 'configuration')", exception.Message);
    }

    [Fact]
    public void AddEventStorePubSub_SectionNotExist_EventStorePubSubException()
    {
        // Arrange
        var configuration = ConfigurationUtil.GetConfiguration(new object() { });

        var serviceCollection = new ServiceCollection();

        // Act
        var exception = Assert.Throws<EventStorePubSubException>(() => serviceCollection.AddEventStorePubSub(configuration));

        // Assert
        Assert.Equal($"The section {EventStorePubSubOptions.Section} is required.", exception.Message);
    }

    [Fact]
    public void AddEventStorePubSub_CheckServices_Success()
    {
        // Arrange
        var configuration = ConfigurationUtil.GetConfiguration(new { Core = OptionsUtil.CoreOptions, EventStore = OptionsUtil.EventStoreOptions, EventStorePubSub = OptionsUtil.Options });

        var serviceCollection = new ServiceCollection();

        // Act
        serviceCollection.AddEventStorePubSub(configuration);

        // Assert
        var libraryService = serviceCollection.FirstOrDefault(x => x.ServiceType == typeof(IEventStorePubSub));

        Assert.NotNull(libraryService);
        Assert.Equal(ServiceLifetime.Singleton, libraryService.Lifetime);
        Assert.Equal(typeof(EventStorePubSubService), libraryService.ImplementationType);
    }

    [Fact]
    public void AddEventStorePubSub_SameOptions_Success()
    {
        // Arrange
        var configuration = ConfigurationUtil.GetConfiguration(new { Core = OptionsUtil.CoreOptions, EventStore = OptionsUtil.EventStoreOptions, EventStorePubSub = OptionsUtil.Options });

        var serviceCollection = new ServiceCollection();

        // Act
        serviceCollection.AddEventStorePubSub(configuration);

        // Assert
        var serviceProvider = serviceCollection.BuildServiceProvider();

        var options = serviceProvider.GetService<IOptions<EventStorePubSubOptions>>();
        var value = options?.Value;

        Assert.NotNull(options);
        Assert.NotNull(value);
    }
}
