﻿using System.Linq.Expressions;
using System.Data;
using Microsoft.EntityFrameworkCore;
using CodeDesignPlus.Net.Core.Abstractions;

namespace CodeDesignPlus.Net.EFCore.Abstractions;

/// <summary>
/// Exposes the base methods to carray out the most concurrent operations with the database
/// </summary>
public interface IRepositoryBase
{
    /// <summary>
    /// Convert the DbContext to the assigned generic type
    /// </summary>
    /// <typeparam name="TContext">Type of context to return</typeparam>
    /// <returns>Returns the context of the database</returns>
    TContext GetContext<TContext>() where TContext : DbContext;
    /// <summary>
    /// Get a DbSet that can be used to query and save instances of TEntity.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity for which a set should be returned.</typeparam>
    /// <returns>A set for the given entity type.</returns>
    DbSet<TEntity> GetEntity<TEntity>() where TEntity : class, IEntityBase;
    /// <summary>
    /// Method that creates an entity in the database
    /// </summary>
    /// <typeparam name="TEntity">Type of the entity to create</typeparam>
    /// <param name="entity">Entity to create</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>Represents an asynchronous operation that can return a value.</returns>
    Task CreateAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default) where TEntity : class, IEntityBase;
    /// <summary>
    /// Method that updates an entity in the database
    /// </summary>
    /// <typeparam name="TEntity">Type of the entity to udpate</typeparam>
    /// <param name="entity">Entity to update</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>Represents an asynchronous operation that can return a value.</returns>
    Task UpdateAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default) where TEntity : class, IEntityBase;
    /// <summary>
    /// Method that deletes an entity in the database
    /// </summary>
    /// <typeparam name="TEntity">Type of the entity to delete</typeparam>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>Represents an asynchronous operation that can return a value.</returns>
    Task DeleteAsync<TEntity>(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default) where TEntity : class, IEntityBase;
    /// <summary>
    /// Method that creates a set of entities in the database
    /// </summary>
    /// <typeparam name="TEntity">Type of the entity to create</typeparam>
    /// <param name="entities">List of entities to create</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>Represents an asynchronous operation that can return a value.</returns>
    Task CreateRangeAsync<TEntity>(List<TEntity> entities, CancellationToken cancellationToken = default) where TEntity : class, IEntityBase;
    /// <summary>
    /// Method that updates a set of entities in the database
    /// </summary>
    /// <typeparam name="TEntity">Type of the entity to update</typeparam>
    /// <param name="entities">List of entities to update</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>Represents an asynchronous operation that can return a value.</returns>
    Task UpdateRangeAsync<TEntity>(List<TEntity> entities, CancellationToken cancellationToken = default) where TEntity : class, IEntityBase;
    /// <summary>
    /// Method that deletes a set of entities in the database
    /// </summary>
    /// <typeparam name="TEntity">Type of the entity to delete</typeparam>
    /// <param name="entities">List of entities to delete</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>Represents an asynchronous operation that can return a value.</returns>
    Task DeleteRangeAsync<TEntity>(List<TEntity> entities, CancellationToken cancellationToken = default) where TEntity : class, IEntityBase;
    /// <summary>
    /// Method that will change the state to the registry in the database
    /// </summary>
    /// <typeparam name="TEntity">Type of the entity to update</typeparam>
    /// <param name="id">Id of the record to update</param>
    /// <param name="state">Status tha will be assigned to the record if it exists</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>Represents an asynchronous operation that can return a value.</returns>
    Task ChangeStateAsync<TEntity>(Guid id, bool state, CancellationToken cancellationToken = default) where TEntity : class, IEntity;
    /// <summary>
    /// Method that allows multiple process in the database in a single transaction
    /// </summary>
    /// <typeparam name="TResult">Type of data to return if the process is succesful</typeparam>
    /// <param name="process">Process to execute in the transaction flow</param>
    /// <param name="isolation">Specifies the transaction locking behavior for the connection.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>Represents an asynchronous operation that can return a value.</returns>
    Task TransactionAsync<TResult>(Func<DbContext, Task> process, IsolationLevel isolation = IsolationLevel.ReadUncommitted, CancellationToken cancellationToken = default);
}