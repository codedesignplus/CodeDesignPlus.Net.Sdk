namespace CodeDesignPlus.Net.Mongo.Abstractions.Operations;

/// <summary>
/// Defines the update operation for an entity.
/// </summary>
/// <typeparam name="TEntity">The type of the entity.</typeparam>
public interface IUpdateOperation<in TEntity> where TEntity : class, IEntityBase
{
    /// <summary>
    /// Updates an entity by its identifier asynchronously.
    /// </summary>
    /// <param name="id">The identifier of the entity to update.</param>
    /// <param name="entity">The entity with updated values.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous update operation.</returns>
    Task UpdateAsync(Guid id, TEntity entity, CancellationToken cancellationToken);
    /// <summary>
    /// Updates an entity by its identifier asynchronously.
    /// </summary>
    /// <param name="id">The identifier of the entity to update.</param>
    /// <param name="entity">The entity with updated values.</param>
    /// <param name="tenant">The tenant identifier only for entities that inherit from <see cref="AggregateRoot"/>.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous update operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the entity is null.</exception>
    Task UpdateAsync(Guid id, TEntity entity, Guid tenant, CancellationToken cancellationToken);
}