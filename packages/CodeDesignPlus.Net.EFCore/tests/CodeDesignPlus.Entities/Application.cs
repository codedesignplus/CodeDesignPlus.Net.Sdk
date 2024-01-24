using System;
using System.Collections.Generic;
using CodeDesignPlus.Net.Core.Abstractions;

namespace CodeDesignPlus.Entities
{
    public class Application : IEntity, IAuditTrail
    {
        public Guid Id { get; set; }
        public Guid Tenant { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public Guid IdUserCreator { get; set; }
        public DateTime CreatedAt { get; set; }

        public List<AppPermision> AppPermisions { get; set; } = [];
        public List<RolePermission> RolePermisions { get; set; } = [];
    }
}
