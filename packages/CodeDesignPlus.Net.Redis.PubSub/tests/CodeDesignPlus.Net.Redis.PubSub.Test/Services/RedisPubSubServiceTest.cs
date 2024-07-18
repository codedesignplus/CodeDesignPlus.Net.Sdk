using CodeDesignPlus.Net.Core.Abstractions;
using CodeDesignPlus.Net.Core.Services;
using CodeDesignPlus.Net.Redis.Abstractions;
using CodeDesignPlus.Net.Redis.PubSub.Extensions;
using CodeDesignPlus.Net.Redis.PubSub.Test.Helpers.Events;
using CodeDesignPlus.Net.Redis.PubSub.Test.Helpers.Memory;
using CodeDesignPlus.Net.xUnit.Helpers.RedisContainer;
using Microsoft.Extensions.Hosting;
using Moq;
using StackExchange.Redis;
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
        var serviceProvider = Mock.Of<IServiceProvider>();
        var logger = Mock.Of<ILogger<RedisPubSubService>>();
        var options = O.Options.Create(OptionsUtil.RedisPubSubOptions);
        var domainEventResolverService = Mock.Of<IDomainEventResolverService>();


        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new RedisPubSubService(null, serviceProvider, logger, domainEventResolverService));
    }

    [Fact]
    public void Constructor_ServiceProviderIsNull_ArgumentNullException()
    {
        // Arrange
        var factory = Mock.Of<IRedisServiceFactory>();
        var logger = Mock.Of<ILogger<RedisPubSubService>>();
        var options = O.Options.Create(OptionsUtil.RedisPubSubOptions);

        var domainEventResolverService = Mock.Of<IDomainEventResolverService>();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new RedisPubSubService(factory, null, logger, domainEventResolverService));
    }

    [Fact]
    public void Constructor_LoggerIsNull_ArgumentNullException()
    {
        // Arrange
        var factory = Mock.Of<IRedisServiceFactory>();
        var serviceProvider = Mock.Of<IServiceProvider>();
        var options = O.Options.Create(OptionsUtil.RedisPubSubOptions);

        var domainEventResolverService = Mock.Of<IDomainEventResolverService>();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new RedisPubSubService(factory, serviceProvider, null, domainEventResolverService));
    }

    [Fact]
    public void Constructor_DomainEventResolverServiceIsNull_ArgumentNullException()
    {
        // Arrange
        var factory = Mock.Of<IRedisServiceFactory>();
        var serviceProvider = Mock.Of<IServiceProvider>();
        var logger = Mock.Of<ILogger<RedisPubSubService>>();
        var options = O.Options.Create(OptionsUtil.RedisPubSubOptions);


        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new RedisPubSubService(factory, serviceProvider, logger, null));
    }

    [Fact]
    public async Task PublishAsync_EventIsNull_ArgumentNullException()
    {
        // Arrange
        var factory = Mock.Of<IRedisServiceFactory>();
        var serviceProvider = Mock.Of<IServiceProvider>();
        var logger = Mock.Of<ILogger<RedisPubSubService>>();

        var domainEventResolverService = Mock.Of<IDomainEventResolverService>();

        var redisPubSubService = new RedisPubSubService(factory, serviceProvider, logger, domainEventResolverService);

        // Act & Arc
        await Assert.ThrowsAsync<ArgumentNullException>(() => redisPubSubService.PublishAsync((IDomainEvent)null!, CancellationToken.None));
    }

    [Fact]
    public async Task PublishAsync_InvokePublish_Succeses()
    {
        // Arrange
        var @event = new UserCreatedEvent(Guid.NewGuid())
        {
            Names = "Code",
            Lastnames = "Design Plus",
            UserName = "coded",
            Birthdate = new DateTime(2019, 11, 21)
        };

        var @eventSend = new UserCreatedEvent(Guid.NewGuid());

        var coreOptions = O.Options.Create(OptionsUtil.CoreOptions);
        var serviceProvider = Mock.Of<IServiceProvider>();
        var logger = Mock.Of<ILogger<RedisPubSubService>>();
        var options = O.Options.Create(OptionsUtil.RedisPubSubOptions);

        var domainEventResolverService = new DomainEventResolverService(coreOptions);

        var number = (long)new Random().Next(0, int.MaxValue);

        var subscriber = new Mock<ISubscriber>();

        subscriber
            .Setup(x => x.PublishAsync(It.IsAny<RedisChannel>(), It.IsAny<RedisValue>(), It.IsAny<CommandFlags>()))
            .ReturnsAsync(number)
            .Callback<RedisChannel, RedisValue, CommandFlags>((channel, value, commandFlags) =>
            {
                @eventSend = JsonSerializer.Deserialize<UserCreatedEvent>(value!);
            });

        var redisService = new Mock<IRedisService>();
        redisService.SetupGet(x => x.Subscriber).Returns(subscriber.Object);

        var factory = new Mock<IRedisServiceFactory>();
        factory.Setup(x => x.Create(It.IsAny<string>())).Returns(redisService.Object);

        var redisPubSubService = new RedisPubSubService(factory.Object, serviceProvider, logger, domainEventResolverService);

        // Act
        await redisPubSubService.PublishAsync(@event, CancellationToken.None);

        // Assert
        Assert.Equal(@event.Names, @eventSend.Names);
        Assert.Equal(@event.Lastnames, @eventSend.Lastnames);
        Assert.Equal(@event.UserName, @eventSend.UserName);
        Assert.Equal(@event.Birthdate, @eventSend.Birthdate);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task PublishEvent_InvokeHandler_CheckEvent(bool useQueue)
    {
        // Arrange
        var @event = new UserCreatedEvent(Guid.NewGuid())
        {
            Names = "Code",
            Lastnames = "Design Plus",
            UserName = "coded",
            Birthdate = new DateTime(2019, 11, 21)
        };

        var configuration = ConfigurationUtil.GetConfiguration(new
        {
            Core = OptionsUtil.CoreOptions,
            Redis = redisContainer.RedisOptions("client.pfx", "Temporal1", "Core"),
            RedisPubSub = OptionsUtil.RedisPubSubOptions(useQueue)
        });

        var serviceCollection = new ServiceCollection();

        serviceCollection
            .AddSingleton<IMemoryService, MemoryService>()
            .AddLogging()
            .AddSingleton(configuration)
            .AddRedisPubSub(configuration);

        var serviceProvider = serviceCollection.BuildServiceProvider();

        var hostServices = serviceProvider.GetServices<IHostedService>();
        var memory = serviceProvider.GetRequiredService<IMemoryService>();
        var evenBus = serviceProvider.GetRequiredService<IRedisPubSubService>();

        foreach (var hostService in hostServices)
            await hostService.StartAsync(CancellationToken.None);

        await evenBus.PublishAsync([@event], CancellationToken.None);

        while (!memory.UserEventTrace.Any(x => x.EventId == @event.EventId))
        {
            await Task.Delay(1000);
        }

        // Assert
        var userEvent = memory.UserEventTrace.FirstOrDefault();

        Assert.NotEmpty(memory.UserEventTrace);
        Assert.NotNull(userEvent);
        Assert.Equal(@event.UserName, userEvent.UserName);
        Assert.Equal(@event.Names, userEvent.Names);
        Assert.Equal(@event.Birthdate, userEvent.Birthdate);
        Assert.Equal(@event.Lastnames, userEvent.Lastnames);
        Assert.Equal(@event.OccurredAt, userEvent.OccurredAt);
        Assert.Equal(@event.EventId, userEvent.EventId);
    }

    [Fact]
    public async Task Unsubscribe_InvokeUnsubscribeRedis_NotListenerChannel()
    {
        // Arrange

        var channelUnsubscribe = string.Empty;

        var coreOptions = O.Options.Create(OptionsUtil.CoreOptions);
        var serviceProvider = Mock.Of<IServiceProvider>();
        var logger = Mock.Of<ILogger<RedisPubSubService>>();
        var subscriber = new Mock<ISubscriber>();

        var domainEventResolverService = new DomainEventResolverService(coreOptions);
        var eventKey = domainEventResolverService.GetKeyDomainEvent<UserCreatedEvent>();

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

        var redisPubSubService = new RedisPubSubService(factory.Object, serviceProvider, logger, domainEventResolverService);

        // Act
        await redisPubSubService.UnsubscribeAsync<UserCreatedEvent, UserCreatedEventHandler>(CancellationToken.None);

        // Assert
        Assert.Equal(eventKey, channelUnsubscribe);
    }
}