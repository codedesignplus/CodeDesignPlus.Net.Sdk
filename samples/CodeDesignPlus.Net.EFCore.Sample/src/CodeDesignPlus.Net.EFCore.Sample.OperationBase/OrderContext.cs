using System.Diagnostics.CodeAnalysis;
using CodeDesignPlus.Net.EFCore.Extensions;
using CodeDesignPlus.Net.EFCore.Sample.OperationBase.Entities;
using Microsoft.EntityFrameworkCore;

namespace CodeDesignPlus.Net.EFCore.Sample.OperationBase;

public class OrderContext([NotNull] DbContextOptions options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.RegisterEntityConfigurations<OrderContext>();
    }

    public DbSet<OrderAggregate> Order { get; set; }
}
