using CodeDesignPlus.Net.Event.Bus.Test;
using CodeDesignPlus.Net.Event.Bus.Test.Helpers.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;

namespace CodeDesignPlus.Net.Event.Bus.Extensions;

public class ServiceCollectionExtensionsTest
{
    [Fact]
    public void AddEventBus_ServiceCollectionIsNull_ArgumentNullException()
    {
        // Arrange
        ServiceCollection? serviceCollection = null;

        // Act
        var exception = Assert.Throws<ArgumentNullException>(() => serviceCollection.AddEventBus(null));

        // Assert
        Assert.Equal("Value cannot be null. (Parameter 'services')", exception.Message);
    }

    [Fact]
    public void AddEventBus_ConfigurationIsNull_ArgumentNullException()
    {
        // Arrange
        var serviceCollection = new ServiceCollection();

        // Act
        var exception = Assert.Throws<ArgumentNullException>(() => serviceCollection.AddEventBus(null));

        // Assert
        Assert.Equal("Value cannot be null. (Parameter 'configuration')", exception.Message);
    }

    [Fact]
    public void AddEventBus_SectionNotExist_EventBusException()
    {
        // Arrange
        var configuration = ConfigurationUtil.GetConfiguration(new object() { });

        var serviceCollection = new ServiceCollection();

        // Act
        var exception = Assert.Throws<EventBusException>(() => serviceCollection.AddEventBus(configuration));

        // Assert
        Assert.Equal($"The section {EventBusOptions.Section} is required.", exception.Message);
    }

    [Fact]
    public void AddEventBus_SameOptions_Success()
    {
        // Arrange
        var configuration = ConfigurationUtil.GetConfiguration();

        var serviceCollection = new ServiceCollection();

        // Act
        serviceCollection.AddEventBus(configuration);

        // Assert
        var serviceProvider = serviceCollection.BuildServiceProvider();

        var options = serviceProvider.GetService<IOptions<EventBusOptions>>();
        var value = options?.Value;

        Assert.NotNull(options);
        Assert.NotNull(value);

        Assert.Equal(ConfigurationUtil.EventBusOptions.Name, value.Name);
        Assert.Equal(ConfigurationUtil.EventBusOptions.Email, value.Email);
        Assert.Equal(ConfigurationUtil.EventBusOptions.Enable, value.Enable);
    }

    /// <summary>
    /// Valida que se genere la excepción cuando no se encuentra un servicio que implemente la interfaz <see cref="IEventBus"/>
    /// </summary>
    [Fact]
    public void AddEventBus_RegisterServices_ServicesBase()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = ConfigurationUtil.GetConfiguration();

        // Act 
        services.AddEventBus(configuration);

        // Assert
        var subscriptionManager = services.FirstOrDefault(x => typeof(ISubscriptionManager).IsAssignableFrom(x.ImplementationType));
        var eventBus = services.FirstOrDefault(x => typeof(IEventBus).IsAssignableFrom(x.ImplementationType));

        Assert.NotNull(subscriptionManager);
        Assert.NotNull(eventBus);

        Assert.Equal(typeof(SubscriptionManager), subscriptionManager.ImplementationType);
        Assert.Equal(ServiceLifetime.Singleton, subscriptionManager.Lifetime);

        Assert.Equal(typeof(EventBusService), eventBus.ImplementationType);
        Assert.Equal(ServiceLifetime.Singleton, eventBus.Lifetime);
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
        services.AddEventBus(configuration);

        // Act
        services.AddEventsHandlers<Startup>();

        // Assert
        var handler = services.FirstOrDefault(x =>
            x.ImplementationType == typeof(UserRegisteredEventHandler)
        );

        var queue = services.FirstOrDefault(x =>
            x.ImplementationType == typeof(QueueService<UserRegisteredEventHandler, UserRegisteredEvent>)
        );

        var hostService = services.FirstOrDefault(x =>
            x.ImplementationType == typeof(QueueBackgroundService<UserRegisteredEventHandler, UserRegisteredEvent>)
        );

        Assert.NotNull(handler);
        Assert.NotNull(queue);
        Assert.NotNull(hostService);

        Assert.True(handler.ImplementationType.IsAssignableGenericFrom(typeof(IEventHandler<>)));
        Assert.Equal(typeof(UserRegisteredEventHandler), handler.ImplementationType);
        Assert.Equal(ServiceLifetime.Transient, handler.Lifetime);

        Assert.True(queue.ImplementationType.IsAssignableGenericFrom(typeof(IQueueService<,>)));
        Assert.Equal(typeof(QueueService<UserRegisteredEventHandler, UserRegisteredEvent>), queue.ImplementationType);
        Assert.Equal(ServiceLifetime.Singleton, queue.Lifetime);

        Assert.True(hostService.ImplementationType.IsAssignableGenericFrom(typeof(IEventBusBackgroundService<,>)));
        Assert.Equal(typeof(QueueBackgroundService<UserRegisteredEventHandler, UserRegisteredEvent>), hostService.ImplementationType);
        Assert.Equal(ServiceLifetime.Transient, hostService.Lifetime);
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
        var success = eventHandler.GetType().IsAssignableGenericFrom(typeof(IQueueService<,>));

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
        var eventHandlers = EventBusExtensions.GetEventHandlers();

        // Assert
        Assert.NotEmpty(eventHandlers);
    }

}
