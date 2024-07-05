using CodeDesignPlus.Net.Core.Abstractions;
using MongoDB.Driver;
using System.Linq.Expressions;
using C = CodeDesignPlus.Net.Core.Abstractions.Models.Criteria;

namespace CodeDesignPlus.Net.Mongo.Abstractions;

/// <summary>
/// Exposes the base methods to carray out the most concurrent operations with the database
/// </summary>
public interface IRepositoryBase
{
    /// <summary>
    /// Get a Mongo Collection that can be used to query and save instances of TEntity.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity for which a set should be returned.</typeparam>
    /// <returns>A set for the given entity type.</returns>
    IMongoCollection<TEntity> GetCollection<TEntity>() where TEntity : class, IEntityBase;
    /// <summary>
    /// Method that creates an entity in the database
    /// </summary>
    /// <typeparam name="TEntity">Type of the entity to create</typeparam>
    /// <param name="entity">Entity to create</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>Represents an asynchronous operation</returns>
    Task CreateAsync<TEntity>(TEntity entity, CancellationToken cancellationToken) where TEntity : class, IEntityBase;
    /// <summary>
    /// Method that updates an entity in the database
    /// </summary>
    /// <typeparam name="TEntity">Type of the entity to udpate</typeparam>
    /// <param name="entity">Entity to update</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>Represents an asynchronous operation</returns>
    Task UpdateAsync<TEntity>(TEntity entity, CancellationToken cancellationToken) where TEntity : class, IEntityBase;
    /// <summary>
    /// Method that deletes an entity in the database
    /// </summary>
    /// <typeparam name="TEntity">Type of the entity to delete</typeparam>
    /// <param name="filter">A filter that determines which elements to delete from the collection.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>Represents an asynchronous operation</returns>
    Task DeleteAsync<TEntity>(FilterDefinition<TEntity> filter, CancellationToken cancellationToken) where TEntity : class, IEntityBase;
    /// <summary>
    /// Method that creates a set of entities in the database
    /// </summary>
    /// <typeparam name="TEntity">Type of the entity to create</typeparam>
    /// <param name="entities">List of entities to create</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>Represents an asynchronous operation</returns>
    Task CreateRangeAsync<TEntity>(List<TEntity> entities, CancellationToken cancellationToken) where TEntity : class, IEntityBase;
    /// <summary>
    /// Method that updates a set of entities in the database
    /// </summary>
    /// <typeparam name="TEntity">Type of the entity to update</typeparam>
    /// <param name="entities">List of entities to update</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>Represents an asynchronous operation</returns>
    Task UpdateRangeAsync<TEntity>(List<TEntity> entities, CancellationToken cancellationToken) where TEntity : class, IEntityBase;
    /// <summary>
    /// Method that deletes a set of entities in the database
    /// </summary>
    /// <typeparam name="TEntity">Type of the entity to delete</typeparam>
    /// <param name="entities">List of entities to delete</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>Represents an asynchronous operation</returns>
    Task DeleteRangeAsync<TEntity>(List<TEntity> entities, CancellationToken cancellationToken) where TEntity : class, IEntityBase;
    /// <summary>
    /// Method that will change the state to the registry in the database
    /// </summary>
    /// <typeparam name="TEntity">Type of the entity to update</typeparam>
    /// <param name="id">Id of the record to update</param>
    /// <param name="state">Status tha will be assigned to the record if it exists</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>Represents an asynchronous operation</returns>
    Task ChangeStateAsync<TEntity>(Guid id, bool state, CancellationToken cancellationToken) where TEntity : class, IEntity;
    /// <summary>
    /// Method that allows multiple process in the database in a single transaction
    /// </summary>
    /// <typeparam name="TResult">Type of data to return if the process is succesful</typeparam>
    /// <param name="process">Process to execute in the transaction flow</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>Represents an asynchronous operation</returns>
    Task TransactionAsync(Func<IMongoDatabase, IClientSessionHandle, Task> process, CancellationToken cancellationToken);
    /// <summary>
    /// Method that returns a list of records from the database.
    /// </summary>
    /// <typeparam name="TEntity">The entity type to be configured.</typeparam>
    /// <param name="criteria">The criteria to filter the records.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>Represents an asynchronous operation</returns>    
    Task<List<TEntity>> MatchingAsync<TEntity>(C.Criteria criteria, CancellationToken cancellationToken) where TEntity : class, IEntityBase;
    /// <summary>
    /// Method that returns a list of records from the database.
    /// </summary>
    /// <typeparam name="TEntity">The entity type to be configured.</typeparam>
    /// <typeparam name="TResult">The type of the result to project.</typeparam>
    /// <param name="criteria">The criteria to filter the records.</param>
    /// <param name="projection">The projection to apply to the records.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>Represents an asynchronous operation</returns>
    Task<List<TResult>> MatchingAsync<TEntity, TResult>(C.Criteria criteria, Expression<Func<TEntity, TResult>> projection, CancellationToken cancellationToken) where TEntity : class, IEntityBase;
    /// <summary>
    /// Method that returns a list of records from the database.
    /// </summary>
    /// <typeparam name="TEntity">The entity type to be configured.</typeparam>
    /// <param name="id">The ID of the record to search.</param>
    /// <param name="criteria">The criteria to filter the records.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>Represents an asynchronous operation</returns>
    Task<List<TEntity>> MatchingAsync<TEntity>(Guid id, C.Criteria criteria, Expression<Func<TEntity, List<TEntity>>> projection, CancellationToken cancellationToken) where TEntity : class, IEntityBase;
}