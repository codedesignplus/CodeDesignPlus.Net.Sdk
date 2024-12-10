using System;
using CodeDesignPlus.Net.Core.Abstractions;

namespace CodeDesignPlus.Net.Mongo.Sample.Entities;

public class UserEntity : IEntity
{
    public Guid Id { get; set; }
    public bool IsActive { get; set; }
    public long CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }
    public long? UpdatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }

    public required string Name { get; set; }
    public required string Email { get; set; }
    public string? Password { get; set; }

    public List<ProductEntity> Products { get; set; } = [];
}