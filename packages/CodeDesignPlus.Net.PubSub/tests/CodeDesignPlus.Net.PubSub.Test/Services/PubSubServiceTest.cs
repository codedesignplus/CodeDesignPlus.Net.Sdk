using CodeDesignPlus.Net.Core.Abstractions;
using CodeDesignPlus.Net.Core.Abstractions.Outbox;
using CodeDesignPlus.Net.PubSub.Test.Helpers.Events;
using InMemoryOutboxStore = CodeDesignPlus.Net.PubSub.Test.Helpers.Services.InMemoryOutboxStore;
using CodeDesignPlus.Net.xUnit.Extensions;
using Moq;

namespace CodeDesignPlus.Net.PubSub.Test.Services;

/// <summary>
/// Publicar no depende del token de quien publica, y con la bandeja de salida el evento queda en la base antes de salir
/// (pendings/245).
/// </summary>
public class PubSubTest
{
    private const string Topic = "codedesignplus.xunit-pub-sub.v1.user.created";

    private readonly Mock<IMessage> messageMock = new();
    private readonly Mock<IDomainEventResolver> resolverMock = new();
    private readonly Mock<IOutboxSignal> signalMock = new();
    private readonly Mock<ILogger<PubSubService>> loggerMock = new();
    private readonly Mock<IOutboxTransport> transportMock = new();
    private readonly InMemoryOutboxStore store = new();

    public PubSubTest()
    {
        this.resolverMock.Setup(x => x.GetKeyDomainEvent(It.IsAny<Type>())).Returns(Topic);
    }

    [Fact]
    public void Constructor_MessageIsNull_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new PubSubService(null!, Options(), this.resolverMock.Object, this.signalMock.Object, this.loggerMock.Object));
    }

    [Fact]
    public void Constructor_OptionsIsNull_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new PubSubService(this.messageMock.Object, null!, this.resolverMock.Object, this.signalMock.Object, this.loggerMock.Object));
    }

    [Fact]
    public void Constructor_ResolverIsNull_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new PubSubService(this.messageMock.Object, Options(), null!, this.signalMock.Object, this.loggerMock.Object));
    }

    [Fact]
    public void Constructor_SignalIsNull_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new PubSubService(this.messageMock.Object, Options(), this.resolverMock.Object, null!, this.loggerMock.Object));
    }

    [Fact]
    public void Constructor_LoggerIsNull_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new PubSubService(this.messageMock.Object, Options(), this.resolverMock.Object, this.signalMock.Object, null!));
    }

    [Fact]
    public void Constructor_OutboxEnabledWithoutStore_WarnsThatItPublishesDirectly()
    {
        // Act
        _ = new PubSubService(this.messageMock.Object, Options(), this.resolverMock.Object, this.signalMock.Object, this.loggerMock.Object);

        // Assert
        this.loggerMock.VerifyLogging("The outbox is enabled but there is no IOutboxStore or no IOutboxTransport: events are published directly.", LogLevel.Warning);
    }

    [Fact]
    public async Task PublishAsync_OutboxDisabled_PublishesDirectly()
    {
        // Arrange
        var pubSub = this.Create(outboxEnabled: false);
        var @event = NewEvent();

        // Act
        await pubSub.PublishAsync(@event, CancellationToken.None);

        // Assert
        this.messageMock.Verify(x => x.PublishAsync(@event, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task PublishAsync_CallerCancelledWithoutOutbox_StillPublishes()
    {
        // Arrange: el cliente de la petición se fue después de guardar (pendings/245).
        var pubSub = this.Create(outboxEnabled: false);
        var @event = NewEvent();
        var tokens = new List<CancellationToken>();

        this.messageMock.Setup(x => x.PublishAsync(@event, Capture.In(tokens))).Returns(Task.CompletedTask);

        // Act
        await pubSub.PublishAsync(@event, new CancellationToken(canceled: true));

        // Assert
        Assert.False(Assert.Single(tokens).IsCancellationRequested);
    }

    [Fact]
    public async Task PublishAsync_CallerCancelledWithOutbox_EnqueuesTheEvent()
    {
        // Arrange: el cliente de la petición se fue después de guardar (pendings/245).
        var pubSub = this.Create(outboxEnabled: true);
        var @event = NewEvent();

        // Act
        await pubSub.PublishAsync(@event, new CancellationToken(canceled: true));

        // Assert
        Assert.Equal(@event.EventId, Assert.Single(this.store.Entries).Message.Id);
    }

    [Fact]
    public async Task PublishAsync_WithOutbox_DoesNotSendDirectly()
    {
        // Arrange
        var pubSub = this.Create(outboxEnabled: true);

        // Act
        await pubSub.PublishAsync(NewEvent(), CancellationToken.None);

        // Assert
        this.messageMock.Verify(x => x.PublishAsync(It.IsAny<IDomainEvent>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task PublishAsync_WithOutbox_WakesTheDispatcher()
    {
        // Arrange
        var pubSub = this.Create(outboxEnabled: true);

        // Act
        await pubSub.PublishAsync(NewEvent(), CancellationToken.None);

        // Assert
        this.signalMock.Verify(x => x.Notify(), Times.Once);
    }

    [Fact]
    public async Task PublishAsync_WithOutbox_KeepsTheEventAsItWillTravel()
    {
        // Arrange
        var pubSub = this.Create(outboxEnabled: true);
        var @event = NewEvent();

        // Act
        await pubSub.PublishAsync(@event, CancellationToken.None);

        // Assert
        var message = Assert.Single(this.store.Entries).Message;

        Assert.Equal((Topic, nameof(UserRegisteredEvent), @event.AggregateId, JsonSerializer.Serialize(@event)), (message.Topic, message.EventName, message.AggregateId, message.Body));
    }

    [Fact]
    public async Task PublishAsync_SeveralEvents_EnqueuesThemAll()
    {
        // Arrange
        var pubSub = this.Create(outboxEnabled: true);
        IReadOnlyList<IDomainEvent> events = [NewEvent(), NewEvent(), NewEvent()];

        // Act
        await pubSub.PublishAsync(events, CancellationToken.None);

        // Assert
        Assert.Equal(events.Select(x => x.EventId), this.store.Entries.Select(x => x.Message.Id));
    }

    [Fact]
    public async Task PublishAsync_OutboxFails_PublishesDirectly()
    {
        // Arrange: lo guardado no se queda sin su evento porque la bandeja falle.
        var pubSub = this.Create(outboxEnabled: true);
        var @event = NewEvent();

        this.store.FailOnAdd = true;

        // Act
        await pubSub.PublishAsync(@event, CancellationToken.None);

        // Assert
        this.messageMock.Verify(x => x.PublishAsync(@event, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task PublishAsync_EmptyList_DoesNothing()
    {
        // Arrange
        var pubSub = this.Create(outboxEnabled: true);

        // Act
        await pubSub.PublishAsync([], CancellationToken.None);

        // Assert
        Assert.Empty(this.store.Entries);
    }

    private PubSubService Create(bool outboxEnabled)
    {
        var options = Options();

        options.Value.Outbox.Enable = outboxEnabled;

        return new PubSubService(this.messageMock.Object, options, this.resolverMock.Object, this.signalMock.Object, this.loggerMock.Object, this.store, this.transportMock.Object);
    }

    private static IOptions<PubSubOptions> Options() => Microsoft.Extensions.Options.Options.Create(new PubSubOptions());

    private static UserRegisteredEvent NewEvent() => new(Guid.NewGuid())
    {
        Name = "Code",
        User = "coded",
        Age = 30
    };
}
