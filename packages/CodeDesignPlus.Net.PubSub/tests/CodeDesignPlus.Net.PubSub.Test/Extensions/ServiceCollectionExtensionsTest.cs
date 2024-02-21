using CodeDesignPlus.Net.PubSub.Test.Helpers.Events;
using CodeDesignPlus.Net.PubSub.Extensions;
using CodeDesignPlus.Net.PubSub.Abstractions.Options;

namespace CodeDesignPlus.Net.PubSub.Test.Extensions;

public class ServiceCollectionExtensionsTest
{
    [Fact]
    public void AddPubSub_ServiceCollectionIsNull_ArgumentNullException()
    {
        // Arrange
        ServiceCollection? serviceCollection = null;

        // Act
        var exception = Assert.Throws<ArgumentNullException>(() => serviceCollection.AddPubSub(null));

        // Assert
        Assert.Equal("Value cannot be null. (Parameter 'services')", exception.Message);
    }

    [Fact]
    public void AddPubSub_ConfigurationIsNull_ArgumentNullException()
    {
        // Arrange
        var serviceCollection = new ServiceCollection();

        // Act
        var exception = Assert.Throws<ArgumentNullException>(() => serviceCollection.AddPubSub(null));

        // Assert
        Assert.Equal("Value cannot be null. (Parameter 'configuration')", exception.Message);
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

        Assert.Equal(ConfigurationUtil.PubSubOptions.EnableQueue, value.EnableQueue);
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
        var pubSub = services.FirstOrDefault(x => typeof(IMessage).IsAssignableFrom(x.ImplementationType));

        Assert.NotNull(pubSub);

        Assert.Equal(typeof(PubSubService), pubSub.ImplementationType);
        Assert.Equal(ServiceLifetime.Singleton, pubSub.Lifetime);
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

        var queue = services.FirstOrDefault(x =>
            x.ImplementationType == typeof(QueueService<UserRegisteredEventHandler, UserRegisteredEvent>)
        );

        var queueBackgroundService = services.FirstOrDefault(x =>
            x.ImplementationType == typeof(QueueBackgroundService<UserRegisteredEventHandler, UserRegisteredEvent>)
        );

        var eventHandlerBackgroundService = services.FirstOrDefault(x =>
            x.ImplementationType == typeof(EventHandlerBackgroundService<UserRegisteredEventHandler, UserRegisteredEvent>)
        );

        Assert.NotNull(handler);
        Assert.NotNull(queue);
        Assert.NotNull(queueBackgroundService);
        Assert.NotNull(eventHandlerBackgroundService);

        Assert.True(handler.ImplementationType.IsAssignableGenericFrom(typeof(IEventHandler<>)));
        Assert.Equal(typeof(UserRegisteredEventHandler), handler.ImplementationType);
        Assert.Equal(ServiceLifetime.Singleton, handler.Lifetime);

        Assert.True(queue.ImplementationType.IsAssignableGenericFrom(typeof(IQueueService<,>)));
        Assert.Equal(typeof(QueueService<UserRegisteredEventHandler, UserRegisteredEvent>), queue.ImplementationType);
        Assert.Equal(ServiceLifetime.Singleton, queue.Lifetime);

        Assert.Equal(typeof(QueueBackgroundService<UserRegisteredEventHandler, UserRegisteredEvent>), queueBackgroundService.ImplementationType);
        Assert.Equal(ServiceLifetime.Singleton, queueBackgroundService.Lifetime);
        
        Assert.Equal(typeof(EventHandlerBackgroundService<UserRegisteredEventHandler, UserRegisteredEvent>), eventHandlerBackgroundService.ImplementationType);
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
        var eventHandlers = PubSubExtensions.GetEventHandlers();

        // Assert
        Assert.NotEmpty(eventHandlers);
    }

}
