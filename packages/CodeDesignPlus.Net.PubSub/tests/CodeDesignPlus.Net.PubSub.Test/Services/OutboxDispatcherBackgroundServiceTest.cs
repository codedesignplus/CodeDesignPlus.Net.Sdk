using CodeDesignPlus.Net.Core.Abstractions;
using CodeDesignPlus.Net.Core.Abstractions.Outbox;
using CodeDesignPlus.Net.PubSub.Test.Helpers.Events;
using CodeDesignPlus.Net.PubSub.Test.Helpers.Services;
using PubSubService = CodeDesignPlus.Net.PubSub.Services.PubSubService;
using Moq;

namespace CodeDesignPlus.Net.PubSub.Test.Services;

/// <summary>
/// El despachador envía lo encolado, reintenta lo que falla con espera creciente y deja a la vista lo que agota sus
/// intentos, sin descartarlo (pendings/245).
/// </summary>
public class OutboxDispatcherBackgroundServiceTest
{
    private readonly InMemoryOutboxStore store = new();
    private readonly Mock<IOutboxTransport> transportMock = new();
    private readonly ListLogger<OutboxDispatcherBackgroundService> logger = new();
    private readonly PubSubOptions options = new();

    [Fact]
    public async Task DispatchPendingAsync_PendingMessage_SendsItWithTheSameIdAndBody()
    {
        // Arrange
        var message = NewMessage();
        var sent = new List<OutboxMessage>();

        await this.store.AddAsync([message], CancellationToken.None);

        this.transportMock.Setup(x => x.SendAsync(Capture.In(sent), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        // Act
        await this.Create().DispatchPendingAsync(CancellationToken.None);

        // Assert
        Assert.Equal((message.Id, message.Body), (Assert.Single(sent).Id, sent[0].Body));
    }

    [Fact]
    public async Task DispatchPendingAsync_Sent_MarksItAsSent()
    {
        // Arrange
        await this.store.AddAsync([NewMessage()], CancellationToken.None);

        // Act
        await this.Create().DispatchPendingAsync(CancellationToken.None);

        // Assert
        Assert.Equal("Sent", Assert.Single(this.store.Entries).Status);
    }

    [Fact]
    public async Task DispatchPendingAsync_TransportFails_KeepsItPendingForARetry()
    {
        // Arrange
        await this.store.AddAsync([NewMessage()], CancellationToken.None);

        this.transportMock.Setup(x => x.SendAsync(It.IsAny<OutboxMessage>(), It.IsAny<CancellationToken>())).ThrowsAsync(new InvalidOperationException("Service Bus is down."));

        var before = SystemClock.Instance.GetCurrentInstant();

        // Act
        await this.Create().DispatchPendingAsync(CancellationToken.None);

        // Assert
        var entry = Assert.Single(this.store.Entries);

        Assert.Equal(("Pending", 1, "Service Bus is down.", true), (entry.Status, entry.Attempts, entry.LastError, entry.NextAttemptAt >= before + Duration.FromSeconds(this.options.Outbox.InitialRetryDelaySeconds)));
    }

    [Fact]
    public async Task DispatchPendingAsync_LastAttemptFails_LeavesItFailedAndVisible()
    {
        // Arrange
        this.options.Outbox.MaxAttempts = 1;

        await this.store.AddAsync([NewMessage()], CancellationToken.None);

        this.transportMock.Setup(x => x.SendAsync(It.IsAny<OutboxMessage>(), It.IsAny<CancellationToken>())).ThrowsAsync(new InvalidOperationException("Service Bus is down."));

        // Act
        await this.Create().DispatchPendingAsync(CancellationToken.None);

        // Assert
        Assert.Equal("Failed", Assert.Single(this.store.Entries).Status);
    }

    [Fact]
    public async Task DispatchPendingAsync_LastAttemptFails_LogsTheStableAlertMessage()
    {
        // Arrange
        this.options.Outbox.MaxAttempts = 1;

        var message = NewMessage();

        await this.store.AddAsync([message], CancellationToken.None);

        this.transportMock.Setup(x => x.SendAsync(It.IsAny<OutboxMessage>(), It.IsAny<CancellationToken>())).ThrowsAsync(new InvalidOperationException("Service Bus is down."));

        // Act
        await this.Create().DispatchPendingAsync(CancellationToken.None);

        // Assert
        Assert.Contains((LogLevel.Error, $"Outbox message exhausted: {message.EventName} {message.Id} of aggregate {message.AggregateId} to {message.Topic} failed 1 attempts and will not be retried."), this.logger.Entries);
    }

    [Fact]
    public async Task DispatchPendingAsync_FailedMessage_IsNotSentAgain()
    {
        // Arrange
        this.options.Outbox.MaxAttempts = 1;

        await this.store.AddAsync([NewMessage()], CancellationToken.None);

        this.transportMock.Setup(x => x.SendAsync(It.IsAny<OutboxMessage>(), It.IsAny<CancellationToken>())).ThrowsAsync(new InvalidOperationException("Service Bus is down."));

        var dispatcher = this.Create();

        await dispatcher.DispatchPendingAsync(CancellationToken.None);

        // Act
        var count = await dispatcher.DispatchPendingAsync(CancellationToken.None);

        // Assert
        Assert.Equal(0, count);
    }

    [Fact]
    public async Task DispatchPendingAsync_TwoDispatchers_SendEachMessageOnce()
    {
        // Arrange
        var messages = Enumerable.Range(0, 50).Select(_ => NewMessage()).ToList();
        var sent = new System.Collections.Concurrent.ConcurrentBag<Guid>();

        await this.store.AddAsync(messages, CancellationToken.None);

        this.transportMock
            .Setup(x => x.SendAsync(It.IsAny<OutboxMessage>(), It.IsAny<CancellationToken>()))
            .Returns<OutboxMessage, CancellationToken>(async (message, _) =>
            {
                await Task.Yield();

                sent.Add(message.Id);
            });

        // Act
        await Task.WhenAll(this.Create().DispatchPendingAsync(CancellationToken.None), this.Create().DispatchPendingAsync(CancellationToken.None));

        // Assert
        Assert.Equal(messages.Select(x => x.Id).Order(), sent.Order());
    }

    [Theory]
    [InlineData(1, 5)]
    [InlineData(2, 10)]
    [InlineData(3, 20)]
    [InlineData(8, 600)]
    [InlineData(19, 600)]
    public void RetryDelay_Attempts_DoublesUpToTheMaximum(int attempts, int expectedSeconds)
    {
        // Act
        var delay = OutboxDispatcherBackgroundService.RetryDelay(attempts, new OutboxOptions());

        // Assert
        Assert.Equal(Duration.FromSeconds(expectedSeconds), delay);
    }

    [Fact]
    public async Task StartAsync_WithoutStore_WarnsAndDoesNotRun()
    {
        // Arrange
        var dispatcher = new OutboxDispatcherBackgroundService(new OutboxSignal(), Microsoft.Extensions.Options.Options.Create(this.options), this.logger, null, this.transportMock.Object);

        // Act
        await dispatcher.StartAsync(CancellationToken.None);
        await dispatcher.StopAsync(CancellationToken.None);

        // Assert
        Assert.Contains((LogLevel.Warning, "The outbox dispatcher is not running: there is no IOutboxStore or no IOutboxTransport."), this.logger.Entries);
    }

    [Fact]
    public async Task PublishThenDispatch_CallerCancelledAfterSaving_TheEventStillLeaves()
    {
        // Arrange: la petición se cortó entre guardar y publicar, como el contador del 242 (pendings/245).
        var resolver = new Mock<IDomainEventResolver>();
        var signal = new OutboxSignal();
        var sent = new List<OutboxMessage>();
        var @event = new UserRegisteredEvent(Guid.NewGuid()) { Name = "Code", User = "coded" };

        resolver.Setup(x => x.GetKeyDomainEvent(It.IsAny<Type>())).Returns("codedesignplus.xunit-pub-sub.v1.user.created");

        this.transportMock.Setup(x => x.SendAsync(Capture.In(sent), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var pubSub = new PubSubService(Mock.Of<IMessage>(), Microsoft.Extensions.Options.Options.Create(this.options), resolver.Object, signal, Mock.Of<ILogger<PubSubService>>(), this.store, this.transportMock.Object);

        using var dispatcher = new OutboxDispatcherBackgroundService(signal, Microsoft.Extensions.Options.Options.Create(this.options), this.logger, this.store, this.transportMock.Object);

        await dispatcher.StartAsync(CancellationToken.None);

        // Act
        await pubSub.PublishAsync(@event, new CancellationToken(canceled: true));

        await WaitUntilAsync(() => this.store.Entries.All(x => x.Status == "Sent"));

        await dispatcher.StopAsync(CancellationToken.None);

        // Assert
        Assert.Equal(@event.EventId, Assert.Single(sent).Id);
    }

    private OutboxDispatcherBackgroundService Create()
    {
        return new OutboxDispatcherBackgroundService(new OutboxSignal(), Microsoft.Extensions.Options.Options.Create(this.options), this.logger, this.store, this.transportMock.Object);
    }

    private static OutboxMessage NewMessage()
    {
        return new OutboxMessage(Guid.NewGuid(), "codedesignplus.xunit-pub-sub.v1.user.created", nameof(UserRegisteredEvent), Guid.NewGuid(), "{}", [], SystemClock.Instance.GetCurrentInstant(), 0);
    }

    private static async Task WaitUntilAsync(Func<bool> condition)
    {
        var deadline = DateTime.UtcNow.AddSeconds(10);

        while (!condition() && DateTime.UtcNow < deadline)
        {
            await Task.Delay(20);
        }
    }
}
