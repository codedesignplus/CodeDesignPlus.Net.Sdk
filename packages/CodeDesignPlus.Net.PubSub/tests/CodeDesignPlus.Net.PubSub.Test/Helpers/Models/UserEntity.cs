using CodeDesignPlus.Net.Core.Abstractions;

namespace CodeDesignPlus.Net.PubSub.Test.Helpers.Models;

public class UserEntity : IEntity
{
    public bool IsActive { get; set; }
    public Instant CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }
    public Instant? UpdatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }
    public Guid Tenant { get; set; }

    public Guid Id { get; set; }

    public string? Name { get; set; }
}
