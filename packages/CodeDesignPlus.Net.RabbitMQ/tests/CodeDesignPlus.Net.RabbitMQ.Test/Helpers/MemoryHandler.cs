using CodeDesignPlus.Net.Core.Abstractions;

namespace CodeDesignPlus.Net.RabbitMQ.Test.Helpers;

public class MemoryHandler : IMemoryHandler
{
    public Dictionary<Guid, IDomainEvent> Memory { get; set; } = [];
}

public interface IMemoryHandler
{
    Dictionary<Guid, IDomainEvent> Memory { get; set; }
}