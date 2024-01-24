using CodeDesignPlus.Net.Core.Abstractions;
using System;
using System.Collections.Generic;

namespace CodeDesignPlus.Entities
{
    public class Permission : IEntity, IAuditTrail
    {
        public Guid Id { get; set; }
        public Guid Tenant { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public bool IsActive { get; set; }
        public Guid IdUserCreator { get; set; }
        public DateTime CreatedAt { get; set; }


        public List<AppPermision> AppPermisions { get; set; } = [];
        public List<RolePermission> RolePermisions { get; set; } = [];
    }
}
