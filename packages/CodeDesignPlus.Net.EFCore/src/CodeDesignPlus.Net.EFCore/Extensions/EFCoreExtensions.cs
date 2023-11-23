using System.Reflection;
using CodeDesignPlus.Net.Core.Abstractions;
using CodeDesignPlus.Net.Core.Abstractions.Models.Pager;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CodeDesignPlus.Net.EFCore.Extensions;

/// <summary>
/// Provides a set of extension methods for CodeDesignPlus.EFCore
/// </summary>
public static class EFCoreExtensions
{
    /// <summary>
    /// Sets the traversal properties of an entity that implements the IEntityBase interface
    /// </summary>
    /// <typeparam name="TKey">Type of data that will identify the record</typeparam>
    /// <typeparam name="TUserKey">Type of data that the user will identify</typeparam>
    /// <typeparam name="TEntity">The entity type to be configured.</typeparam>
    /// <param name="userRequired"></param>
    /// <param name="builder">The builder to be used to configure the entity type.</param>
    public static void ConfigurationBase<TKey, TUserKey, TEntity>(this EntityTypeBuilder<TEntity> builder, bool userRequired = true, int maxLenghtUser = 256)
        where TEntity : class, IEntityBase<TKey, TUserKey>
    {
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.IdUserCreator).HasMaxLength(maxLenghtUser).IsRequired(userRequired);
        builder.Property(x => x.IsActive).IsRequired();
        builder.Property(x => x.CreatedAt).IsRequired();
    }

    /// <summary>
    /// Obtains a set of records from the database returning an object of type Pager for the datatable
    /// </summary>
    /// <typeparam name="TEntity">The entity type to be consult.</typeparam>
    /// <param name="query">The type of the elements of source.</param>
    /// <param name="currentPage">Current Page</param>
    /// <param name="pageSize">Page Size</param>
    /// <returns>Represents an asynchronous operation that can return a value.</returns>
    public static async Task<Pager<TEntity>> ToPageAsync<TEntity>(this IQueryable<TEntity> query, int currentPage, int pageSize)
        where TEntity : class, IEntityBase
    {
        if (currentPage < 1 || pageSize < 1)
            return default;

        var totalItems = await query.CountAsync();

        var skip = (currentPage - 1) * pageSize;

        var data = await query.Skip(skip).Take(pageSize).ToListAsync();

        return new Pager<TEntity>(totalItems, data, currentPage, pageSize);
    }

    /// <summary>
    /// Obtenga todas las clases que implementan la interfaz IEntityTypeConfiguration{TEntity} y cree una instancia para invocar el método configure
    /// </summary>
    /// <typeparam name="TContext">Represents a session with the database and can be used to query and save instances of your entities</typeparam>
    /// <param name="builder">The builder to be used to configure the entity type.</param>
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
    /// Method Extension that get the method Entity of the object ModelBuilder
    /// </summary>
    /// <param name="builder">Object of type <see cref="ModelBuilder"/></param>
    /// <returns>Return an object of type <see cref="MethodInfo"/></returns>
    private static MethodInfo GetMethodEntity(this ModelBuilder builder)
    {
        return builder
            .GetType()
            .GetMethods()
            .Single(x => x.IsGenericMethod && x.Name == nameof(ModelBuilder.Entity) && x.ReturnType.Name == "EntityTypeBuilder`1");
    }

    /// <summary>
    /// Method extension that validates if the type inheritance is IEntityTypeConfiguration
    /// </summary>
    /// <param name="type">Type</param>
    /// <returns>Return true if the type inheritanced is of IEntityTypeConfiguration</returns>
    private static bool IsEntityTypeConfiguration(Type type)
    {
        return type
            .GetInterfaces()
            .Any(x => x.GetTypeInfo().IsGenericType && x.GetGenericTypeDefinition() == typeof(IEntityTypeConfiguration<>));
    }
}
