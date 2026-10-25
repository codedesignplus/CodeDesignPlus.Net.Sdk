using CodeDesignPlus.Net.Core.Abstractions.Snapshots;
using CodeDesignPlus.Net.PubSub.Abstractions.Snapshots;
using CodeDesignPlus.Net.PubSub.Extensions;
using CodeDesignPlus.Net.PubSub.Snapshots;
using CodeDesignPlus.Net.PubSub.Test.Helpers.Services;
using CodeDesignPlus.Net.PubSub.Test.Helpers.Snapshots;
using Microsoft.Extensions.Hosting;
using Moq;

namespace CodeDesignPlus.Net.PubSub.Test.Snapshots;

/// <summary>
/// Al arrancar, la copia pide la foto de cada fuente que no figura como sincronizada, y solo cuando ya existen las colas
/// donde le van a llegar las fotos (pendings/380). La que quedó incompleta, o marcada antes de que se contaran las
/// fotos aplicadas, se vuelve a pedir; y mientras no se sincronice, se repite cada intervalo (pendings/485).
/// </summary>
public class SnapshotSyncTest
{
    private static readonly SnapshotKey UserSnapshot = new("xunit-owner", "UserEntity", nameof(UserSnapshotDomainEvent));

    private readonly Mock<ISubscriptionTracker> tracker = new();
    private readonly Mock<ISnapshotRequester> requester = new();
    private readonly Mock<IProjectionSyncStore> store = new();
    private readonly Guid requestId = Guid.NewGuid();
    private readonly ListLogger<SnapshotSyncBackgroundService> logger = new();
    private static readonly Instant RequestedAt = Instant.FromUtc(2026, 10, 14, 12, 0);

    public SnapshotSyncTest()
    {
        this.tracker.SetupGet(x => x.ExpectedCount).Returns(2);
        this.tracker.SetupGet(x => x.ReadyCount).Returns(2);
        this.tracker.SetupGet(x => x.FailedHandlers).Returns([]);

        this.requester
            .Setup(x => x.RequestAsync(typeof(UserSnapshotDomainEvent), null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(this.requestId);
    }

    [Fact]
    public async Task ExecuteAsync_SourceNeverSynced_RequestsTheFullSnapshotAndRemembersIt()
    {
        // Arrange
        var service = this.Service();

        // Act
        await RunAsync(service);

        // Assert
        this.store.Verify(x => x.MarkRequestedAsync(UserSnapshot, this.requestId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_SourceAlreadySynced_DoesNotRequest()
    {
        // Arrange
        this.store
            .Setup(x => x.FindAsync(UserSnapshot, It.IsAny<CancellationToken>()))
            .ReturnsAsync(State(synced: true) with { Applied = 3 });

        var service = this.Service();

        // Act
        await RunAsync(service);

        // Assert
        this.requester.Verify(x => x.RequestAsync(It.IsAny<Type>(), It.IsAny<Guid?>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    /// <summary>
    /// Una marca puesta solo con la cuenta del dueño, antes de que la copia contara lo que aplicó, no prueba nada: así
    /// quedó la foto de usuarios en ms-assemblies con sus 28 fotos descartadas.
    /// </summary>
    [Fact]
    public async Task ExecuteAsync_SourceSyncedWithoutAppliedCount_RequestsAgain()
    {
        // Arrange
        this.store
            .Setup(x => x.FindAsync(UserSnapshot, It.IsAny<CancellationToken>()))
            .ReturnsAsync(State(synced: true));

        var service = this.Service();

        // Act
        await RunAsync(service);

        // Assert
        this.requester.Verify(x => x.RequestAsync(typeof(UserSnapshotDomainEvent), null, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_SourceIncomplete_WarnsAndRequestsAgain()
    {
        // Arrange
        var incompleteRequest = Guid.NewGuid();

        this.store
            .Setup(x => x.FindAsync(UserSnapshot, It.IsAny<CancellationToken>()))
            .ReturnsAsync(State(synced: false) with { IncompleteRequestId = incompleteRequest, IncompleteAt = RequestedAt });

        this.store
            .Setup(x => x.FindProgressAsync(incompleteRequest, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SnapshotProgress(incompleteRequest, 0, 28));

        var service = this.Service();

        // Act
        await RunAsync(service);

        // Assert
        Assert.Equal((true, 1), (
            this.logger.Entries.Any(x => x.Level == LogLevel.Warning && x.Message.Contains("0 of 28")),
            this.requester.Invocations.Count));
    }

    [Fact]
    public async Task ExecuteAsync_SourceStillUnsyncedAfterTheInterval_RequestsAgain()
    {
        // Arrange
        this.store
            .Setup(x => x.FindAsync(UserSnapshot, It.IsAny<CancellationToken>()))
            .ReturnsAsync(State(synced: false));

        var service = this.Service(TimeSpan.FromMilliseconds(50), RequestedAt + Duration.FromHours(2));

        // Act
        await RunAsync(service);

        // Assert
        this.requester.Verify(x => x.RequestAsync(typeof(UserSnapshotDomainEvent), null, It.IsAny<CancellationToken>()), Times.AtLeast(2));
    }

    [Fact]
    public async Task ExecuteAsync_LastRequestStillWithinTheInterval_DoesNotRepeatIt()
    {
        // Arrange
        this.store
            .Setup(x => x.FindAsync(UserSnapshot, It.IsAny<CancellationToken>()))
            .ReturnsAsync(State(synced: false));

        var service = this.Service(TimeSpan.FromSeconds(1), RequestedAt + Duration.FromMilliseconds(10));

        // Act
        await RunAsync(service, TimeSpan.FromMilliseconds(1500));

        // Assert
        this.requester.Verify(x => x.RequestAsync(typeof(UserSnapshotDomainEvent), null, It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// La copia arrancó antes que el dueño y su petición se perdió: con el intervalo de una hora, las primeras esperas
    /// cortas la repiten pronto (pendings/466).
    /// </summary>
    [Fact]
    public async Task ExecuteAsync_OwnerNotUpYet_RetriesAfterTheShortDelaysBeforeTheInterval()
    {
        // Arrange
        this.store
            .Setup(x => x.FindAsync(UserSnapshot, It.IsAny<CancellationToken>()))
            .ReturnsAsync(State(synced: false));

        var service = this.Service(TimeSpan.FromHours(1), RequestedAt + Duration.FromHours(2), [TimeSpan.FromMilliseconds(20), TimeSpan.FromMilliseconds(40)]);

        // Act
        await RunAsync(service);

        // Assert
        this.requester.Verify(x => x.RequestAsync(typeof(UserSnapshotDomainEvent), null, It.IsAny<CancellationToken>()), Times.Exactly(3));
    }

    /// <summary>
    /// Cuando llega el cierre y la fuente queda sincronizada, las repeticiones que faltan ya no piden.
    /// </summary>
    [Fact]
    public async Task ExecuteAsync_SourceSyncedAfterTheFirstRetry_StopsRequesting()
    {
        // Arrange
        var reads = 0;

        this.store
            .Setup(x => x.FindAsync(UserSnapshot, It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => Interlocked.Increment(ref reads) <= 2 ? State(synced: false) : State(synced: true) with { Applied = 3 });

        var service = this.Service(TimeSpan.FromHours(1), RequestedAt + Duration.FromHours(2), [TimeSpan.FromMilliseconds(20), TimeSpan.FromMilliseconds(20), TimeSpan.FromMilliseconds(20), TimeSpan.FromMilliseconds(20)]);

        // Act
        await RunAsync(service);

        // Assert
        this.requester.Verify(x => x.RequestAsync(typeof(UserSnapshotDomainEvent), null, It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    [Fact]
    public async Task ExecuteAsync_RetryDisabled_RequestsOnlyOnStartup()
    {
        // Arrange
        this.store
            .Setup(x => x.FindAsync(UserSnapshot, It.IsAny<CancellationToken>()))
            .ReturnsAsync(State(synced: false));

        var service = this.Service(TimeSpan.Zero, RequestedAt + Duration.FromHours(2));

        // Act
        await RunAsync(service);

        // Assert
        this.requester.Verify(x => x.RequestAsync(typeof(UserSnapshotDomainEvent), null, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_SnapshotHandlerFailedToSubscribe_DoesNotRequest()
    {
        // Arrange
        this.tracker.SetupGet(x => x.ReadyCount).Returns(1);
        this.tracker.SetupGet(x => x.FailedHandlers).Returns([nameof(UserSnapshotHandler)]);

        var service = this.Service();

        // Act
        await RunAsync(service);

        // Assert
        this.requester.Verify(x => x.RequestAsync(It.IsAny<Type>(), It.IsAny<Guid?>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_SubscriptionsNotReady_WaitsBeforeRequesting()
    {
        // Arrange
        this.tracker.SetupGet(x => x.ReadyCount).Returns(1);

        var service = this.Service();

        // Act
        await RunAsync(service);

        // Assert
        this.requester.Verify(x => x.RequestAsync(It.IsAny<Type>(), It.IsAny<Guid?>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public void AddPubSub_ApplicationWithSourceAndSnapshotHandler_RegistersBothSides()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddPubSub(ConfigurationUtil.GetConfiguration());

        // Assert
        var hosted = services.Where(x => x.ServiceType == typeof(IHostedService)).Select(x => x.ImplementationType).ToList();

        Assert.Equal((true, true, true, true), (
            hosted.Contains(typeof(RegisterEventHandlerBackgroundService<SnapshotRequestedHandler, SnapshotRequestedDomainEvent>)),
            hosted.Contains(typeof(RegisterEventHandlerBackgroundService<SnapshotCompletedHandler, SnapshotCompletedDomainEvent>)),
            hosted.Contains(typeof(SnapshotSyncBackgroundService)),
            services.Any(x => x.ServiceType == typeof(ISnapshotSource) && x.ImplementationType == typeof(UserSnapshotSource))));
    }

    [Fact]
    public void GetEventHandlers_AbstractBaseAndOwnHandlers_AreLeftOut()
    {
        // Act
        var handlers = PubSubExtensions.GetEventHandlers();

        // Assert
        Assert.Equal((true, false, false, false), (
            handlers.Contains(typeof(UserSnapshotHandler)),
            handlers.Contains(typeof(SnapshotHandler<>)),
            handlers.Contains(typeof(SnapshotRequestedHandler)),
            handlers.Contains(typeof(SnapshotCompletedHandler))));
    }

    private SnapshotSyncBackgroundService Service(TimeSpan? retryInterval = null, Instant? now = null, TimeSpan[] retryDelays = null)
    {
        var options = new SnapshotSyncOptions();

        options.Snapshots.Add((typeof(UserSnapshotDomainEvent), typeof(UserSnapshotHandler)));

        // Sin esperas crecientes salvo que la prueba las pida: así se prueba el intervalo solo.
        var pubSubOptions = new PubSubOptions { SnapshotRetryDelays = retryDelays ?? [] };

        if (retryInterval.HasValue)
        {
            pubSubOptions.SnapshotRetryInterval = retryInterval.Value;
        }

        return new SnapshotSyncBackgroundService(this.tracker.Object, this.requester.Object, Microsoft.Extensions.Options.Options.Create(options), Microsoft.Extensions.Options.Options.Create(pubSubOptions), this.logger, this.store.Object)
        {
            PollInterval = TimeSpan.FromMilliseconds(10),
            Clock = new FixedClock(now ?? SystemClock.Instance.GetCurrentInstant())
        };
    }

    private static ProjectionSyncState State(bool synced) => new("xunit-owner", "UserEntity", nameof(UserSnapshotDomainEvent), Guid.NewGuid(), RequestedAt, synced ? RequestedAt + Duration.FromMinutes(1) : null, 3);

    private static async Task RunAsync(BackgroundService service, TimeSpan? duration = null)
    {
        await service.StartAsync(CancellationToken.None);
        await Task.Delay(duration ?? TimeSpan.FromMilliseconds(300));
        await service.StopAsync(CancellationToken.None);
    }

    private sealed class FixedClock(Instant now) : IClock
    {
        public Instant GetCurrentInstant() => now;
    }
}
