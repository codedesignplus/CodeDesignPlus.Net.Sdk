namespace CodeDesignPlus.Net.Mongo.Abstractions.Operations;

/// <summary>
/// Defines the create operation for an entity.
/// </summary>
/// <typeparam name="TEntity">The type of the entity.</typeparam>
public interface ICreateOperation<in TEntity> where TEntity : class, IEntityBase
{
    /// <summary>
    /// Creates a new entity asynchronously.
    /// </summary>
    /// <param name="entity">The entity to create.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous create operation.</returns>
    Task CreateAsync(TEntity entity, CancellationToken cancellationToken = default);
}