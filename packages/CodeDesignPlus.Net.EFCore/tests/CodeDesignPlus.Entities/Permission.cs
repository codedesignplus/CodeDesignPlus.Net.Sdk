using CodeDesignPlus.Net.Core.Abstractions;
using System;
using System.Collections.Generic;

namespace CodeDesignPlus.Entities
{
    public class Permission : IEntity
    {
        public Guid Id { get; set; }
        public Guid Tenant { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public bool IsActive { get; set; }
        public Instant CreatedAt { get; set; }
        public Guid CreatedBy { get; set; }
        public Instant? UpdatedAt { get; set; }
        public Guid? UpdatedBy { get; set; }
        public Instant? DeletedAt { get; set; }
        public Guid? DeletedBy { get; set; }
        public bool IsDeleted { get; set; }

        public List<AppPermision> AppPermisions { get; set; } = [];
        public List<RolePermission> RolePermisions { get; set; } = [];
    }
}
