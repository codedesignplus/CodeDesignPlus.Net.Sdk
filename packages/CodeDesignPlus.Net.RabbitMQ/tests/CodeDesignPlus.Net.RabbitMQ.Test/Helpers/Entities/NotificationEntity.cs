using CodeDesignPlus.Net.Core.Abstractions;

namespace CodeDesignPlus.Net.RabbitMQ.Test.Helpers.Entities;

public class NotificationEntity : IEntity
{
    public string? Message { get; set; }
    public string? UserName { get; set; }
    public bool IsActive { get; set; }
    public long CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }
    public long? UpdatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }
    public Guid Tenant { get; set; }

    public Guid Id { get; set; }
}
