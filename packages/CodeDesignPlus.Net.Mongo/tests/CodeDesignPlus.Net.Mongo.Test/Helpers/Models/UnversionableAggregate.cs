using CodeDesignPlus.Net.Core.Abstractions;

namespace CodeDesignPlus.Net.Mongo.Test.Helpers.Models;

public class UnversionableAggregate(Guid id) : AggregateRoot(id), IVersionedAggregate
{
    public long Version => 0;
}
