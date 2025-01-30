using System;
using CodeDesignPlus.Net.Core.Abstractions;

namespace CodeDesignPlus.Net.Mongo.Sample.OperationBase.Entities;

public class ProductEntity : IEntity
{
    public Guid Id { get; set; }
    public bool IsActive { get; set; } 
    public Instant CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }
    public Instant? UpdatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }

    public required string Name { get; set; }
    public string? Description { get; set; }
}