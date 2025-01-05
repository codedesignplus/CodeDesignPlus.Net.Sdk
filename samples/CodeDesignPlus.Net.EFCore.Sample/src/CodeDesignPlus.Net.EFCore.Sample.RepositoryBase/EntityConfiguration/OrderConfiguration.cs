using CodeDesignPlus.Net.EFCore.Extensions;
using CodeDesignPlus.Net.EFCore.Sample.RepositoryBase.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CodeDesignPlus.Net.EFCore.Sample.RepositoryBase.EntityConfiguration;

public class OrderConfiguration : IEntityTypeConfiguration<OrderAggregate>
{
    public void Configure(EntityTypeBuilder<OrderAggregate> builder)
    {
        builder.ConfigurationBase();

        builder.ToTable("Orders");
        builder.Property(x => x.Name).HasColumnType("varchar(64)").IsRequired();
        builder.Property(x => x.Description).HasColumnType("varchar(512)").IsRequired();
    }
}