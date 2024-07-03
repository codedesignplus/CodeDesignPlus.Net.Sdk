using CodeDesignPlus.Net.Core.Abstractions;

namespace CodeDesignPlus.Net.Kafka.Test.Helpers.Entities;

public class UserEntity : IEntityBase
{
    public Guid Id { get; set; }
}
