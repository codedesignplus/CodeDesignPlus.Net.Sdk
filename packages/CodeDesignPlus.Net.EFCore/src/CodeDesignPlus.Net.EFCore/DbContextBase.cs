using CodeDesignPlus.Net.EFCore.Converters;

namespace CodeDesignPlus.Net.EFCore;

/// <summary>
/// Base class for the DbContext of the application.
/// </summary>
public abstract class DbContextBase(DbContextOptions options): DbContext(options)
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DbContextBase"/> class.
    /// </summary>
    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<Instant>()
            .HaveConversion<InstantValueConverter>();

        configurationBuilder.Properties<Instant?>()
            .HaveConversion<NullableInstantValueConverter>();
    }
}
