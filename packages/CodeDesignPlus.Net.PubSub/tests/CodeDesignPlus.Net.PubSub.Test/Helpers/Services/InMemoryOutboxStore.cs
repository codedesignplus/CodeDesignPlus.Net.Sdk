using CodeDesignPlus.Net.Core.Abstractions.Outbox;

namespace CodeDesignPlus.Net.PubSub.Test.Helpers.Services;

/// <summary>
/// Una bandeja de salida en memoria con las mismas reglas que la de Mongo: reclamo con plazo, reintento programado y
/// fallido que no vuelve a salir.
/// </summary>
public sealed class InMemoryOutboxStore : IOutboxStore
{
    private readonly object sync = new();
    private readonly List<Entry> entries = [];

    /// <summary>
    /// Lo que hay en la bandeja.
    /// </summary>
    public IReadOnlyList<Entry> Entries
    {
        get
        {
            lock (this.sync)
            {
                return [.. this.entries];
            }
        }
    }

    /// <summary>
    /// Si se le pide, falla al encolar.
    /// </summary>
    public bool FailOnAdd { get; set; }

    public Task EnsureIndexesAsync(Duration sentRetention, CancellationToken cancellationToken) => Task.CompletedTask;

    public Task AddAsync(IReadOnlyList<OutboxMessage> messages, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (this.FailOnAdd)
        {
            throw new InvalidOperationException("The outbox is down.");
        }

        lock (this.sync)
        {
            foreach (var message in messages.Where(x => this.entries.All(y => y.Message.Id != x.Id)))
            {
                this.entries.Add(new Entry { Message = message, NextAttemptAt = message.CreatedAt });
            }
        }

        return Task.CompletedTask;
    }

    public Task<OutboxMessage?> LeaseNextAsync(string owner, Duration lease, CancellationToken cancellationToken)
    {
        var now = SystemClock.Instance.GetCurrentInstant();

        lock (this.sync)
        {
            var entry = this.entries
                .Where(x => x.Status == "Pending" && x.NextAttemptAt <= now && (x.LockedUntil is null || x.LockedUntil < now))
                .OrderBy(x => x.Message.CreatedAt)
                .FirstOrDefault();

            if (entry is null)
            {
                return Task.FromResult<OutboxMessage?>(null);
            }

            entry.LockedUntil = now + lease;
            entry.LockedBy = owner;

            return Task.FromResult<OutboxMessage?>(entry.Message with { Attempts = entry.Attempts });
        }
    }

    public Task MarkSentAsync(Guid id, CancellationToken cancellationToken)
    {
        lock (this.sync)
        {
            var entry = this.entries.Single(x => x.Message.Id == id);

            entry.Status = "Sent";
            entry.LockedUntil = null;
        }

        return Task.CompletedTask;
    }

    public Task MarkFailedAsync(Guid id, string error, Instant? nextAttemptAt, CancellationToken cancellationToken)
    {
        lock (this.sync)
        {
            var entry = this.entries.Single(x => x.Message.Id == id);

            entry.Attempts++;
            entry.LastError = error;
            entry.LockedUntil = null;

            if (nextAttemptAt.HasValue)
            {
                entry.NextAttemptAt = nextAttemptAt.Value;
            }
            else
            {
                entry.Status = "Failed";
            }
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Un mensaje de la bandeja con su estado.
    /// </summary>
    public sealed class Entry
    {
        public required OutboxMessage Message { get; init; }

        public string Status { get; set; } = "Pending";

        public int Attempts { get; set; }

        public string? LastError { get; set; }

        public Instant NextAttemptAt { get; set; }

        public Instant? LockedUntil { get; set; }

        public string? LockedBy { get; set; }
    }
}
