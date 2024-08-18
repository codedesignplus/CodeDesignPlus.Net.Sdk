namespace CodeDesignPlus.Net.EFCore.Abstractions.Operations;

/// <summary>
/// Provides an interface for creating a record in the repository and assigning information to the transversal properties of the entity.
/// </summary>
/// <typeparam name="TEntity">The type of entity to create.</typeparam>
public interface ICreateOperation<in TEntity> where TEntity : class, IEntityBase
{
    /// <summary>
    /// Creates a record in the database.
    /// </summary>
    /// <param name="entity">The entity to create.</param>
    /// <param name="cancellationToken">A token to propagate notification that operations should be canceled.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task CreateAsync(TEntity entity, CancellationToken cancellationToken = default);
}