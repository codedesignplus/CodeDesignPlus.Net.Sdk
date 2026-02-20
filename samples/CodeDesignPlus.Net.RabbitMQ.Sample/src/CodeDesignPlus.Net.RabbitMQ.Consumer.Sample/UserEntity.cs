using System;
using CodeDesignPlus.Net.Core.Abstractions;
using NodaTime;

namespace CodeDesignPlus.Net.RabbitMQ.Consumer.Sample;

public class UserEntity : IEntity
{
    public Guid Id { get; set; }
    public bool IsActive { get; set; } 
    public Instant CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }
    public Instant? UpdatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }
    public Instant? DeletedAt { get; set; }
    public Guid? DeletedBy { get; set; }
    public bool IsDeleted { get; set; }

    public required string Name { get; set; }
    public required string Email { get; set; }
    public string? Password { get; set; }
}