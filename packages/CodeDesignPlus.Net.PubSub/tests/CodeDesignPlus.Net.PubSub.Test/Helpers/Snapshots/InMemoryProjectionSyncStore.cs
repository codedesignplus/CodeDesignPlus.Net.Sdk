using CodeDesignPlus.Net.Core.Abstractions.Snapshots;

namespace CodeDesignPlus.Net.PubSub.Test.Helpers.Snapshots;

/// <summary>
/// Almacén de sincronización en memoria con la misma atomicidad que el de Mongo: cada operación sobre el progreso de una
/// petición escribe y devuelve el resultado de una vez.
/// </summary>
public class InMemoryProjectionSyncStore : IProjectionSyncStore
{
    private readonly object sync = new();
    private readonly Dictionary<SnapshotKey, ProjectionSyncState> states = [];
    private readonly Dictionary<Guid, SnapshotProgress> progress = [];
    private readonly HashSet<Guid> receipts = [];

    public int SyncedMarks { get; private set; }

    public ProjectionSyncState? State(SnapshotKey key)
    {
        lock (this.sync)
        {
            return this.states.GetValueOrDefault(key);
        }
    }

    public void Seed(ProjectionSyncState state)
    {
        lock (this.sync)
        {
            this.states[new SnapshotKey(state.Source, state.Aggregate, state.Snapshot)] = state;
        }
    }

    public Task<ProjectionSyncState?> FindAsync(SnapshotKey key, CancellationToken cancellationToken) => Task.FromResult(this.State(key));

    public Task MarkRequestedAsync(SnapshotKey key, Guid requestId, CancellationToken cancellationToken)
    {
        this.Change(key, x => x with { LastRequestId = requestId, RequestedAt = SystemClock.Instance.GetCurrentInstant() });

        return Task.CompletedTask;
    }

    public Task MarkSyncedAsync(SnapshotKey key, Guid requestId, long count, CancellationToken cancellationToken)
    {
        lock (this.sync)
        {
            this.SyncedMarks++;
        }

        this.Change(key, x => x with { SyncedAt = SystemClock.Instance.GetCurrentInstant(), Count = count, Applied = count, IncompleteRequestId = null, IncompleteAt = null });

        return Task.CompletedTask;
    }

    public Task MarkIncompleteAsync(SnapshotKey key, Guid requestId, CancellationToken cancellationToken)
    {
        this.Change(key, x => x with { IncompleteRequestId = requestId, IncompleteAt = SystemClock.Instance.GetCurrentInstant() });

        return Task.CompletedTask;
    }

    public Task<SnapshotProgress?> RecordAppliedAsync(SnapshotKey key, Guid requestId, Guid eventId, CancellationToken cancellationToken)
    {
        lock (this.sync)
        {
            if (!this.receipts.Add(ProjectionSyncState.ReceiptIdOf(requestId, eventId)))
            {
                return Task.FromResult<SnapshotProgress?>(null);
            }

            var current = this.progress.GetValueOrDefault(requestId) ?? new SnapshotProgress(requestId, 0, null);

            this.progress[requestId] = current with { Applied = current.Applied + 1 };

            return Task.FromResult<SnapshotProgress?>(this.progress[requestId]);
        }
    }

    public Task<SnapshotProgress> RecordCompletedAsync(SnapshotKey key, Guid requestId, long count, CancellationToken cancellationToken)
    {
        lock (this.sync)
        {
            var current = this.progress.GetValueOrDefault(requestId) ?? new SnapshotProgress(requestId, 0, null);

            this.progress[requestId] = current with { Expected = count };

            return Task.FromResult(this.progress[requestId]);
        }
    }

    public Task<SnapshotProgress?> FindProgressAsync(Guid requestId, CancellationToken cancellationToken)
    {
        lock (this.sync)
        {
            return Task.FromResult(this.progress.GetValueOrDefault(requestId));
        }
    }

    private void Change(SnapshotKey key, Func<ProjectionSyncState, ProjectionSyncState> change)
    {
        lock (this.sync)
        {
            var current = this.states.GetValueOrDefault(key) ?? new ProjectionSyncState(key.Source!, key.Aggregate, key.Snapshot, null, null, null, 0);

            this.states[key] = change(current);
        }
    }
}
