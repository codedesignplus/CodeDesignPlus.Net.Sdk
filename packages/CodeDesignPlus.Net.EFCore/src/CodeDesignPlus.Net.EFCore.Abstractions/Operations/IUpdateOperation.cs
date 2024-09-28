namespace CodeDesignPlus.Net.EFCore.Abstractions.Operations;

/// <summary>
/// Allows the repository to update a record by assigning the information to the transversal properties of the entity.
/// </summary>
/// <typeparam name="TEntity">Type of entity to update.</typeparam>
public interface IUpdateOperation<in TEntity> where TEntity : class, IEntityBase
{
    /// <summary>
    /// Updates a record in the database.
    /// </summary>
    /// <param name="id">The ID of the record to update.</param>
    /// <param name="entity">The entity with the information to update.</param>
    /// <param name="cancellationToken">A token to propagate notification that operations should be canceled.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task UpdateAsync(Guid id, TEntity entity, CancellationToken cancellationToken = default);
}