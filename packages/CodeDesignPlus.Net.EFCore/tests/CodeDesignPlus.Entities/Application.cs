using CodeDesignPlus.Net.Core.Abstractions;
using System;
using System.Collections.Generic;

namespace CodeDesignPlus.Entities
{
    public class Application : IEntity
    {
        public Guid Id { get; set; }
        public Guid Tenant { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }

        public long CreatedAt { get; set; }
        public Guid CreatedBy { get; set; }
        public long? UpdatedAt { get; set; }
        public Guid? UpdatedBy { get; set; }

        public List<AppPermision> AppPermisions { get; set; } = [];
        public List<RolePermission> RolePermisions { get; set; } = [];
    }
}
