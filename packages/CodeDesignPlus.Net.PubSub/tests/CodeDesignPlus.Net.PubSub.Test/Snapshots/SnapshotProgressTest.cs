using CodeDesignPlus.Net.Core.Abstractions.Snapshots;
using CodeDesignPlus.Net.PubSub.Abstractions.Snapshots;
using CodeDesignPlus.Net.PubSub.Extensions;
using CodeDesignPlus.Net.PubSub.Snapshots;
using CodeDesignPlus.Net.PubSub.Test.Helpers.Snapshots;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace CodeDesignPlus.Net.PubSub.Test.Snapshots;

/// <summary>
/// La copia solo marca la foto como sincronizada cuando aplicó con éxito tantas fotos como anunció el dueño al cerrar,
/// lleguen en el orden que lleguen; si no, la deja incompleta (pendings/485).
/// </summary>
public class SnapshotProgressTest
{
    private const string App = "xunit-copy";

    private static readonly Guid Tenant = Guid.Parse("17daab5e-b090-48bd-969a-a7618eb2a726");
    private static readonly SnapshotKey UserSnapshot = new("xunit-owner", "UserEntity", nameof(UserSnapshotDomainEvent));

    private readonly InMemoryProjectionSyncStore store = new();
    private readonly SnapshotProgressTracker tracker;
    private readonly UserSnapshotHandler handler = new(SnapshotConsumerTest.Core(App));
    private readonly SnapshotCompletedHandler completedHandler;
    private readonly SnapshotRequestedDomainEvent request = SnapshotRequestedDomainEvent.Create(UserSnapshot, App, null);

    public SnapshotProgressTest()
    {
        this.tracker = new SnapshotProgressTracker(NullLogger<SnapshotProgressTracker>.Instance, this.store);
        this.completedHandler = new SnapshotCompletedHandler(SnapshotConsumerTest.Core(App), this.tracker, NullLogger<SnapshotCompletedHandler>.Instance);

        ((ISnapshotProgressReporter)this.handler).ProgressTracker = this.tracker;
    }

    /// <summary>
    /// Una de las tres fotos es de una copropiedad dada de baja: se descarta sin ir a la cola de descartados y cuenta, así
    /// que la petición cuadra y no se vuelve a pedir cada hora (pendings/467).
    /// </summary>
    [Fact]
    public async Task HandleAsync_SnapshotOfAMissingTenantDiscarded_IsSynced()
    {
        // Arrange
        var goneTenant = Guid.NewGuid();
        var snapshots = this.Snapshots(2);
        var filter = new Mock<ISnapshotTenantFilter>();

        snapshots.Add(new UserSnapshotDomainEvent(Guid.NewGuid(), "Gone", goneTenant) { RequestId = this.request.RequestId, RequestedBy = App });
        filter.Setup(x => x.IsMissingAsync(goneTenant, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        ((ISnapshotProgressReporter)this.handler).TenantFilter = filter.Object;

        // Act
        await this.DeliverAsync(snapshots);
        await this.completedHandler.HandleAsync(SnapshotCompletedDomainEvent.Create(this.request, 3), CancellationToken.None);

        // Assert
        Assert.Equal((true, 2), (this.store.State(UserSnapshot)!.IsSynced, this.handler.Applied.Count));
    }

    /// <summary>
    /// Todas las fotos acabaron en la cola de descartados y el cierre anunció las tres: la foto no queda sincronizada (el
    /// caso de ms-assemblies que abrió el plan, con 28 usuarios).
    /// </summary>
    [Fact]
    public async Task HandleAsync_EverySnapshotFails_IsNotSynced()
    {
        // Arrange
        var snapshots = this.Snapshots(3);

        foreach (var snapshot in snapshots)
        {
            this.handler.Failing.Add(snapshot.AggregateId);
        }

        // Act
        await this.DeliverAsync(snapshots);
        await this.completedHandler.HandleAsync(SnapshotCompletedDomainEvent.Create(this.request, 3), CancellationToken.None);

        // Assert
        var state = this.store.State(UserSnapshot);

        Assert.Equal((false, true), (state!.IsSynced, state.IsIncomplete));
    }

    [Fact]
    public async Task HandleAsync_SomeSnapshotsFail_IsIncomplete()
    {
        // Arrange
        var snapshots = this.Snapshots(3);

        this.handler.Failing.Add(snapshots[1].AggregateId);

        // Act
        await this.DeliverAsync(snapshots);
        await this.completedHandler.HandleAsync(SnapshotCompletedDomainEvent.Create(this.request, 3), CancellationToken.None);

        // Assert
        var state = this.store.State(UserSnapshot);
        var progress = await this.store.FindProgressAsync(this.request.RequestId, CancellationToken.None);

        Assert.Equal((false, true, this.request.RequestId, 2L, 3L), (state!.IsSynced, state.IsIncomplete, state.IncompleteRequestId, progress!.Applied, progress.Expected));
    }

    [Fact]
    public async Task HandleAsync_EverySnapshotApplied_IsSynced()
    {
        // Arrange
        var snapshots = this.Snapshots(3);

        // Act
        await this.DeliverAsync(snapshots);
        await this.completedHandler.HandleAsync(SnapshotCompletedDomainEvent.Create(this.request, 3), CancellationToken.None);

        // Assert
        var state = this.store.State(UserSnapshot);

        Assert.Equal((true, 3L, 3L), (state!.IsSynced, state.Count, state.Applied));
    }

    /// <summary>
    /// El cierre llega por otra cola y se adelanta a las fotos: queda incompleta y la última foto la marca.
    /// </summary>
    [Fact]
    public async Task HandleAsync_CompletionArrivesBeforeTheSnapshots_IsSyncedByTheLastOne()
    {
        // Arrange
        var snapshots = this.Snapshots(3);

        await this.completedHandler.HandleAsync(SnapshotCompletedDomainEvent.Create(this.request, 3), CancellationToken.None);

        var incompleteBefore = this.store.State(UserSnapshot)!.IsIncomplete;

        // Act
        await this.DeliverAsync(snapshots);

        // Assert
        var state = this.store.State(UserSnapshot);

        Assert.Equal((true, true, false), (incompleteBefore, state!.IsSynced, state.IsIncomplete));
    }

    [Fact]
    public async Task HandleAsync_OwnerHasNothingToSend_IsSynced()
    {
        // Act
        await this.completedHandler.HandleAsync(SnapshotCompletedDomainEvent.Create(this.request, 0), CancellationToken.None);

        // Assert
        Assert.True(this.store.State(UserSnapshot)!.IsSynced);
    }

    /// <summary>
    /// El bus reentrega una foto ya aplicada: cuenta una vez, y no tapa la que falló.
    /// </summary>
    [Fact]
    public async Task HandleAsync_RedeliveredSnapshot_CountsOnce()
    {
        // Arrange
        var snapshots = this.Snapshots(3);

        this.handler.Failing.Add(snapshots[2].AggregateId);

        // Act
        await this.DeliverAsync([snapshots[0], snapshots[0], snapshots[1], snapshots[2]]);
        await this.completedHandler.HandleAsync(SnapshotCompletedDomainEvent.Create(this.request, 3), CancellationToken.None);

        // Assert
        Assert.False(this.store.State(UserSnapshot)!.IsSynced);
    }

    [Fact]
    public async Task HandleAsync_SnapshotRequestedByAnotherCopy_IsNotCounted()
    {
        // Arrange
        var snapshots = this.Snapshots(1);

        snapshots[0].RequestedBy = "ms-other-copy";

        // Act
        await this.DeliverAsync(snapshots);
        await this.completedHandler.HandleAsync(SnapshotCompletedDomainEvent.Create(this.request, 1), CancellationToken.None);

        // Assert
        Assert.False(this.store.State(UserSnapshot)!.IsSynced);
    }

    /// <summary>
    /// Fotos y cierre a la vez, desde varios hilos como varias réplicas: siempre acaba sincronizada.
    /// </summary>
    [Fact]
    public async Task HandleAsync_SnapshotsAndCompletionConcurrently_AlwaysEndsSynced()
    {
        // Arrange
        var outcomes = new List<bool>();

        for (var round = 0; round < 50; round++)
        {
            var roundStore = new InMemoryProjectionSyncStore();
            var roundTracker = new SnapshotProgressTracker(NullLogger<SnapshotProgressTracker>.Instance, roundStore);
            var roundRequest = SnapshotRequestedDomainEvent.Create(UserSnapshot, App, null);
            var snapshots = Enumerable.Range(0, 20).Select(_ => new UserSnapshotDomainEvent(Guid.NewGuid(), "Ana", Tenant) { RequestId = roundRequest.RequestId, RequestedBy = App }).ToList();

            // Act
            var work = snapshots
                .Select(snapshot => Task.Run(() => roundTracker.RecordAppliedAsync(snapshot, CancellationToken.None)))
                .Append(Task.Run(() => roundTracker.RecordCompletedAsync(SnapshotCompletedDomainEvent.Create(roundRequest, 20), CancellationToken.None)));

            await Task.WhenAll(work);

            outcomes.Add(roundStore.State(UserSnapshot)!.IsSynced);
        }

        // Assert
        Assert.All(outcomes, Assert.True);
    }

    /// <summary>
    /// El consumidor que resuelve el contenedor lleva la cuenta sin que el micro cambie su constructor.
    /// </summary>
    [Fact]
    public async Task AddPubSub_ResolvedSnapshotHandler_CountsWhatItApplies()
    {
        // Arrange
        var services = new ServiceCollection();

        services.AddLogging();
        services.AddPubSub(ConfigurationUtil.GetConfiguration());
        services.AddSingleton<IProjectionSyncStore>(this.store);
        services.AddSingleton(SnapshotConsumerTest.Core(App));

        using var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();

        var resolved = scope.ServiceProvider.GetRequiredService<UserSnapshotHandler>();
        var snapshots = this.Snapshots(2);

        // Act
        foreach (var snapshot in snapshots)
        {
            await resolved.HandleAsync(snapshot, CancellationToken.None);
        }

        // Assert
        var progress = await this.store.FindProgressAsync(this.request.RequestId, CancellationToken.None);

        Assert.Equal(2L, progress?.Applied);
    }

    private List<UserSnapshotDomainEvent> Snapshots(int count) => [.. Enumerable.Range(0, count).Select(i => new UserSnapshotDomainEvent(Guid.NewGuid(), $"User {i}", Tenant) { RequestId = this.request.RequestId, RequestedBy = App })];

    /// <summary>
    /// Entrega cada foto como el bus: si el consumidor lanza, el mensaje acaba en la cola de descartados y se sigue.
    /// </summary>
    private async Task DeliverAsync(IEnumerable<UserSnapshotDomainEvent> snapshots)
    {
        foreach (var snapshot in snapshots)
        {
            try
            {
                await this.handler.HandleAsync(snapshot, CancellationToken.None);
            }
            catch (InvalidOperationException)
            {
                // Descartada.
            }
        }
    }
}
