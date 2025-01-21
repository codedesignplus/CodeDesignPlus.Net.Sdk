﻿using CodeDesignPlus.Entities;
using CodeDesignPlus.Net.EFCore;
using CodeDesignPlus.Net.EFCore.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace CodeDesignPlus.InMemory
{
    public class CodeDesignPlusContextInMemory : DbContextBase
    {
        public CodeDesignPlusContextInMemory([NotNull] DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.RegisterEntityConfigurations<CodeDesignPlusContextInMemory>();
        }

        public DbSet<Application> Application { get; set; }
        public DbSet<Permission> Permission { get; set; }
        public DbSet<AppPermision> AppPermision { get; set; }
        public DbSet<RolePermission> RolePermission { get; set; }
    }
}
