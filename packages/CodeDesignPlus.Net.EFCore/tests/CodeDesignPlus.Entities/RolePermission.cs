﻿using CodeDesignPlus.Net.Core.Abstractions;
using System;

namespace CodeDesignPlus.Entities
{
    public class RolePermission : IEntityLong<int>
    {
        public long Id { get; set; }
        public long IdApplication { get; set; }
        public long IdPermission { get; set; }
        public string NameRole { get; set; }
        public bool IsActive { get; set; }
        public int IdUserCreator { get; set; }
        public DateTime CreatedAt { get; set; }

        public Application Application { get; set; }
        public Permission Permission { get; set; }
    }
}
