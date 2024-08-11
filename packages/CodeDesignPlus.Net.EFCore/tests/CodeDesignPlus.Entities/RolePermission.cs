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

        public long CreatedAt { get; set; }
        public Guid CreatedBy { get; set; }
        public long? UpdatedAt { get; set; }
        public Guid? UpdatedBy { get; set; }

        public Application Application { get; set; }
        public Permission Permission { get; set; }
    }
}
