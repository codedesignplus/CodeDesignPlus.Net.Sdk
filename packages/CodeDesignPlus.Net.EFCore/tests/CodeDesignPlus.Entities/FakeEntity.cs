using CodeDesignPlus.Net.Core.Abstractions;
using System;

namespace CodeDesignPlus.Entities
{
    public class FakeEntity : IEntity, IAuditTrail
    {
        public Guid Id { get; set; }
        public Guid Tenant { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public Guid IdUserCreator { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
