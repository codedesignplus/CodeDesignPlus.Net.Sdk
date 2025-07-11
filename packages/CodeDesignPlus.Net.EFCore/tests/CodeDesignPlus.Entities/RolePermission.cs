using CodeDesignPlus.Net.Core.Abstractions;
using System;

namespace CodeDesignPlus.Entities
{
    public class RolePermission : IEntity
    {
        public Guid Id { get; set; }
        public Guid Tenant { get; set; }
        public Guid IdApplication { get; set; }
        public Guid IdPermission { get; set; }
        public string NameRole { get; set; }
        public bool IsActive { get; set; }

        public Instant CreatedAt { get; set; }
        public Guid CreatedBy { get; set; }
        public Instant? UpdatedAt { get; set; }
        public Guid? UpdatedBy { get; set; }
        public Instant? DeletedAt { get; set; }
        public Guid? DeletedBy { get; set; }
        public bool IsDeleted { get; set; }

        public Application Application { get; set; }
        public Permission Permission { get; set; }
    }
}
