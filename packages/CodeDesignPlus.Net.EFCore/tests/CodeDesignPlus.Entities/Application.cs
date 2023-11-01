using System;
using System.Collections.Generic;
using CodeDesignPlus.Net.Core.Abstractions;

namespace CodeDesignPlus.Entities
{
    public class Application : IEntityLong<int>
    {
        public Application()
        {
            this.AppPermisions = new List<AppPermision>();
            this.RolePermisions = new List<RolePermission>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public int IdUserCreator { get; set; }
        public DateTime DateCreated { get; set; }

        public List<AppPermision> AppPermisions { get; set; }
        public List<RolePermission> RolePermisions { get; set; }
    }
}
