using CodeDesignPlus.Net.Core.Abstractions;

namespace CodeDesignPlus.Net.Mongo.Test.Helpers.Models;

public class CounterAggregate(Guid id) : AggregateRoot(id), IVersionedAggregate
{
    public long Version { get; private set; }

    public int Value { get; private set; }

    public List<Guid> CountedIds { get; private set; } = [];

    public static CounterAggregate Create(Guid id, Guid tenant)
    {
        return new CounterAggregate(id)
        {
            Tenant = tenant,
            IsActive = true,
            CreatedAt = SystemClock.Instance.GetCurrentInstant()
        };
    }

    public void Increment()
    {
        Value++;
    }

    public bool Count(Guid id)
    {
        if (CountedIds.Contains(id))
        {
            return false;
        }

        CountedIds.Add(id);
        Value++;

        return true;
    }
}
