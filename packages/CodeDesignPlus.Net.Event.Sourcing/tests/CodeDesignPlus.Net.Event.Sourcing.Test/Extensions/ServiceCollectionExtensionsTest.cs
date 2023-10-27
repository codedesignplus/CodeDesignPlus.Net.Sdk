﻿using Microsoft.Extensions.DependencyInjection;
using CodeDesignPlus.Net.Event.Sourcing.Extensions;

namespace CodeDesignPlus.Net.Event.Sourcing.Test.Extensions;

public class ServiceCollectionExtensionsTest
{
    [Fact]
    public void AddEventSourcing_ServiceCollectionIsNull_ArgumentNullException()
    {
        // Arrange
        ServiceCollection? serviceCollection = null;

        // Act
        var exception = Assert.Throws<ArgumentNullException>(() => serviceCollection.AddEventSourcing(null));

        // Assert
        Assert.Equal("Value cannot be null. (Parameter 'services')", exception.Message);
    }

    [Fact]
    public void AddEventSourcing_ConfigurationIsNull_ArgumentNullException()
    {
        // Arrange
        var serviceCollection = new ServiceCollection();

        // Act
        var exception = Assert.Throws<ArgumentNullException>(() => serviceCollection.AddEventSourcing(null));

        // Assert
        Assert.Equal("Value cannot be null. (Parameter 'configuration')", exception.Message);
    }

    [Fact]
    public void AddEventSourcing_SectionNotExist_EventSourcingException()
    {
        // Arrange
        var configuration = ConfigurationUtil.GetConfiguration(new object() { });

        var serviceCollection = new ServiceCollection();

        // Act
        var exception = Assert.Throws<EventSourcingException>(() => serviceCollection.AddEventSourcing(configuration));

        // Assert
        Assert.Equal($"The section {EventSourcingOptions.Section} is required.", exception.Message);
    }

    [Fact]
    public void AddEventSourcing_CheckServices_Success()
    {
        // Arrange
        var configuration = ConfigurationUtil.GetConfiguration();

        var serviceCollection = new ServiceCollection();

        // Act
        serviceCollection.AddEventSourcing(configuration);

        // Assert
        var libraryService = serviceCollection.FirstOrDefault(x => x.ServiceType == typeof(IEventSourcingService));

        Assert.NotNull(libraryService);
        Assert.Equal(ServiceLifetime.Singleton, libraryService.Lifetime);
        Assert.Equal(typeof(EventSourcingService), libraryService.ImplementationType);
    }

    [Fact]
    public void AddEventSourcing_SameOptions_Success()
    {
        // Arrange
        var configuration = ConfigurationUtil.GetConfiguration();

        var serviceCollection = new ServiceCollection();

        // Act
        serviceCollection.AddEventSourcing(configuration);

        // Assert
        var serviceProvider = serviceCollection.BuildServiceProvider();

        var options = serviceProvider.GetService<IOptions<EventSourcingOptions>>();
        var value = options?.Value;

        Assert.NotNull(options);
        Assert.NotNull(value);

        Assert.Equal(ConfigurationUtil.EventSourcingOptions.Name, value.Name);
        Assert.Equal(ConfigurationUtil.EventSourcingOptions.Email, value.Email);
        Assert.Equal(ConfigurationUtil.EventSourcingOptions.Enable, value.Enable);
    }


}