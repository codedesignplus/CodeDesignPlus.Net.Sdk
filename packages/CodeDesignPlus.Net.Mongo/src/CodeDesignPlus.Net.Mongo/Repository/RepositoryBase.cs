﻿using CodeDesignPlus.Net.Core.Abstractions;
using CodeDesignPlus.Net.Mongo.Abstractions.Options;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace CodeDesignPlus.Net.Mongo.Repository;

/// <summary>
/// Represents a base repository class for MongoDB operations.
/// </summary>
/// <typeparam name="TKey">The type of the entity's primary key.</typeparam>
/// <typeparam name="TUserKey">The type of the user's primary key.</typeparam>
public abstract class RepositoryBase<TKey, TUserKey>: IRepositoryBase<TKey, TUserKey>
{
    private readonly IServiceProvider serviceProvider;
    private readonly ILogger<RepositoryBase<TKey, TUserKey>> logger;
    private readonly MongoOptions mongoOptions;

    /// <summary>
    /// Initializes a new instance of the RepositoryBase class using the specified options.
    /// </summary>
    /// <param name="serviceProvider">The IServiceProvider to add services to the container.</param>
    /// <param name="mongoOptions">The options to configure a MongoDB.</param>
    /// <param name="logger">Represents a type used to perform logging.</param>
    public RepositoryBase(IServiceProvider serviceProvider, IOptions<MongoOptions> mongoOptions, ILogger<RepositoryBase<TKey, TUserKey>> logger)
    {
        this.serviceProvider = serviceProvider;
        this.logger = logger;
        this.mongoOptions = mongoOptions.Value;
    }

    /// <summary>
    /// Method that returns a list of records from the database.
    /// </summary>
    /// <typeparam name="TEntity">The entity type to be configured.</typeparam>
    /// <returns>Represents an asynchronous operation that can return a value.</returns>
    public IMongoCollection<TEntity> GetCollection<TEntity>()
        where TEntity : class, IEntityBase<TKey, TUserKey>
    {
        return serviceProvider.GetRequiredService<IMongoCollection<TEntity>>();
    }

    /// <summary>
    /// Changes the state of a record in the database.
    /// </summary>
    /// <typeparam name="TEntity">The entity type to be configured.</typeparam>
    /// <param name="id">The ID of the record to search.</param>
    /// <param name="state">The state of the record to set.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>Represents an asynchronous operation that can return a value.</returns>
    public async Task<bool> ChangeStateAsync<TEntity>(TKey id, bool state, CancellationToken cancellationToken)
        where TEntity : class, IEntityBase<TKey, TUserKey>
    {
        var collection = this.GetCollection<TEntity>();

        var filter = Builders<TEntity>.Filter.Eq(e => e.Id, id);
        var update = Builders<TEntity>.Update.Set(e => e.IsActive, state);

        var result = await collection.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);

        return result.ModifiedCount > 0;
    }

    /// <summary>
    /// Creates a new record in the database.
    /// </summary>
    /// <typeparam name="TEntity">The entity type to be configured.</typeparam>
    /// <param name="entity">The entity to be created.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>Represents an asynchronous operation that can return a value.</returns>
    public async Task<TEntity> CreateAsync<TEntity>(TEntity entity, CancellationToken cancellationToken)
        where TEntity : class, IEntityBase<TKey, TUserKey>
    {
        var collection = this.GetCollection<TEntity>();

        await collection.InsertOneAsync(entity, cancellationToken: cancellationToken);

        return entity;
    }

    /// <summary>
    /// Creates multiple records in the database.
    /// </summary>
    /// <typeparam name="TEntity">The entity type to be configured.</typeparam>
    /// <param name="entities">The list of entities to be created.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>Represents an asynchronous operation that can return a value.</returns>
    public async Task<List<TEntity>> CreateRangeAsync<TEntity>(List<TEntity> entities, CancellationToken cancellationToken)
        where TEntity : class, IEntityBase<TKey, TUserKey>
    {
        var collection = this.GetCollection<TEntity>();

        await collection.InsertManyAsync(entities, cancellationToken: cancellationToken);

        return entities;
    }

    /// <summary>
    /// Deletes a record from the database.
    /// </summary>
    /// <typeparam name="TEntity">The entity type to be configured.</typeparam>
    /// <param name="filter">The filter to search for the record to delete.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>Represents an asynchronous operation that can return a value.</returns>
    public async Task<bool> DeleteAsync<TEntity>(FilterDefinition<TEntity> filter, CancellationToken cancellationToken)
        where TEntity : class, IEntityBase<TKey, TUserKey>
    {
        var collection = this.GetCollection<TEntity>();

        var result = await collection.DeleteOneAsync(filter, cancellationToken);

        return result.DeletedCount > 0;
    }

    /// <summary>
    /// Deletes multiple records from the database.
    /// </summary>
    /// <typeparam name="TEntity">The entity type to be configured.</typeparam>
    /// <param name="entities">The list of entities to be deleted.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>Represents an asynchronous operation that can return a value.</returns>
    public async Task<bool> DeleteRangeAsync<TEntity>(List<TEntity> entities, CancellationToken cancellationToken)
        where TEntity : class, IEntityBase<TKey, TUserKey>
    {
        var result = new List<bool>();

        foreach (var entity in entities)
        {
            var filter = Builders<TEntity>.Filter.Eq(e => e.Id, entity.Id);

            result.Add(await this.DeleteAsync(filter, cancellationToken));
        }

        return result.All(x => x);
    }

    /// <summary>
    /// Updates a record in the database.
    /// </summary>
    /// <typeparam name="TEntity">The entity type to be configured.</typeparam>
    /// <param name="entity">The entity to be updated.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>Represents an asynchronous operation that can return a value.</returns>
    public async Task<bool> UpdateAsync<TEntity>(TEntity entity, CancellationToken cancellationToken)
        where TEntity : class, IEntityBase<TKey, TUserKey>
    {
        var collection = this.GetCollection<TEntity>();

        FilterDefinition<TEntity> filter = Builders<TEntity>.Filter.Eq(e => e.Id, entity.Id);

        var result = await collection.ReplaceOneAsync(filter, entity, cancellationToken: cancellationToken);

        return result.ModifiedCount > 0;
    }

    /// <summary>
    /// Updates multiple records in the database.
    /// </summary>
    /// <typeparam name="TEntity">The entity type to be configured.</typeparam>
    /// <param name="entities">The list of entities to be updated.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>Represents an asynchronous operation that can return a value.</returns>
    public async Task<bool> UpdateRangeAsync<TEntity>(List<TEntity> entities, CancellationToken cancellationToken)
        where TEntity : class, IEntityBase<TKey, TUserKey>
    {
        var result = new List<bool>();

        foreach (var entity in entities)
        {
            result.Add(await this.UpdateAsync(entity, cancellationToken));
        }

        return result.All(x => x);
    }

    /// <summary>
    /// Executes a transactional operation on the MongoDB database.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="process">The function that represents the transactional operation.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>Represents an asynchronous operation that can return a value.</returns>
    public async Task<TResult> TransactionAsync<TResult>(Func<IMongoDatabase, IClientSessionHandle, Task<TResult>> process, CancellationToken cancellationToken = default)
    {
        var client = this.serviceProvider.GetRequiredService<IMongoClient>();

        var database = client.GetDatabase(this.mongoOptions.Database);

        using var session = await client.StartSessionAsync(cancellationToken: cancellationToken);

        session.StartTransaction();

        try
        {
            var result = await process(database, session);

            await session.CommitTransactionAsync(cancellationToken);

            return result;
        }
        catch (Exception ex)
        {
            await session.AbortTransactionAsync(cancellationToken);

            this.logger.LogError(ex, "Failed to execute transaction");
        }

        return default;
    }
}