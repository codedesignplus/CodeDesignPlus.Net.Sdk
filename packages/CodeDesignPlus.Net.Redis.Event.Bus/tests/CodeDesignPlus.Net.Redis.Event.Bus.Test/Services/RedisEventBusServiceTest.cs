using CodeDesignPlus.Net.Event.Bus.Abstractions;
using CodeDesignPlus.Net.Redis.Abstractions;
using CodeDesignPlus.Net.Redis.Event.Bus.Extensions;
using CodeDesignPlus.Net.Redis.Event.Bus.Services;
using CodeDesignPlus.Net.Redis.Event.Bus.Test.Helpers.Events;
using CodeDesignPlus.Net.Redis.Event.Bus.Test.Helpers.Memory;
using CodeDesignPlus.Net.Redis.Extensions;
using CodeDesignPlus.Net.Event.Bus.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using Moq;
using StackExchange.Redis;
using CodeDesignPlus.Net.xUnit.Helpers.Server;
using O = Microsoft.Extensions.Options;

namespace CodeDesignPlus.Net.Redis.Event.Bus.Test.Services;

public class RedisEventBusServiceTest : IClassFixture<RedisContainer>
{
    private readonly RedisContainer redisContainer;

    public RedisEventBusServiceTest(RedisContainer redisContainer)
    {
        this.redisContainer = redisContainer;
    }

    [Fact]
    public void Constructor_RedisIsNull_ArgumentNullException()
    {
        // Arrange
        var subscriptionManager = Mock.Of<ISubscriptionManager>();
        var serviceProvider = Mock.Of<IServiceProvider>();
        var logger = Mock.Of<ILogger<RedisEventBusService>>();
        var options = O.Options.Create(OptionsUtil.RedisEventBusOptions);
        var eventBusOptions = O.Options.Create(OptionsUtil.EventBusOptions);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new RedisEventBusService(null, subscriptionManager, serviceProvider, logger, options, eventBusOptions));
    }

    [Fact]
    public void Constructor_SubscriptionManagerIsNull_ArgumentNullException()
    {
        // Arrange
        var factory = Mock.Of<IRedisServiceFactory>();
        var serviceProvider = Mock.Of<IServiceProvider>();
        var logger = Mock.Of<ILogger<RedisEventBusService>>();
        var options = O.Options.Create(OptionsUtil.RedisEventBusOptions);
        var eventBusOptions = O.Options.Create(OptionsUtil.EventBusOptions);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new RedisEventBusService(factory, null, serviceProvider, logger, options, eventBusOptions));
    }

    [Fact]
    public void Constructor_ServiceProviderIsNull_ArgumentNullException()
    {
        // Arrange
        var factory = Mock.Of<IRedisServiceFactory>();
        var subscriptionManager = Mock.Of<ISubscriptionManager>();
        var logger = Mock.Of<ILogger<RedisEventBusService>>();
        var options = O.Options.Create(OptionsUtil.RedisEventBusOptions);
        var eventBusOptions = O.Options.Create(OptionsUtil.EventBusOptions);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new RedisEventBusService(factory, subscriptionManager, null, logger, options, eventBusOptions));
    }

    [Fact]
    public void Constructor_LoggerIsNull_ArgumentNullException()
    {
        // Arrange
        var factory = Mock.Of<IRedisServiceFactory>();
        var subscriptionManager = Mock.Of<ISubscriptionManager>();
        var serviceProvider = Mock.Of<IServiceProvider>();
        var options = O.Options.Create(OptionsUtil.RedisEventBusOptions);
        var eventBusOptions = O.Options.Create(OptionsUtil.EventBusOptions);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new RedisEventBusService(factory, subscriptionManager, serviceProvider, null, options, eventBusOptions));
    }

    [Fact]
    public void Constructor_OptionsIsNull_ArgumentNullException()
    {
        // Arrange
        var factory = Mock.Of<IRedisServiceFactory>();
        var subscriptionManager = Mock.Of<ISubscriptionManager>();
        var serviceProvider = Mock.Of<IServiceProvider>();
        var logger = Mock.Of<ILogger<RedisEventBusService>>();
        var eventBusOptions = O.Options.Create(OptionsUtil.EventBusOptions);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new RedisEventBusService(factory, subscriptionManager, serviceProvider, logger, null, eventBusOptions));
    }

    [Fact]
    public void Constructor_OptionsEventBusIsNull_ArgumentNullException()
    {
        // Arrange
        var factory = Mock.Of<IRedisServiceFactory>();
        var subscriptionManager = Mock.Of<ISubscriptionManager>();
        var serviceProvider = Mock.Of<IServiceProvider>();
        var logger = Mock.Of<ILogger<RedisEventBusService>>();
        var options = O.Options.Create(OptionsUtil.RedisEventBusOptions);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new RedisEventBusService(factory, subscriptionManager, serviceProvider, logger, options, null));
    }

    [Fact]
    public void PublishAsync_EventIsNull_ArgumentNullException()
    {
        // Arrange
        var factory = Mock.Of<IRedisServiceFactory>();
        var subscriptionManager = Mock.Of<ISubscriptionManager>();
        var serviceProvider = Mock.Of<IServiceProvider>();
        var logger = Mock.Of<ILogger<RedisEventBusService>>();
        var options = O.Options.Create(OptionsUtil.RedisEventBusOptions);
        var eventBusOptions = O.Options.Create(OptionsUtil.EventBusOptions);

        var redisEventBusService = new RedisEventBusService(factory, subscriptionManager, serviceProvider, logger, options, eventBusOptions);

        // Act & Arc
        Assert.ThrowsAsync<ArgumentNullException>(() => redisEventBusService.PublishAsync(null, CancellationToken.None));
    }

    [Fact]
    public async Task PublishAsync_InvokePublish_Succeses()
    {
        // Arrange
        var @event = new UserCreatedEvent()
        {
            Id = 1,
            Names = "Code",
            Lastnames = "Design Plus",
            UserName = "coded",
            Birthdate = new DateTime(2019, 11, 21)
        };

        var @eventSend = new UserCreatedEvent();

        var subscriptionManager = Mock.Of<ISubscriptionManager>();
        var serviceProvider = Mock.Of<IServiceProvider>();
        var logger = Mock.Of<ILogger<RedisEventBusService>>();
        var options = O.Options.Create(OptionsUtil.RedisEventBusOptions);
        var eventBusOptions = O.Options.Create(OptionsUtil.EventBusOptions);

        var number = (long)new Random().Next(0, int.MaxValue);

        var subscriber = new Mock<ISubscriber>();

        subscriber
            .Setup(x => x.PublishAsync(It.IsAny<RedisChannel>(), It.IsAny<RedisValue>(), It.IsAny<CommandFlags>()))
            .ReturnsAsync(number)
            .Callback<RedisChannel, RedisValue, CommandFlags>((channel, value, commandFlags) =>
            {
                @eventSend = Newtonsoft.Json.JsonConvert.DeserializeObject<UserCreatedEvent>(value!);
            });

        var redisService = new Mock<IRedisService>();
        redisService.SetupGet(x => x.Subscriber).Returns(subscriber.Object);

        var factory = new Mock<IRedisServiceFactory>();
        factory.Setup(x => x.Create(It.IsAny<string>())).Returns(redisService.Object);

        var redisEventBusService = new RedisEventBusService(factory.Object, subscriptionManager, serviceProvider, logger, options, eventBusOptions);

        // Act
        await redisEventBusService.PublishAsync(@event, CancellationToken.None);

        // Assert
        Assert.Equal(@event.Id, @eventSend.Id);
        Assert.Equal(@event.Names, @eventSend.Names);
        Assert.Equal(@event.Lastnames, @eventSend.Lastnames);
        Assert.Equal(@event.UserName, @eventSend.UserName);
        Assert.Equal(@event.Birthdate, @eventSend.Birthdate);
    }

    [Fact]
    public async Task PublishEvent_InvokeHandlerWithQueueEnable_CheckEvent()
    {
        // Arrange
        var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;

        var configuration = ConfigurationUtil.GetConfiguration(new
        {
            EventBus = new {
                EnableQueue = true
            },
            Redis = RedisContainer.RedisOptions("client.pfx", "Temporal1"),
            RedisEventBus = OptionsUtil.RedisEventBusOptions
        });

        var serviceCollection = new ServiceCollection();

        serviceCollection.AddSingleton<IMemoryService, MemoryService>();

        serviceCollection
            .AddLogging()
            .AddRedis(configuration)
            .AddRedisEventBus(configuration)
            .AddEventBus(configuration);

        var serviceProvider = serviceCollection.BuildServiceProvider();

        // Register the all subscribes
        var hostService = serviceProvider.GetRequiredService<IHostedService>();
        await hostService.StartAsync(CancellationToken.None);

        var memory = serviceProvider.GetRequiredService<IMemoryService>();

        var evenBus = serviceProvider.GetRequiredService<IRedisEventBusService>();

        var queue = serviceProvider.GetRequiredService<IQueueService<UserCreatedEventHandler, UserCreatedEvent>>();

        var @event = new UserCreatedEvent()
        {
            Id = 1,
            Names = "Code",
            Lastnames = "Design Plus",
            UserName = "coded",
            Birthdate = new DateTime(2019, 11, 21)
        };

        _ = Task.Factory.StartNew(() =>
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                if (memory.UserEventTrace.Any(x => x.Id == 1))
                {
                    cancellationTokenSource.Cancel();
                }

                Thread.Sleep(1000);
            }
        }, cancellationToken);

        _ = Task.Factory.StartNew(async () =>
        {
            Thread.Sleep(3000);

            await evenBus.PublishAsync(@event, cancellationToken);
        }, cancellationToken);

        // Act
        await queue.DequeueAsync(cancellationToken);

        // Assert
        cancellationToken.Register(() =>
        {
            var userEvent = memory.UserEventTrace.FirstOrDefault();

            Assert.NotEmpty(memory.UserEventTrace);
            Assert.NotNull(userEvent);
            Assert.Equal(@event.Id, userEvent.Id);
            Assert.Equal(@event.UserName, userEvent.UserName);
            Assert.Equal(@event.Names, userEvent.Names);
            Assert.Equal(@event.Birthdate, userEvent.Birthdate);
            Assert.Equal(@event.Lastnames, userEvent.Lastnames);
            Assert.Equal(@event.EventDate.Day, userEvent.EventDate.Day);
            Assert.Equal(@event.EventDate.Month, userEvent.EventDate.Month);
            Assert.Equal(@event.EventDate.Year, userEvent.EventDate.Year);
            Assert.Equal(@event.IdEvent, userEvent.IdEvent);
        });
    }

    
    [Fact]
    public async Task PublishEvent_InvokeHandlerWithQueueDisable_CheckEvent()
    {
        // Arrange
        var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;

        var configuration = ConfigurationUtil.GetConfiguration(new
        {
            EventBus = new {
                EnableQueue = false
            },
            Redis = RedisContainer.RedisOptions("client.pfx", "Temporal1"),
            RedisEventBus = OptionsUtil.RedisEventBusOptions
        });

        var serviceCollection = new ServiceCollection();

        serviceCollection.AddSingleton<IMemoryService, MemoryService>();

        serviceCollection
            .AddLogging()
            .AddRedis(configuration)
            .AddRedisEventBus(configuration)
            .AddEventBus(configuration);

        var serviceProvider = serviceCollection.BuildServiceProvider();

        // Register the all subscribes
        var hostService = serviceProvider.GetRequiredService<IHostedService>();
        await hostService.StartAsync(CancellationToken.None);

        var memory = serviceProvider.GetRequiredService<IMemoryService>();

        var evenBus = serviceProvider.GetRequiredService<IRedisEventBusService>();

        var @event = new UserCreatedEvent()
        {
            Id = 1,
            Names = "Code",
            Lastnames = "Design Plus",
            UserName = "coded",
            Birthdate = new DateTime(2019, 11, 21)
        };

        _ = Task.Factory.StartNew(() =>
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                if (memory.UserEventTrace.Any(x => x.Id == 1))
                {
                    cancellationTokenSource.Cancel();
                }

                Thread.Sleep(1000);
            }
        }, cancellationToken);

        _ = Task.Factory.StartNew(async () =>
        {
            Thread.Sleep(3000);

            await evenBus.PublishAsync(@event, cancellationToken);
        }, cancellationToken);

        // Act

        // Assert
        cancellationToken.Register(() =>
        {
            var userEvent = memory.UserEventTrace.FirstOrDefault();

            Assert.NotEmpty(memory.UserEventTrace);
            Assert.NotNull(userEvent);
            Assert.Equal(@event.Id, userEvent.Id);
            Assert.Equal(@event.UserName, userEvent.UserName);
            Assert.Equal(@event.Names, userEvent.Names);
            Assert.Equal(@event.Birthdate, userEvent.Birthdate);
            Assert.Equal(@event.Lastnames, userEvent.Lastnames);
            Assert.Equal(@event.EventDate.Day, userEvent.EventDate.Day);
            Assert.Equal(@event.EventDate.Month, userEvent.EventDate.Month);
            Assert.Equal(@event.EventDate.Year, userEvent.EventDate.Year);
            Assert.Equal(@event.IdEvent, userEvent.IdEvent);
        });

        await Task.Delay(TimeSpan.FromSeconds(10));
    }

    [Fact]
    public void Unsubscribe_InvokeUnsubscribeRedis_NotListenerChannel()
    {
        // Arrange
        var channelUnsubscribe = string.Empty;
        var isInvokedRemoveSubscription = false;

        var serviceProvider = Mock.Of<IServiceProvider>();
        var logger = Mock.Of<ILogger<RedisEventBusService>>();
        var subscriptionManager = new Mock<ISubscriptionManager>();
        var subscriber = new Mock<ISubscriber>();
        var options = O.Options.Create(OptionsUtil.RedisEventBusOptions);
        var eventBusOptions = O.Options.Create(OptionsUtil.EventBusOptions);

        subscriptionManager
            .Setup(x => x.RemoveSubscription<UserCreatedEvent, UserCreatedEventHandler>())
            .Callback(() =>
            {
                isInvokedRemoveSubscription = true;
            });

        subscriber
            .Setup(x => x.Unsubscribe(It.IsAny<RedisChannel>(), It.IsAny<Action<RedisChannel, RedisValue>>(), It.IsAny<CommandFlags>()))
            .Callback<RedisChannel, Action<RedisChannel, RedisValue>, CommandFlags>((channel, action, commandFlags) =>
            {
                channelUnsubscribe = channel;
            });

        var redisService = new Mock<IRedisService>();
        redisService.SetupGet(x => x.Subscriber).Returns(subscriber.Object);

        var factory = new Mock<IRedisServiceFactory>();
        factory.Setup(x => x.Create(It.IsAny<string>())).Returns(redisService.Object);

        var redisEventBusService = new RedisEventBusService(factory.Object, subscriptionManager.Object, serviceProvider, logger, options, eventBusOptions);

        // Act
        redisEventBusService.Unsubscribe<UserCreatedEvent, UserCreatedEventHandler>();

        // Assert
        Assert.Equal(typeof(UserCreatedEvent).Name, channelUnsubscribe);
        Assert.True(isInvokedRemoveSubscription);
    }
}