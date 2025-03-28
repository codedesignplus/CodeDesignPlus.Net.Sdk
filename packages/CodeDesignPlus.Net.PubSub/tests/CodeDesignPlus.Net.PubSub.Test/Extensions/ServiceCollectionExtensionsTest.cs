﻿using CodeDesignPlus.Net.PubSub.Diagnostics;
using CodeDesignPlus.Net.PubSub.Extensions;
using CodeDesignPlus.Net.PubSub.Test.Helpers.Events;
using Microsoft.Extensions.Hosting;
using Moq;

namespace CodeDesignPlus.Net.PubSub.Test.Extensions;

public class ServiceCollectionExtensionsTest
{
    [Fact]
    public void AddPubSub_ServiceCollectionIsNull_ArgumentNullException()
    {
        // Arrange
        ServiceCollection? serviceCollection = null;

        // Act
        var exception = Assert.Throws<ArgumentNullException>(() => serviceCollection.AddPubSub(configuration: null));

        // Assert
        Assert.Equal("Value cannot be null. (Parameter 'services')", exception.Message);
    }

    [Fact]
    public void AddPubSubOverride_ServiceCollectionIsNull_ArgumentNullException()
    {
        // Arrange
        ServiceCollection? serviceCollection = null;
        var configuration = Mock.Of<IConfiguration>();

        // Act
        var exception = Assert.Throws<ArgumentNullException>(() => serviceCollection.AddPubSub(configuration: configuration, setupOptions: null));

        // Assert
        Assert.Equal("Value cannot be null. (Parameter 'services')", exception.Message);
    }

    [Fact]
    public void AddPubSub_ConfigurationIsNull_ArgumentNullException()
    {
        // Arrange
        var serviceCollection = new ServiceCollection();

        // Act
        var exception = Assert.Throws<ArgumentNullException>(() => serviceCollection.AddPubSub(configuration: null));

        // Assert
        Assert.Equal("Value cannot be null. (Parameter 'configuration')", exception.Message);
    }

    [Fact]
    public void AddPubSubOverride_ConfigurationIsNull_ArgumentNullException()
    {
        // Arrange
        var serviceCollection = new ServiceCollection();
        var configuration = Mock.Of<IConfiguration>();

        // Act
        var exception = Assert.Throws<ArgumentNullException>(() => serviceCollection.AddPubSub(configuration: null, setupOptions: x => {}));

        // Assert
        Assert.Equal("Value cannot be null. (Parameter 'configuration')", exception.Message);
    }

    [Fact]
    public void AddPubSubOverride_SetupOptionsIsNull_ArgumentNullException()
    {
        // Arrange
        var serviceCollection = new ServiceCollection();
        var configuration = Mock.Of<IConfiguration>();

        // Act
        var exception = Assert.Throws<ArgumentNullException>(() => serviceCollection.AddPubSub(configuration: configuration, setupOptions: null));

        // Assert
        Assert.Equal("Value cannot be null. (Parameter 'setupOptions')", exception.Message);
    }

    [Fact]
    public void AddPubSub_SectionNotExist_PubSubException()
    {
        // Arrange
        var configuration = ConfigurationUtil.GetConfiguration(new object() { });

        var serviceCollection = new ServiceCollection();

        // Act
        var exception = Assert.Throws<PubSubException>(() => serviceCollection.AddPubSub(configuration));

        // Assert
        Assert.Equal($"The section {PubSubOptions.Section} is required.", exception.Message);
    }

    [Fact]
    public void AddPubSub_SameOptions_Success()
    {
        // Arrange
        var configuration = ConfigurationUtil.GetConfiguration();

        var serviceCollection = new ServiceCollection();

        // Act
        serviceCollection.AddPubSub(configuration);

        // Assert
        var serviceProvider = serviceCollection.BuildServiceProvider();

        var options = serviceProvider.GetService<IOptions<PubSubOptions>>();
        var value = options?.Value;

        Assert.NotNull(options);
        Assert.NotNull(value);

        Assert.Equal(ConfigurationUtil.PubSubOptions.UseQueue, value.UseQueue);
    }

    /// <summary>
    /// Valida que se genere la excepción cuando no se encuentra un servicio que implemente la interfaz <see cref="IMessage"/>
    /// </summary>
    [Fact]
    public void AddPubSub_RegisterServices_ServicesBase()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = ConfigurationUtil.GetConfiguration();

        // Act 
        services.AddPubSub(configuration);

        // Assert
        var pubSub = services.FirstOrDefault(x => typeof(IPubSub).IsAssignableFrom(x.ImplementationType));
        var activity = services.FirstOrDefault(x => typeof(IActivityService).IsAssignableFrom(x.ImplementationType));

        var eventQueue = services.FirstOrDefault(x => typeof(IEventQueue).IsAssignableFrom(x.ImplementationType));
        var hostServices = services.Where(x => typeof(IHostedService).IsAssignableFrom(x.ImplementationType));

        Assert.NotNull(pubSub);
        Assert.Equal(typeof(PubSub.Services.PubSubService), pubSub.ImplementationType);
        Assert.Equal(ServiceLifetime.Singleton, pubSub.Lifetime);

        Assert.NotNull(activity);
        Assert.Equal(typeof(ActivitySourceService), activity.ImplementationType);
        Assert.Equal(ServiceLifetime.Singleton, activity.Lifetime);

        Assert.NotNull(eventQueue);
        Assert.Equal(typeof(EventQueueService), eventQueue.ImplementationType);
        Assert.Equal(ServiceLifetime.Singleton, eventQueue.Lifetime);

        Assert.NotEmpty(hostServices);
        Assert.Contains(hostServices, x => x.ImplementationType == typeof(EventQueueBackgroundService));
    }

    [Fact]
    public void AddPubSubOverride_RegisterServices_ServicesBase()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = ConfigurationUtil.GetConfiguration();

        // Act 
        services.AddPubSub(configuration, x =>
        {
            x.UseQueue = true;
            x.SecondsWaitQueue = 4;
            x.EnableDiagnostic = true;
        });

        // Assert
        var pubSub = services.FirstOrDefault(x => typeof(IPubSub).IsAssignableFrom(x.ImplementationType));
        var activity = services.FirstOrDefault(x => typeof(IActivityService).IsAssignableFrom(x.ImplementationType));

        var eventQueue = services.FirstOrDefault(x => typeof(IEventQueue).IsAssignableFrom(x.ImplementationType));
        var hostServices = services.Where(x => typeof(IHostedService).IsAssignableFrom(x.ImplementationType));

        Assert.NotNull(pubSub);
        Assert.Equal(typeof(PubSub.Services.PubSubService), pubSub.ImplementationType);
        Assert.Equal(ServiceLifetime.Singleton, pubSub.Lifetime);

        Assert.NotNull(activity);
        Assert.Equal(typeof(ActivitySourceService), activity.ImplementationType);
        Assert.Equal(ServiceLifetime.Singleton, activity.Lifetime);

        Assert.NotNull(eventQueue);
        Assert.Equal(typeof(EventQueueService), eventQueue.ImplementationType);
        Assert.Equal(ServiceLifetime.Singleton, eventQueue.Lifetime);

        Assert.NotEmpty(hostServices);
        Assert.Contains(hostServices, x => x.ImplementationType == typeof(EventQueueBackgroundService));
    }

    /// <summary>
    /// Valida que se registre los event handler, queues and host service
    /// </summary>
    [Fact]
    public void AddEventHandlers_Services_HandlersQueueAndService()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = ConfigurationUtil.GetConfiguration();

        // Act
        services.AddPubSub(configuration);

        // Assert
        var handler = services.FirstOrDefault(x =>
            x.ImplementationType == typeof(UserRegisteredEventHandler)
        );

        var eventHandlerBackgroundService = services.FirstOrDefault(x =>
            x.ImplementationType == typeof(RegisterEventHandlerBackgroundService<UserRegisteredEventHandler, UserRegisteredEvent>)
        );

        Assert.NotNull(handler);
        Assert.NotNull(eventHandlerBackgroundService);

        Assert.True(handler.ImplementationType.IsAssignableGenericFrom(typeof(IEventHandler<>)));
        Assert.Equal(typeof(UserRegisteredEventHandler), handler.ImplementationType);
        Assert.Equal(ServiceLifetime.Scoped, handler.Lifetime);

        Assert.Equal(typeof(RegisterEventHandlerBackgroundService<UserRegisteredEventHandler, UserRegisteredEvent>), eventHandlerBackgroundService.ImplementationType);
        Assert.Equal(ServiceLifetime.Singleton, eventHandlerBackgroundService.Lifetime);
    }

    /// <summary>
    /// Valida que retorne true si la clase implementa una determinada interfaz generica
    /// </summary>
    [Fact]
    public void IsAssignableGenericFrom_ClassImplementInterface_True()
    {
        // Arrange
        var eventHandler = new UserRegisteredEventHandler();

        // Act
        var success = eventHandler.GetType().IsAssignableGenericFrom(typeof(IEventHandler<>));

        // Assert
        Assert.True(success);
    }

    /// <summary>
    /// Valida que retorne false si la clase no implementa una determinada interfaz generica
    /// </summary>
    [Fact]
    public void IsAssignableGenericFrom_ClassNotImplementInterface_False()
    {
        // Arrange
        var eventHandler = new UserRegisteredEventHandler();

        // Act
        var success = eventHandler.GetType().IsAssignableGenericFrom(typeof(IQueue<>));

        // Assert
        Assert.False(success);
    }

    /// <summary>
    /// Valida que retorne los event handler del assembly
    /// </summary>
    [Fact]
    public void GetEventHandlers_NotEmpty_EventsHandlers()
    {
        // Arrange & Act
        var eventHandlers = PubSubExtensions.GetEventHandlers();

        // Assert
        Assert.NotEmpty(eventHandlers);
    }

}
