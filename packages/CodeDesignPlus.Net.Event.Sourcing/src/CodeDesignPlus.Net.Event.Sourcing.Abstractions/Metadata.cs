namespace CodeDesignPlus.Net.Event.Sourcing.Abstractions;

public class Metadata<TUserKey>
{
    public Guid AggregateId { get; private set; }
    public long Version { get; private set; }
    public TUserKey UserId { get; private set; }
    public string Category { get; private set; }

    public Metadata(Guid aggregateId, long version, TUserKey userId, string category)
    {
        this.AggregateId = aggregateId;
        this.Version = version;
        this.UserId = userId;
        this.Category = category;
    }
}