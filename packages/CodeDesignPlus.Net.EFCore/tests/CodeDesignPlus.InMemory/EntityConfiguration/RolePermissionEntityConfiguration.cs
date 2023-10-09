﻿using CodeDesignPlus.Net.EFCore.Extensions;
using CodeDesignPlus.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CodeDesignPlus.InMemory.EntityConfiguration
{
    public class RolePermissionEntityConfiguration : IEntityTypeConfiguration<RolePermission>
    {
        /// <summary>
        /// Control property for unit tests
        /// </summary>
        public static bool IsInvoked;

        public void Configure(EntityTypeBuilder<RolePermission> builder)
        {
            IsInvoked = true;

            builder.ConfigurationBase<long, int, RolePermission>();

            builder.Property(x => x.NameRole).HasColumnType("varchar(32)").IsRequired();

            builder.HasOne(x => x.Permission).WithMany(x => x.RolePermisions).HasForeignKey(x => x.IdPermission);
            builder.HasOne(x => x.Application).WithMany(x => x.RolePermisions).HasForeignKey(x => x.IdApplication);
        }
    }
}
