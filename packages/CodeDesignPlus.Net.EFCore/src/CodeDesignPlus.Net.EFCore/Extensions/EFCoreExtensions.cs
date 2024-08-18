namespace CodeDesignPlus.Net.EFCore.Extensions;

/// <summary>
/// Provides extension methods for configuring Entity Framework Core entities and handling pagination.
/// </summary>
public static class EFCoreExtensions
{
    /// <summary>
    /// Configures the base properties for an entity.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="builder">The entity type builder.</param>
    public static void ConfigurationBase<TEntity>(this EntityTypeBuilder<TEntity> builder)
        where TEntity : class, IEntityBase
    {
        builder.Property(x => x.Id);

        if (typeof(IEntity).IsAssignableFrom(typeof(TEntity)))
        {
            builder.Property(nameof(IEntity.CreatedAt)).IsRequired();
            builder.Property(nameof(IEntity.CreatedBy)).IsRequired();
            builder.Property(nameof(IEntity.UpdatedAt)).IsRequired(false);
            builder.Property(nameof(IEntity.UpdatedBy)).IsRequired(false);
        }
    }

    /// <summary>
    /// Paginates the query results asynchronously.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="query">The query to paginate.</param>
    /// <param name="currentPage">The current page number.</param>
    /// <param name="pageSize">The size of the page.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the paginated results.</returns>
    public static async Task<Pager<TEntity>> ToPageAsync<TEntity>(this IQueryable<TEntity> query, int currentPage, int pageSize)
        where TEntity : class, IEntity
    {
        if (currentPage < 1 || pageSize < 1)
            return default;

        var totalItems = await query.CountAsync().ConfigureAwait(false);

        var skip = (currentPage - 1) * pageSize;

        var data = await query.Skip(skip).Take(pageSize).ToListAsync().ConfigureAwait(false);

        return new Pager<TEntity>(totalItems, data, currentPage, pageSize);
    }

    /// <summary>
    /// Registers entity configurations from the specified context.
    /// </summary>
    /// <typeparam name="TContext">The type of the context.</typeparam>
    /// <param name="builder">The model builder.</param>
    public static void RegisterEntityConfigurations<TContext>(this ModelBuilder builder)
        where TContext : DbContext
    {
        var assembly = typeof(TContext).GetTypeInfo().Assembly;

        var entityConfigurationTypes = assembly.GetTypes().Where(x => IsEntityTypeConfiguration(x));

        var entityMethod = builder.GetMethodEntity();

        foreach (var entityConfigurationType in entityConfigurationTypes)
        {
            var genericTypeArgument = entityConfigurationType.GetInterfaces().Single().GenericTypeArguments.Single();

            var genericEntityMethod = entityMethod.MakeGenericMethod(genericTypeArgument);

            var entityTypeBuilder = genericEntityMethod.Invoke(builder, null);

            var instance = Activator.CreateInstance(entityConfigurationType);

            instance.GetType().GetMethod(nameof(IEntityTypeConfiguration<object>.Configure)).Invoke(instance, new[] { entityTypeBuilder });
        }
    }

    /// <summary>
    /// Gets the method for configuring an entity.
    /// </summary>
    /// <param name="builder">The model builder.</param>
    /// <returns>The method info for configuring an entity.</returns>
    private static MethodInfo GetMethodEntity(this ModelBuilder builder)
    {
        return builder
            .GetType()
            .GetMethods()
            .Single(x => x.IsGenericMethod && x.Name == nameof(ModelBuilder.Entity) && x.ReturnType.Name == "EntityTypeBuilder`1");
    }
    
    /// <summary>
    /// Determines whether the specified type is an entity type configuration.
    /// </summary>
    /// <param name="type">The type to check.</param>
    /// <returns>true if the type is an entity type configuration; otherwise, false.</returns>
    private static bool IsEntityTypeConfiguration(Type type)
    {
        return Array.Exists(type.GetInterfaces(), x => x.GetTypeInfo().IsGenericType && x.GetGenericTypeDefinition() == typeof(IEntityTypeConfiguration<>));
    }
}