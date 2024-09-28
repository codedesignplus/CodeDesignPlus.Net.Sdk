namespace CodeDesignPlus.Net.xUnit.Microservice.Test.Helpers;

public class ClientEntity : IEntity
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public bool IsActive { get; set; }
    public long CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }
    public long? UpdatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }
    public Guid Tenant { get; set; }
}
