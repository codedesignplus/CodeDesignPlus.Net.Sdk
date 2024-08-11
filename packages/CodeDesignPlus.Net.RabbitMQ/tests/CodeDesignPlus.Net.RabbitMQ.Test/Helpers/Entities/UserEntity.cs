using CodeDesignPlus.Net.Core.Abstractions;

namespace CodeDesignPlus.Net.RabbitMQ.Test.Helpers.Entities;

public class UserEntity : IEntity
{
    public bool IsActive { get; set; }
    public long CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }
    public long? UpdatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }
    public Guid Tenant { get; set; }

    public string? Name { get; set; }

    public Guid Id { get; set; }
}
