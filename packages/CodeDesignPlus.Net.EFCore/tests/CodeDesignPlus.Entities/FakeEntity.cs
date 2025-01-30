using CodeDesignPlus.Net.Core.Abstractions;
using System;

namespace CodeDesignPlus.Entities
{
    public class FakeEntity : IEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public Instant CreatedAt { get; set; }
        public Guid CreatedBy { get; set; }
        public Instant? UpdatedAt { get; set; }
        public Guid? UpdatedBy { get; set; }
        public Guid Tenant { get; set; }
    }
}
