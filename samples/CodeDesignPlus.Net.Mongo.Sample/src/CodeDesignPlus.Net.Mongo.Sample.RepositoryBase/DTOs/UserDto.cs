using CodeDesignPlus.Net.Core.Abstractions;

namespace CodeDesignPlus.Net.Mongo.Sample.RepositoryBase.DTOs;

public class UserDto : IDto
{

    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public List<ProductDto> Products { get; set; } = [];
    public Guid Tenant { get; set; }
    public bool IsActive { get; set; }
    public long CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }
    public long? UpdatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }
}
