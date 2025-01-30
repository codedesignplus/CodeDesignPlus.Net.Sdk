using CodeDesignPlus.Net.Core.Abstractions;

namespace CodeDesignPlus.Net.Core.Sample.Resources.Entity;

public class ClientEntity : IEntity
{
    public bool IsActive { get; set; }
    public Instant CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }
    public Instant? UpdatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }
    public Guid Tenant { get; set; }

    public Guid Id { get; set; }
}
