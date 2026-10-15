using CodeDesignPlus.Net.Core.Abstractions.Snapshots;
using CodeDesignPlus.Net.PubSub.Abstractions.Snapshots;
using CodeDesignPlus.Net.PubSub.Extensions;
using CodeDesignPlus.Net.PubSub.Snapshots;
using CodeDesignPlus.Net.PubSub.Test.Helpers.Snapshots;
using Microsoft.Extensions.Hosting;
using Moq;

namespace CodeDesignPlus.Net.PubSub.Test.Snapshots;

/// <summary>
/// Al arrancar, la copia pide la foto de cada fuente que no figura como sincronizada, y solo cuando ya existen las colas
/// donde le van a llegar las fotos (pendings/380).
/// </summary>
public class SnapshotSyncTest
{
    private static readonly SnapshotKey UserSnapshot = new("xunit-owner", "UserEntity", nameof(UserSnapshotDomainEvent));

    private readonly Mock<ISubscriptionTracker> tracker = new();
    private readonly Mock<ISnapshotRequester> requester = new();
    private readonly Mock<IProjectionSyncStore> store = new();
    private readonly Guid requestId = Guid.NewGuid();

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
            .ReturnsAsync(new ProjectionSyncState("xunit-owner", "UserEntity", nameof(UserSnapshotDomainEvent), Guid.NewGuid(), Instant.FromUtc(2026, 10, 14, 12, 0), Instant.FromUtc(2026, 10, 14, 12, 1), 3));

        var service = this.Service();

        // Act
        await RunAsync(service);

        // Assert
        this.requester.Verify(x => x.RequestAsync(It.IsAny<Type>(), It.IsAny<Guid?>(), It.IsAny<CancellationToken>()), Times.Never);
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

    private SnapshotSyncBackgroundService Service()
    {
        var options = new SnapshotSyncOptions();

        options.Snapshots.Add((typeof(UserSnapshotDomainEvent), typeof(UserSnapshotHandler)));

        return new SnapshotSyncBackgroundService(this.tracker.Object, this.requester.Object, Microsoft.Extensions.Options.Options.Create(options), Microsoft.Extensions.Logging.Abstractions.NullLogger<SnapshotSyncBackgroundService>.Instance, this.store.Object)
        {
            PollInterval = TimeSpan.FromMilliseconds(10)
        };
    }

    private static async Task RunAsync(BackgroundService service)
    {
        await service.StartAsync(CancellationToken.None);
        await Task.Delay(300);
        await service.StopAsync(CancellationToken.None);
    }
}
