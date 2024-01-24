using CodeDesignPlus.Net.Core.Abstractions;
using System;

namespace CodeDesignPlus.Entities
{
    public class AppPermision : IEntity, IAuditTrail
    {
        public Guid Id { get; set; }
        public Guid Tenant { get; set; }
        public Guid IdApplication { get; set; }
        public Guid IdPermission { get; set; }
        public bool IsActive { get; set; }
        public Guid IdUserCreator { get; set; }
        public DateTime CreatedAt { get; set; }

        public Application Application { get; set; }
        public Permission Permission { get; set; }
    }
}
