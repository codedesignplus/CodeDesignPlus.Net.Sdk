using System.Collections.Concurrent;
using CodeDesignPlus.Net.Core.Abstractions;

namespace CodeDesignPlus.Net.ServiceBus.Test.Helpers;

/// <summary>
/// Records every delivery so a test can assert not only what arrived, but when.
/// </summary>
public class MemoryHandler : IMemoryHandler
{
    private readonly ConcurrentDictionary<Guid, List<DateTime>> deliveries = new();

    public ConcurrentDictionary<Guid, IDomainEvent> Memory { get; } = new();

    public void Record(IDomainEvent domainEvent)
    {
        this.Memory[domainEvent.AggregateId] = domainEvent;

        this.deliveries.AddOrUpdate(
            domainEvent.AggregateId,
            _ => [DateTime.UtcNow],
            (_, list) => { lock (list) { list.Add(DateTime.UtcNow); } return list; });
    }

    public int Attempts(Guid aggregateId)
        => this.deliveries.TryGetValue(aggregateId, out var list) ? list.Count : 0;

    public IReadOnlyList<DateTime> DeliveryTimes(Guid aggregateId)
        => this.deliveries.TryGetValue(aggregateId, out var list) ? [.. list] : [];
}

public interface IMemoryHandler
{
    ConcurrentDictionary<Guid, IDomainEvent> Memory { get; }

    void Record(IDomainEvent domainEvent);

    int Attempts(Guid aggregateId);

    IReadOnlyList<DateTime> DeliveryTimes(Guid aggregateId);
}
