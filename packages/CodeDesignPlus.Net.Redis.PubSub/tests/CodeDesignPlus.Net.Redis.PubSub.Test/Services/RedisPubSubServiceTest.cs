using CodeDesignPlus.Net.PubSub.Abstractions;
using CodeDesignPlus.Net.Redis.Abstractions;
using CodeDesignPlus.Net.Redis.PubSub.Extensions;
using CodeDesignPlus.Net.Redis.PubSub.Services;
using CodeDesignPlus.Net.Redis.PubSub.Test.Helpers.Events;
using CodeDesignPlus.Net.Redis.PubSub.Test.Helpers.Memory;
using CodeDesignPlus.Net.Redis.Extensions;
using CodeDesignPlus.Net.PubSub.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using Moq;
using StackExchange.Redis;
using CodeDesignPlus.Net.xUnit.Helpers.Server;
using O = Microsoft.Extensions.Options;

namespace CodeDesignPlus.Net.Redis.PubSub.Test.Services;

public class RedisPubSubServiceTest : IClassFixture<RedisContainer>
{
    private readonly RedisContainer redisContainer;

    public RedisPubSubServiceTest(RedisContainer redisContainer)
    {
        this.redisContainer = redisContainer;
    }

    [Fact]
    public void Constructor_RedisIsNull_ArgumentNullException()
    {
        // Arrange
        var subscriptionManager = Mock.Of<ISubscriptionManager>();
        var serviceProvider = Mock.Of<IServiceProvider>();
        var logger = Mock.Of<ILogger<RedisPubSubService>>();
        var options = O.Options.Create(OptionsUtil.RedisPubSubOptions);
        var PubSubOptions = O.Options.Create(OptionsUtil.PubSubOptions);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new RedisPubSubService(null, subscriptionManager, serviceProvider, logger, options, PubSubOptions));
    }

    [Fact]
    public void Constructor_SubscriptionManagerIsNull_ArgumentNullException()
    {
        // Arrange
        var factory = Mock.Of<IRedisServiceFactory>();
        var serviceProvider = Mock.Of<IServiceProvider>();
        var logger = Mock.Of<ILogger<RedisPubSubService>>();
        var options = O.Options.Create(OptionsUtil.RedisPubSubOptions);
        var PubSubOptions = O.Options.Create(OptionsUtil.PubSubOptions);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new RedisPubSubService(factory, null, serviceProvider, logger, options, PubSubOptions));
    }

    [Fact]
    public void Constructor_ServiceProviderIsNull_ArgumentNullException()
    {
        // Arrange
        var factory = Mock.Of<IRedisServiceFactory>();
        var subscriptionManager = Mock.Of<ISubscriptionManager>();
        var logger = Mock.Of<ILogger<RedisPubSubService>>();
        var options = O.Options.Create(OptionsUtil.RedisPubSubOptions);
        var PubSubOptions = O.Options.Create(OptionsUtil.PubSubOptions);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new RedisPubSubService(factory, subscriptionManager, null, logger, options, PubSubOptions));
    }

    [Fact]
    public void Constructor_LoggerIsNull_ArgumentNullException()
    {
        // Arrange
        var factory = Mock.Of<IRedisServiceFactory>();
        var subscriptionManager = Mock.Of<ISubscriptionManager>();
        var serviceProvider = Mock.Of<IServiceProvider>();
        var options = O.Options.Create(OptionsUtil.RedisPubSubOptions);
        var PubSubOptions = O.Options.Create(OptionsUtil.PubSubOptions);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new RedisPubSubService(factory, subscriptionManager, serviceProvider, null, options, PubSubOptions));
    }

    [Fact]
    public void Constructor_OptionsIsNull_ArgumentNullException()
    {
        // Arrange
        var factory = Mock.Of<IRedisServiceFactory>();
        var subscriptionManager = Mock.Of<ISubscriptionManager>();
        var serviceProvider = Mock.Of<IServiceProvider>();
        var logger = Mock.Of<ILogger<RedisPubSubService>>();
        var PubSubOptions = O.Options.Create(OptionsUtil.PubSubOptions);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new RedisPubSubService(factory, subscriptionManager, serviceProvider, logger, null, PubSubOptions));
    }

    [Fact]
    public void Constructor_OptionsPubSubIsNull_ArgumentNullException()
    {
        // Arrange
        var factory = Mock.Of<IRedisServiceFactory>();
        var subscriptionManager = Mock.Of<ISubscriptionManager>();
        var serviceProvider = Mock.Of<IServiceProvider>();
        var logger = Mock.Of<ILogger<RedisPubSubService>>();
        var options = O.Options.Create(OptionsUtil.RedisPubSubOptions);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new RedisPubSubService(factory, subscriptionManager, serviceProvider, logger, options, null));
    }

    [Fact]
    public void PublishAsync_EventIsNull_ArgumentNullException()
    {
        // Arrange
        var factory = Mock.Of<IRedisServiceFactory>();
        var subscriptionManager = Mock.Of<ISubscriptionManager>();
        var serviceProvider = Mock.Of<IServiceProvider>();
        var logger = Mock.Of<ILogger<RedisPubSubService>>();
        var options = O.Options.Create(OptionsUtil.RedisPubSubOptions);
        var PubSubOptions = O.Options.Create(OptionsUtil.PubSubOptions);

        var redisPubSubService = new RedisPubSubService(factory, subscriptionManager, serviceProvider, logger, options, PubSubOptions);

        // Act & Arc
        Assert.ThrowsAsync<ArgumentNullException>(() => redisPubSubService.PublishAsync(null, CancellationToken.None));
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
        var logger = Mock.Of<ILogger<RedisPubSubService>>();
        var options = O.Options.Create(OptionsUtil.RedisPubSubOptions);
        var PubSubOptions = O.Options.Create(OptionsUtil.PubSubOptions);

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

        var redisPubSubService = new RedisPubSubService(factory.Object, subscriptionManager, serviceProvider, logger, options, PubSubOptions);

        // Act
        await redisPubSubService.PublishAsync(@event, CancellationToken.None);

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
            PubSub = new {
                EnableQueue = true
            },
            Redis = RedisContainer.RedisOptions("client.pfx", "Temporal1"),
            RedisPubSub = OptionsUtil.RedisPubSubOptions
        });

        var serviceCollection = new ServiceCollection();

        serviceCollection.AddSingleton<IMemoryService, MemoryService>();

        serviceCollection
            .AddLogging()
            .AddRedis(configuration)
            .AddRedisPubSub(configuration)
            .AddPubSub(configuration);

        var serviceProvider = serviceCollection.BuildServiceProvider();

        // Register the all subscribes
        var hostService = serviceProvider.GetRequiredService<IHostedService>();
        await hostService.StartAsync(CancellationToken.None);

        var memory = serviceProvider.GetRequiredService<IMemoryService>();

        var evenBus = serviceProvider.GetRequiredService<IRedisPubSubService>();

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
            PubSub = new {
                EnableQueue = false
            },
            Redis = RedisContainer.RedisOptions("client.pfx", "Temporal1"),
            RedisPubSub = OptionsUtil.RedisPubSubOptions
        });

        var serviceCollection = new ServiceCollection();

        serviceCollection.AddSingleton<IMemoryService, MemoryService>();

        serviceCollection
            .AddLogging()
            .AddRedis(configuration)
            .AddRedisPubSub(configuration)
            .AddPubSub(configuration);

        var serviceProvider = serviceCollection.BuildServiceProvider();

        // Register the all subscribes
        var hostService = serviceProvider.GetRequiredService<IHostedService>();
        await hostService.StartAsync(CancellationToken.None);

        var memory = serviceProvider.GetRequiredService<IMemoryService>();

        var evenBus = serviceProvider.GetRequiredService<IRedisPubSubService>();

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
    public async Task Unsubscribe_InvokeUnsubscribeRedis_NotListenerChannel()
    {
        // Arrange
        var channelUnsubscribe = string.Empty;
        var isInvokedRemoveSubscription = false;

        var serviceProvider = Mock.Of<IServiceProvider>();
        var logger = Mock.Of<ILogger<RedisPubSubService>>();
        var subscriptionManager = new Mock<ISubscriptionManager>();
        var subscriber = new Mock<ISubscriber>();
        var options = O.Options.Create(OptionsUtil.RedisPubSubOptions);
        var PubSubOptions = O.Options.Create(OptionsUtil.PubSubOptions);

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

        var redisPubSubService = new RedisPubSubService(factory.Object, subscriptionManager.Object, serviceProvider, logger, options, PubSubOptions);

        // Act
        await redisPubSubService.UnsubscribeAsync<UserCreatedEvent, UserCreatedEventHandler>();

        // Assert
        Assert.Equal(typeof(UserCreatedEvent).Name, channelUnsubscribe);
        Assert.True(isInvokedRemoveSubscription);
    }
}