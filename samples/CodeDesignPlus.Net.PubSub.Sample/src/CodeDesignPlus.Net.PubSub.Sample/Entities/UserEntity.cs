using System;
using CodeDesignPlus.Net.Core.Abstractions;

namespace CodeDesignPlus.Net.PubSub.Sample.Entities;

public class UserEntity : IEntity
{
    public bool IsActive { get; set; }
    public Instant CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }
    public Instant? UpdatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }
    public required string Name { get; set; }

    public Guid Id { get; set; }
}
