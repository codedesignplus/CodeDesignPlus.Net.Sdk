using CodeDesignPlus.Net.Core.Abstractions;
using CodeDesignPlus.Net.Mongo.Abstractions.Options;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Driver;

namespace CodeDesignPlus.Net.Mongo.Repository;

public abstract class RepositoryBase<TKey, TUserKey> : IRepositoryBase<TKey, TUserKey>
{
    private readonly IServiceProvider serviceProvider;

    private readonly ILogger<RepositoryBase<TKey, TUserKey>> logger;

    private readonly MongoOptions mongoOptions;

    public RepositoryBase(IServiceProvider serviceProvider, IOptions<MongoOptions> mongoOptions, ILogger<RepositoryBase<TKey, TUserKey>> logger)
    {
        this.serviceProvider = serviceProvider;
        this.logger = logger;
        this.mongoOptions = mongoOptions.Value;
    }

    public IMongoCollection<TEntity> GetCollection<TEntity>()
        where TEntity : class, IEntityBase<TKey, TUserKey>
    {
        return serviceProvider.GetRequiredService<IMongoCollection<TEntity>>();
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

        return result.IsAcknowledged && result.DeletedCount > 0;
    }

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

    public async Task<bool> UpdateAsync<TEntity>(TEntity entity, CancellationToken cancellationToken)
        where TEntity : class, IEntityBase<TKey, TUserKey>
    {
        var collection = this.GetCollection<TEntity>();

        FilterDefinition<TEntity> filter = Builders<TEntity>.Filter.Eq(e => e.Id, entity.Id);

        var result = await collection.ReplaceOneAsync(filter, entity, cancellationToken: cancellationToken);

        return result.IsAcknowledged && result.ModifiedCount > 0; ;
    }

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
