﻿using CodeDesignPlus.Net.EventStore.Extensions;
using CodeDesignPlus.Net.xUnit.Extensions;

namespace CodeDesignPlus.Net.EventStore.Test.Extensions;

public class ServiceCollectionExtensionsTest
{
    [Fact]
    public void AddEventStore_ServiceCollectionIsNull_ArgumentNullException()
    {
        // Arrange
        ServiceCollection? serviceCollection = null;

        // Act
        var exception = Assert.Throws<ArgumentNullException>(() => serviceCollection.AddEventStore(null));

        // Assert
        Assert.Equal("Value cannot be null. (Parameter 'services')", exception.Message);
    }

    [Fact]
    public void AddEventStore_ConfigurationIsNull_ArgumentNullException()
    {
        // Arrange
        var serviceCollection = new ServiceCollection();

        // Act
        var exception = Assert.Throws<ArgumentNullException>(() => serviceCollection.AddEventStore(null));

        // Assert
        Assert.Equal("Value cannot be null. (Parameter 'configuration')", exception.Message);
    }

    [Fact]
    public void AddEventStore_SectionNotExist_EventStoreException()
    {
        // Arrange
        var configuration = ConfigurationUtil.GetConfiguration(new object() { });

        var serviceCollection = new ServiceCollection();

        // Act
        var exception = Assert.Throws<EventStoreException>(() => serviceCollection.AddEventStore(configuration));

        // Assert
        Assert.Equal($"The section {EventStoreOptions.Section} is required.", exception.Message);
    }

    [Fact]
    public void AddEventStore_CheckServices_Success()
    {
        // Arrange
        var configuration = ConfigurationUtil.GetConfiguration(new
        {
            Core = OptionsUtil.CoreOptions,
            EventStore = OptionsUtil.EventStoreOptions
        });

        var serviceCollection = new ServiceCollection();

        // Act
        serviceCollection.AddEventStore(configuration);

        // Assert
        var libraryService = serviceCollection.FirstOrDefault(x => x.ServiceType == typeof(IEventStore));

        Assert.NotNull(libraryService);
        Assert.Equal(ServiceLifetime.Singleton, libraryService.Lifetime);
        Assert.Equal(typeof(EventStoreService), libraryService.ImplementationType);
    }

    [Fact]
    public void AddEventStore_SameOptions_Success()
    {
        // Arrange
        var configuration = ConfigurationUtil.GetConfiguration(new
        {
            Core = OptionsUtil.CoreOptions,
            EventStore = OptionsUtil.EventStoreOptions
        });

        var serviceCollection = new ServiceCollection();

        // Act
        serviceCollection.AddEventStore(configuration);

        // Assert
        var serviceProvider = serviceCollection.BuildServiceProvider();

        var options = serviceProvider.GetService<IOptions<EventStoreOptions>>();
        var value = options?.Value;

        Assert.NotNull(options);
        Assert.NotNull(value);

        Assert.Contains(OptionsUtil.EventStoreOptions.Servers, x => value.Servers.Any(o => o.Value.ConnectionString == x.Value.ConnectionString));
    }


}
