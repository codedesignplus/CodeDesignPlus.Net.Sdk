using CodeDesignPlus.Entities;
using CodeDesignPlus.Net.EFCore.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CodeDesignPlus.InMemory.EntityConfiguration
{
    public class ApplicationEntityConfiguration : IEntityTypeConfiguration<Application>
    {
        /// <summary>
        /// Control property for unit tests
        /// </summary>
        public static bool IsInvoked;

        public void Configure(EntityTypeBuilder<Application> builder)
        {
            IsInvoked = true;

            builder.ConfigurationBase();

            builder.ToTable("Aplicacion");
            builder.Property(x => x.Name).HasColumnType("varchar(64)").IsRequired();
            builder.Property(x => x.Description).HasColumnType("varchar(512)").IsRequired();
        }
    }
}
