using CodeDesignPlus.Net.Core.Abstractions.Options;
using CodeDesignPlus.Net.Core.Abstractions.Snapshots;
using CodeDesignPlus.Net.PubSub.Abstractions.Snapshots;
using CodeDesignPlus.Net.PubSub.Extensions;
using CodeDesignPlus.Net.PubSub.Snapshots;
using CodeDesignPlus.Net.PubSub.Test.Helpers.Snapshots;
using Moq;

namespace CodeDesignPlus.Net.PubSub.Test.Snapshots;

/// <summary>
/// La copia aplica solo la foto que pidió, marca la fuente como sincronizada cuando termina su foto completa y la foto
/// sobrevive al bus con a quién va dirigida (pendings/380).
/// </summary>
public class SnapshotConsumerTest
{
    private static readonly Guid Tenant = Guid.Parse("17daab5e-b090-48bd-969a-a7618eb2a726");
    private static readonly SnapshotKey Assemblies = new("ms-assemblies", "AssemblyAggregate", "AssemblySnapshotDomainEvent");

    private readonly Mock<IProjectionSyncStore> store = new();

    [Fact]
    public async Task HandleAsync_SnapshotRequestedByThisCopy_IsApplied()
    {
        // Arrange
        var handler = new UserSnapshotHandler(Core("xunit-copy"));
        var snapshot = new UserSnapshotDomainEvent(Guid.NewGuid(), "Ana", Tenant) { RequestedBy = "xunit-copy" };

        // Act
        await handler.HandleAsync(snapshot, CancellationToken.None);

        // Assert
        Assert.Single(handler.Applied);
    }

    [Fact]
    public async Task HandleAsync_SnapshotRequestedByAnotherCopy_IsIgnored()
    {
        // Arrange
        var handler = new UserSnapshotHandler(Core("xunit-copy"));
        var snapshot = new UserSnapshotDomainEvent(Guid.NewGuid(), "Ana", Tenant) { RequestedBy = "ms-other-copy" };

        // Act
        await handler.HandleAsync(snapshot, CancellationToken.None);

        // Assert
        Assert.Empty(handler.Applied);
    }

    /// <summary>
    /// La foto de una copropiedad dada de baja no se aplica (pendings/467).
    /// </summary>
    [Fact]
    public async Task HandleAsync_TenantNoLongerExists_IsNotApplied()
    {
        // Arrange
        var filter = new Mock<ISnapshotTenantFilter>();
        var handler = new UserSnapshotHandler(Core("xunit-copy"));
        var snapshot = new UserSnapshotDomainEvent(Guid.NewGuid(), "Ana", Tenant) { RequestedBy = "xunit-copy" };

        filter.Setup(x => x.IsMissingAsync(Tenant, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        ((ISnapshotProgressReporter)handler).TenantFilter = filter.Object;

        // Act
        await handler.HandleAsync(snapshot, CancellationToken.None);

        // Assert
        Assert.Empty(handler.Applied);
    }

    [Fact]
    public async Task HandleAsync_TenantExists_IsApplied()
    {
        // Arrange
        var filter = new Mock<ISnapshotTenantFilter>();
        var handler = new UserSnapshotHandler(Core("xunit-copy"));
        var snapshot = new UserSnapshotDomainEvent(Guid.NewGuid(), "Ana", Tenant) { RequestedBy = "xunit-copy" };

        filter.Setup(x => x.IsMissingAsync(Tenant, It.IsAny<CancellationToken>())).ReturnsAsync(false);

        ((ISnapshotProgressReporter)handler).TenantFilter = filter.Object;

        // Act
        await handler.HandleAsync(snapshot, CancellationToken.None);

        // Assert
        Assert.Single(handler.Applied);
    }

    [Fact]
    public async Task AddPubSub_SnapshotHandlerResolved_ReceivesTheTenantFilter()
    {
        // Arrange
        var filter = new Mock<ISnapshotTenantFilter>();
        var services = new ServiceCollection();

        services.AddLogging();
        services.AddPubSub(ConfigurationUtil.GetConfiguration());
        services.AddSingleton(Core("xunit-pub-sub"));
        services.AddSingleton(filter.Object);

        using var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();

        var snapshot = new UserSnapshotDomainEvent(Guid.NewGuid(), "Ana", Tenant);

        filter.Setup(x => x.IsMissingAsync(Tenant, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        // Act
        var handler = scope.ServiceProvider.GetRequiredService<UserSnapshotHandler>();

        await handler.HandleAsync(snapshot, CancellationToken.None);

        // Assert
        Assert.Empty(handler.Applied);
    }

    [Fact]
    public void Deserialize_SnapshotFromTheBus_KeepsTenantRequestAndRequester()
    {
        // Arrange
        var requestId = Guid.NewGuid();
        var snapshot = new UserSnapshotDomainEvent(Guid.NewGuid(), "Ana", Tenant) { RequestId = requestId, RequestedBy = "xunit-copy" };

        // Act
        var read = JsonSerializer.Deserialize<UserSnapshotDomainEvent>(JsonSerializer.Serialize(snapshot));

        // Assert
        Assert.Equal((Tenant, requestId, "xunit-copy", "Ana", snapshot.OccurredAt), (read.Tenant, read.RequestId, read.RequestedBy, read.Name, read.OccurredAt));
    }

    [Fact]
    public void Deserialize_RequestFromTheBus_KeepsEveryField()
    {
        // Arrange
        var request = SnapshotRequestedDomainEvent.Create(Assemblies, "ms-communitylife", Tenant);

        // Act
        var read = JsonSerializer.Deserialize<SnapshotRequestedDomainEvent>(JsonSerializer.Serialize(request));

        // Assert
        Assert.Equal((request.Key(), request.RequestId, request.RequestedBy, request.Tenant), (read.Key(), read.RequestId, read.RequestedBy, read.Tenant));
    }

    [Fact]
    public void Deserialize_CompletionFromTheBus_KeepsEveryField()
    {
        // Arrange
        var completed = SnapshotCompletedDomainEvent.Create(SnapshotRequestedDomainEvent.Create(Assemblies, "ms-communitylife", null), 3);

        // Act
        var read = JsonSerializer.Deserialize<SnapshotCompletedDomainEvent>(JsonSerializer.Serialize(completed));

        // Assert
        Assert.Equal((completed.Key(), completed.RequestId, completed.RequestedBy, completed.Tenant, completed.Count), (read.Key(), read.RequestId, read.RequestedBy, read.Tenant, read.Count));
    }

    [Theory]
    [InlineData(null, true)]
    [InlineData(-60, true)]
    [InlineData(60, false)]
    public void Supersedes_LastAppliedChange_OnlyWhenTheSnapshotIsNotOlder(int? secondsFromSnapshot, bool expected)
    {
        // Arrange
        var takenAt = Instant.FromUtc(2026, 10, 14, 12, 0);
        var snapshot = new UserSnapshotDomainEvent(Guid.NewGuid(), "Ana", Tenant, occurredAt: takenAt);
        Instant? lastApplied = secondsFromSnapshot is null ? null : takenAt + Duration.FromSeconds(secondsFromSnapshot.Value);

        // Act
        var supersedes = snapshot.Supersedes(lastApplied);

        // Assert
        Assert.Equal(expected, supersedes);
    }

    [Fact]
    public async Task HandleAsync_FullSnapshotOfThisCopyCompletedAndApplied_MarksTheSourceSynced()
    {
        // Arrange
        var request = SnapshotRequestedDomainEvent.Create(Assemblies, "xunit-copy", null);
        var handler = this.CompletedHandler();

        this.store
            .Setup(x => x.RecordCompletedAsync(Assemblies, request.RequestId, 3, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SnapshotProgress(request.RequestId, 3, 3));

        // Act
        await handler.HandleAsync(SnapshotCompletedDomainEvent.Create(request, 3), CancellationToken.None);

        // Assert
        this.store.Verify(x => x.MarkSyncedAsync(Assemblies, request.RequestId, 3, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [InlineData("ms-other-copy", false)]
    [InlineData("xunit-copy", true)]
    public async Task HandleAsync_CompletionOfAnotherCopyOrOfOneTenant_DoesNotMarkTheSource(string requestedBy, bool oneTenant)
    {
        // Arrange
        var request = SnapshotRequestedDomainEvent.Create(Assemblies, requestedBy, oneTenant ? Tenant : null);
        var handler = this.CompletedHandler();

        // Act
        await handler.HandleAsync(SnapshotCompletedDomainEvent.Create(request, 3), CancellationToken.None);

        // Assert
        this.store.Verify(x => x.RecordCompletedAsync(It.IsAny<SnapshotKey>(), It.IsAny<Guid>(), It.IsAny<long>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    private SnapshotCompletedHandler CompletedHandler()
    {
        var tracker = new SnapshotProgressTracker(Microsoft.Extensions.Logging.Abstractions.NullLogger<SnapshotProgressTracker>.Instance, this.store.Object);

        return new SnapshotCompletedHandler(Core("xunit-copy"), tracker, Microsoft.Extensions.Logging.Abstractions.NullLogger<SnapshotCompletedHandler>.Instance);
    }

    internal static IOptions<CoreOptions> Core(string appName) => Microsoft.Extensions.Options.Options.Create(new CoreOptions
    {
        AppName = appName,
        Business = "CodeDesignPlus",
        Version = "v1",
        Description = "Snapshot tests",
        Contact = new() { Name = "CodeDesignPlus", Email = "wliscano@codedesignplus.com" }
    });
}
