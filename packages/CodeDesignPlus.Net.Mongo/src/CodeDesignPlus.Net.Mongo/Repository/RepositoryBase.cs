using CodeDesignPlus.Net.Core.Abstractions;
using MongoDB.Bson;
using MongoDB.Driver;

namespace CodeDesignPlus.Net.Mongo;

public class RepositoryBase<TKey, TUserKey> : IRepositoryBase<TKey, TUserKey>
{
    private readonly MongoClient mongoClient;
    private readonly IMongoDatabase database;
    public RepositoryBase(MongoClient mongoClient, string databaseName)
    {
        this.mongoClient = mongoClient;
        this.database = this.mongoClient.GetDatabase(databaseName);
    }

    public IMongoCollection<TEntity> GetCollection<TEntity>()
        where TEntity : class, IEntityBase<TKey, TUserKey>
    {
        return this.database.GetCollection<TEntity>(typeof(TEntity).Name);
    }

    public async Task<bool> ChangeStateAsync<TEntity>(TKey id, bool state, CancellationToken cancellationToken)
        where TEntity : class, IEntityBase<TKey, TUserKey>
    {
        var collection = this.GetCollection<TEntity>();

        var filter = Builders<TEntity>.Filter.Eq(e => e.Id, id);
        var update = Builders<TEntity>.Update.Set(e => e.IsActive, state);

        var result = await collection.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);

        return result.IsAcknowledged && result.ModifiedCount > 0;
    }

    public async Task<TEntity> CreateAsync<TEntity>(TEntity entity, CancellationToken cancellationToken)
        where TEntity : class, IEntityBase<TKey, TUserKey>
    {
        var collection = this.GetCollection<TEntity>();

        await collection.InsertOneAsync(entity, cancellationToken: cancellationToken);

        return entity;
    }

    public async Task<List<TEntity>> CreateRangeAsync<TEntity>(List<TEntity> entities, CancellationToken cancellationToken)
        where TEntity : class, IEntityBase<TKey, TUserKey>
    {
        var collection = this.GetCollection<TEntity>();

        await collection.InsertManyAsync(entities, cancellationToken: cancellationToken);

        return entities;
    }

    public async Task<bool> DeleteAsync<TEntity>(FilterDefinition<TEntity> filter, CancellationToken cancellationToken)
        where TEntity : class, IEntityBase<TKey, TUserKey>
    {
        var collection = this.GetCollection<TEntity>();

        var result = await collection.DeleteOneAsync(filter, cancellationToken);

        return result.IsAcknowledged && result.DeletedCount > 0; ;
    }

    public Task<bool> DeleteRangeAsync<TEntity>(List<TEntity> entities, CancellationToken cancellationToken)
        where TEntity : class, IEntityBase<TKey, TUserKey>
    {
        foreach (var entity in entities)
        {
            var filter = Builders<TEntity>.Filter.Eq(e => e.Id, entity.Id);

            return this.DeleteAsync(filter, cancellationToken);
        }

        return Task.FromResult(false);
    }

    public async Task<bool> UpdateAsync<TEntity>(TEntity entity, CancellationToken cancellationToken)
        where TEntity : class, IEntityBase<TKey, TUserKey>
    {
        var collection = this.GetCollection<TEntity>();

        var filter = Builders<TEntity>.Filter.Eq("_id", new ObjectId(entity.Id.ToString()));

        var result = await collection.UpdateOneAsync(filter, new ObjectUpdateDefinition<TEntity>(entity), cancellationToken: cancellationToken);

        return result.IsAcknowledged && result.ModifiedCount > 0; ;
    }

    public Task<bool> UpdateRangeAsync<TEntity>(List<TEntity> entities, CancellationToken cancellationToken)
        where TEntity : class, IEntityBase<TKey, TUserKey>
    {
        foreach (var entity in entities)
        {
            return this.UpdateAsync(entity, cancellationToken);
        }

        return Task.FromResult(false);
    }

    public async Task<TResult> TransactionAsync<TResult>(Func<IMongoDatabase, Task<TResult>> process, CancellationToken cancellationToken = default)
    {
        using var session = await this.mongoClient.StartSessionAsync(cancellationToken: cancellationToken);

        session.StartTransaction();

        try
        {
            var result = await process(this.database);

            await session.CommitTransactionAsync(cancellationToken);

            return result;
        }
        catch (Exception)
        {
            await session.AbortTransactionAsync(cancellationToken);

            throw;
        }
    }
}
