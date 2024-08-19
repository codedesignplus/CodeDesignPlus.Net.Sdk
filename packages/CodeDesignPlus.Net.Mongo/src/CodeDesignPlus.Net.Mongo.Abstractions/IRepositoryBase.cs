namespace CodeDesignPlus.Net.Mongo.Abstractions;

/// <summary>
/// Defines the base repository operations for MongoDB.
/// </summary>
public interface IRepositoryBase
{
    /// <summary>
    /// Creates a new entity asynchronously.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="entity">The entity to create.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous create operation.</returns>
    Task CreateAsync<TEntity>(TEntity entity, CancellationToken cancellationToken) where TEntity : class, IEntityBase;

    /// <summary>
    /// Creates a range of new entities asynchronously.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="entities">The entities to create.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous create operation.</returns>
    Task CreateRangeAsync<TEntity>(List<TEntity> entities, CancellationToken cancellationToken) where TEntity : class, IEntityBase;

    /// <summary>
    /// Updates an entity asynchronously.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="entity">The entity to update.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous update operation.</returns>
    Task UpdateAsync<TEntity>(TEntity entity, CancellationToken cancellationToken) where TEntity : class, IEntityBase;

    /// <summary>
    /// Updates a range of entities asynchronously.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="entities">The entities to update.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous update operation.</returns>
    Task UpdateRangeAsync<TEntity>(List<TEntity> entities, CancellationToken cancellationToken) where TEntity : class, IEntityBase;

    /// <summary>
    /// Deletes an entity asynchronously.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="filter">The filter definition to identify the entity to delete.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous delete operation.</returns>
    Task DeleteAsync<TEntity>(FilterDefinition<TEntity> filter, CancellationToken cancellationToken) where TEntity : class, IEntityBase;

    /// <summary>
    /// Deletes a range of entities asynchronously.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="entities">The entities to delete.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous delete operation.</returns>
    Task DeleteRangeAsync<TEntity>(List<TEntity> entities, CancellationToken cancellationToken) where TEntity : class, IEntityBase;

    /// <summary>
    /// Changes the state of an entity by its identifier asynchronously.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="id">The identifier of the entity.</param>
    /// <param name="state">The new state of the entity.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous change state operation.</returns>
    Task ChangeStateAsync<TEntity>(Guid id, bool state, CancellationToken cancellationToken) where TEntity : class, IEntity;

    /// <summary>
    /// Executes a transaction asynchronously.
    /// </summary>
    /// <param name="process">The process to execute within the transaction.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous transaction operation.</returns>
    Task TransactionAsync(Func<IMongoDatabase, IClientSessionHandle, Task> process, CancellationToken cancellationToken);

    /// <summary>
    /// Finds entities matching the specified criteria asynchronously.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="criteria">The criteria to match.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous find operation.</returns>
    Task<List<TEntity>> MatchingAsync<TEntity>(C.Criteria criteria, CancellationToken cancellationToken) where TEntity : class, IEntityBase;

    /// <summary>
    /// Finds entities matching the specified criteria and projects them to the specified result type asynchronously.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="criteria">The criteria to match.</param>
    /// <param name="projection">The projection expression.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous find operation.</returns>
    Task<List<TResult>> MatchingAsync<TEntity, TResult>(C.Criteria criteria, Expression<Func<TEntity, TResult>> projection, CancellationToken cancellationToken) where TEntity : class, IEntityBase;

    /// <summary>
    /// Finds entities matching the specified criteria and projects them to the specified projection type asynchronously.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TProjection">The type of the projection.</typeparam>
    /// <param name="id">The identifier of the entity.</param>
    /// <param name="criteria">The criteria to match.</param>
    /// <param name="projection">The projection expression.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous find operation.</returns>
    Task<List<TProjection>> MatchingAsync<TEntity, TProjection>(Guid id, C.Criteria criteria, Expression<Func<TEntity, List<TProjection>>> projection, CancellationToken cancellationToken)
        where TEntity : class, IEntityBase
        where TProjection : class, IEntityBase;
}