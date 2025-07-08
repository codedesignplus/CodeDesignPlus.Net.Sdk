using CodeDesignPlus.Net.Core.Abstractions.Models.Pager;
using CodeDesignPlus.Net.Mongo.Extensions;

namespace CodeDesignPlus.Net.Mongo.Repository;

/// <summary>
/// Base class for MongoDB repository operations.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="RepositoryBase"/> class.
/// </remarks>
/// <param name="serviceProvider">The service provider.</param>
/// <param name="mongoOptions">The MongoDB options.</param>
/// <param name="logger">The logger instance.</param>
/// <exception cref="ArgumentNullException">Thrown when any of the parameters are null.</exception>
public abstract class RepositoryBase(IServiceProvider serviceProvider, IOptions<MongoOptions> mongoOptions, ILogger logger) : IRepositoryBase
{
    private readonly MongoOptions mongoOptions = mongoOptions.Value;

    /// <summary>
    /// Gets the MongoDB collection for the specified entity type.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <returns>The MongoDB collection.</returns>
    public IMongoCollection<TEntity> GetCollection<TEntity>()
        where TEntity : class, IEntityBase
    {
        var client = serviceProvider.GetRequiredService<IMongoClient>();

        var database = client.GetDatabase(this.mongoOptions.Database);

        return database.GetCollection<TEntity>(typeof(TEntity).Name);
    }

    /// <summary>
    /// Changes the state of an entity by its identifier asynchronously.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="id">The identifier of the entity.</param>
    /// <param name="state">The new state of the entity.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous change state operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the entity is null.</exception>
    public Task ChangeStateAsync<TEntity>(Guid id, bool state, CancellationToken cancellationToken)
        where TEntity : class, IEntity
    {
        return this.ChangeStateAsync<TEntity>(id, state, Guid.Empty, cancellationToken);
    }

    /// <summary>
    /// Changes the state of an entity by its identifier asynchronously.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="id">The identifier of the entity.</param>
    /// <param name="state">The new state of the entity.</param>
    /// <param name="tenant">The tenant identifier only for entities that inherit from <see cref="AggregateRoot"/>.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous change state operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the entity is null.</exception>
    public Task ChangeStateAsync<TEntity>(Guid id, bool state, Guid tenant, CancellationToken cancellationToken)
        where TEntity : class, IEntity
    {
        var collection = this.GetCollection<TEntity>();

        var filter = Builders<TEntity>.Filter.Eq(e => e.Id, id).BuildFilter(tenant);
        var update = Builders<TEntity>.Update
            .Set(e => e.IsActive, state)
            .Set(e => e.UpdatedAt, SystemClock.Instance.GetCurrentInstant());

        return collection.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Creates a new entity asynchronously.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="entity">The entity to create.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous create operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the entity is null.</exception>
    public Task CreateAsync<TEntity>(TEntity entity, CancellationToken cancellationToken)
        where TEntity : class, IEntityBase
    {
        var collection = this.GetCollection<TEntity>();

        return collection.InsertOneAsync(entity, cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Creates a range of new entities asynchronously.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="entities">The entities to create.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous create range operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the entities are null.</exception>
    public Task CreateRangeAsync<TEntity>(List<TEntity> entities, CancellationToken cancellationToken)
        where TEntity : class, IEntityBase
    {
        var collection = this.GetCollection<TEntity>();

        return collection.InsertManyAsync(entities, cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Determines whether an entity exists by its identifier asynchronously.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="id">The identifier of the entity.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous existence operation.</returns>
    public Task<bool> ExistsAsync<TEntity>(Guid id, CancellationToken cancellationToken)
        where TEntity : class, IEntityBase
    {
        return this.ExistsAsync<TEntity>(id, Guid.Empty, cancellationToken);
    }

    /// <summary>
    /// Determines whether an entity exists by its identifier asynchronously.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="id">The identifier of the entity.</param>
    /// <param name="tenant">The tenant identifier only for entities that inherit from <see cref="AggregateRoot"/>.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous existence operation.</returns>
    public Task<bool> ExistsAsync<TEntity>(Guid id, Guid tenant, CancellationToken cancellationToken)
        where TEntity : class, IEntityBase
    {
        var collection = this.GetCollection<TEntity>();

        var filter = Builders<TEntity>.Filter.Eq(e => e.Id, id).BuildFilter(tenant);

        return collection.Find(filter).AnyAsync(cancellationToken);
    }

    /// <summary>
    /// Deletes an entity by its identifier asynchronously.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="id">The identifier of the entity.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous delete operation.</returns>
    public Task DeleteAsync<TEntity>(Guid id, CancellationToken cancellationToken)
        where TEntity : class, IEntityBase
    {
        return this.DeleteAsync<TEntity>(id, Guid.Empty, cancellationToken);
    }

    /// <summary>
    /// Deletes an entity by its filter asynchronously.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="id">The identifier of the entity.</param>
    /// <param name="tenant">The tenant identifier only for entities that inherit from <see cref="AggregateRoot"/>.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous delete operation.</returns>
    public Task DeleteAsync<TEntity>(Guid id, Guid tenant, CancellationToken cancellationToken)
        where TEntity : class, IEntityBase
    {
        var filter = Builders<TEntity>.Filter.Eq(e => e.Id, id).BuildFilter(tenant);

        return this.DeleteAsync(filter, tenant, cancellationToken);
    }

    /// <summary>
    /// Deletes an entity by its filter asynchronously.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="filter">The filter definition.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous delete operation.</returns>
    public Task DeleteAsync<TEntity>(FilterDefinition<TEntity> filter, CancellationToken cancellationToken)
        where TEntity : class, IEntityBase
    {
        return this.DeleteAsync(filter, Guid.Empty, cancellationToken);
    }

    /// <summary>
    /// Deletes an entity by its filter asynchronously.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="filter">The filter definition.</param>
    /// <param name="tenant">The tenant identifier only for entities that inherit from <see cref="AggregateRoot"/>.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous delete operation.</returns>
    public Task DeleteAsync<TEntity>(FilterDefinition<TEntity> filter, Guid tenant, CancellationToken cancellationToken)
        where TEntity : class, IEntityBase
    {
        var collection = this.GetCollection<TEntity>();

        return collection.DeleteOneAsync(filter.BuildFilter(tenant), cancellationToken);
    }

    /// <summary>
    /// Deletes a range of entities asynchronously.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="entities">The entities to delete.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous delete range operation.</returns>
    public Task DeleteRangeAsync<TEntity>(List<TEntity> entities, CancellationToken cancellationToken)
        where TEntity : class, IEntityBase
    {
        return this.DeleteRangeAsync(entities, Guid.Empty, cancellationToken);
    }

    /// <summary>
    /// Deletes a range of entities asynchronously.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="entities">The entities to delete.</param>
    /// <param name="tenant">The tenant identifier only for entities that inherit from <see cref="AggregateRoot"/>.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous delete range operation.</returns>
    public async Task DeleteRangeAsync<TEntity>(List<TEntity> entities, Guid tenant, CancellationToken cancellationToken)
        where TEntity : class, IEntityBase
    {
        foreach (var entity in entities)
        {
            var filter = Builders<TEntity>.Filter.Eq(e => e.Id, entity.Id).BuildFilter(tenant);

            await this.DeleteAsync(filter, tenant, cancellationToken);
        }
    }

    /// <summary>
    /// Updates an entity asynchronously.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="entity">The entity to update.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous update operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the entity is null.</exception>
    public Task UpdateAsync<TEntity>(TEntity entity, CancellationToken cancellationToken)
        where TEntity : class, IEntityBase
    {
        var collection = this.GetCollection<TEntity>();

        FilterDefinition<TEntity> filter = Builders<TEntity>.Filter.Eq(e => e.Id, entity.Id);

        return collection.ReplaceOneAsync(filter, entity, cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Updates a range of entities asynchronously.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="entities">The entities to update.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous update range operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the entities are null.</exception>
    public async Task UpdateRangeAsync<TEntity>(List<TEntity> entities, CancellationToken cancellationToken)
        where TEntity : class, IEntityBase
    {
        foreach (var entity in entities)
        {
            await this.UpdateAsync(entity, cancellationToken);
        }
    }

    /// <summary>
    /// Executes a transaction asynchronously.
    /// </summary>
    /// <param name="process">The process to execute within the transaction.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous transaction operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the process is null.</exception>
    public async Task TransactionAsync(Func<IMongoDatabase, IClientSessionHandle, Task> process, CancellationToken cancellationToken)
    {
        var client = serviceProvider.GetRequiredService<IMongoClient>();

        var database = client.GetDatabase(this.mongoOptions.Database);

        using var session = await client.StartSessionAsync(cancellationToken: cancellationToken);

        session.StartTransaction();

        try
        {
            await process(database, session);

            await session.CommitTransactionAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            await session.AbortTransactionAsync(cancellationToken);

            logger.LogError(ex, "Failed to execute transaction");
        }
    }

    /// <summary>
    /// Finds an entity by its identifier asynchronously.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="id">The identifier of the entity.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous find operation.</returns>
    public Task<TEntity> FindAsync<TEntity>(Guid id, CancellationToken cancellationToken)
        where TEntity : class, IEntityBase
    {
        return this.FindAsync<TEntity>(id, Guid.Empty, cancellationToken);
    }

    /// <summary>
    /// Finds an entity by its identifier asynchronously.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="id">The identifier of the entity.</param>
    /// <param name="tenant">The tenant identifier only for entities that inherit from <see cref="AggregateRoot"/>.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous find operation.</returns>
    public Task<TEntity> FindAsync<TEntity>(Guid id, Guid tenant, CancellationToken cancellationToken)
        where TEntity : class, IEntityBase
    {
        var collection = this.GetCollection<TEntity>();

        var filter = Builders<TEntity>.Filter.Eq(e => e.Id, id).BuildFilter(tenant);

        return collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
    }

    /// <summary>
    /// Finds entities matching the specified criteria asynchronously.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="criteria">The criteria to match.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous matching operation.</returns>
    public Task<Pagination<TEntity>> MatchingAsync<TEntity>(C.Criteria criteria, CancellationToken cancellationToken)
        where TEntity : class, IEntityBase
    {
        return this.MatchingAsync<TEntity>(criteria, Guid.Empty, cancellationToken);
    }

    /// <summary>
    /// Finds entities matching the specified criteria asynchronously.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="criteria">The criteria to match.</param>
    /// <param name="tenant">The tenant identifier only for entities that inherit from <see cref="AggregateRoot"/>.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous matching operation.</returns>
    public async Task<Pagination<TEntity>> MatchingAsync<TEntity>(C.Criteria criteria, Guid tenant, CancellationToken cancellationToken)
        where TEntity : class, IEntityBase
    {
        var filterExpression = criteria.GetFilterExpression<TEntity>();
        var filter = filterExpression.ToFilterDefinition().BuildFilter(tenant);
        var totalCount = await this.GetCollection<TEntity>().CountDocumentsAsync(filter, cancellationToken: cancellationToken);

        var query = Query<TEntity>(criteria, tenant);
        var data = await query.ToListAsync(cancellationToken);

        return Pagination<TEntity>.Create(data, totalCount, criteria.Skip, criteria.Limit);
    }

    /// <summary>
    /// Finds entities matching the specified criteria and projects them to the specified result type asynchronously.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="criteria">The criteria to match.</param>
    /// <param name="projection">The projection expression.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous matching operation.</returns>
    public Task<Pagination<TResult>> MatchingAsync<TEntity, TResult>(C.Criteria criteria, Expression<Func<TEntity, TResult>> projection, CancellationToken cancellationToken)
        where TEntity : class, IEntityBase
    {
        return this.MatchingAsync(criteria, projection, Guid.Empty, cancellationToken);
    }

    /// <summary>
    /// Finds entities matching the specified criteria and projects them to the specified result type asynchronously.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="criteria">The criteria to match.</param>
    /// <param name="projection">The projection expression.</param>
    /// <param name="tenant">The tenant identifier only for entities that inherit from <see cref="AggregateRoot"/>.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous matching operation.</returns>
    public async Task<Pagination<TResult>> MatchingAsync<TEntity, TResult>(C.Criteria criteria, Expression<Func<TEntity, TResult>> projection, Guid tenant, CancellationToken cancellationToken)
        where TEntity : class, IEntityBase
    {
        var filterExpression = criteria.GetFilterExpression<TEntity>();
        var filter = filterExpression.ToFilterDefinition().BuildFilter(tenant);
        var totalCount = await this.GetCollection<TEntity>().CountDocumentsAsync(filter, cancellationToken: cancellationToken);

        var query = Query<TEntity>(criteria, tenant);
        var data = await query.Project(projection).ToListAsync(cancellationToken);

        return Pagination<TResult>.Create(data, totalCount, criteria.Skip, criteria.Limit);
    }


    /// <summary>
    /// Finds entities matching the specified criteria and projects them to the specified projection type asynchronously.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TProjection">The type of the projection.</typeparam>
    /// <param name="id">The identifier of the entity.</param>
    /// <param name="criteria">The criteria to match.</param>
    /// <param name="projection">The projection expression.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous matching operation.</returns>
    public Task<Pagination<TProjection>> MatchingAsync<TEntity, TProjection>(Guid id, C.Criteria criteria, Expression<Func<TEntity, List<TProjection>>> projection, CancellationToken cancellationToken)
        where TEntity : class, IEntityBase
        where TProjection : class, IEntityBase
    {
        return this.MatchingAsync(id, criteria, projection, Guid.Empty, cancellationToken);
    }

    /// <summary>
    /// Finds entities matching the specified criteria and projects them to the specified projection type asynchronously.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TProjection">The type of the projection.</typeparam>
    /// <param name="id">The identifier of the entity.</param>
    /// <param name="criteria">The criteria to match.</param>
    /// <param name="projection">The projection expression.</param>
    /// <param name="tenant">The tenant identifier only for entities that inherit from <see cref="AggregateRoot"/>.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous matching operation.</returns>
    public async Task<Pagination<TProjection>> MatchingAsync<TEntity, TProjection>(Guid id, C.Criteria criteria, Expression<Func<TEntity, List<TProjection>>> projection, Guid tenant, CancellationToken cancellationToken)
        where TEntity : class, IEntityBase
        where TProjection : class, IEntityBase
    {
        var collection = this.GetCollection<TEntity>();

        var propertyName = GetPropertyName(projection);

        var filterCriteria = criteria.GetFilterExpression<TProjection>();

        var filterDefinition = filterCriteria.ToFilterDefinition(true).BuildFilter(tenant);

        var bsonFilter = ((BsonDocumentFilterDefinition<TProjection>)filterDefinition).Document;

        var pipelineBase = new[]
        {
            new BsonDocument("$match", new BsonDocument("_id", new BsonBinaryData(id, GuidRepresentation.Standard))),
            new BsonDocument("$project", new BsonDocument
            {
                {
                    propertyName, new BsonDocument("$filter", new BsonDocument {
                        { "input", $"${propertyName}" },
                        { "as", "entity" },
                        { "cond", bsonFilter  }
                    })
                }
            }),
            new BsonDocument("$unwind", new BsonDocument("path", $"${propertyName}")),
            new BsonDocument("$replaceRoot", new BsonDocument("newRoot", $"${propertyName}")),
        };

        var pipeline = pipelineBase
            .Concat(
            [
                new BsonDocument("$skip", criteria.Skip ?? 0),
                new BsonDocument("$limit", criteria.Limit ?? 10)
            ])
            .ToList();
        
        var pipelineCount = pipelineBase
            .Concat(
            [
                new BsonDocument("$count", "TotalCount")
            ])
            .ToList();

        var countCursor = await collection.AggregateAsync<BsonDocument>(pipelineCount, cancellationToken: cancellationToken).ConfigureAwait(false);
        var countCursorValue = await countCursor.FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
        var totalCount = countCursorValue["TotalCount"]?.AsInt32 ?? 0;

        var cursor = await collection.AggregateAsync<BsonDocument>(pipeline, cancellationToken: cancellationToken).ConfigureAwait(false);
        var resultList = new List<BsonDocument>();

        while (await cursor.MoveNextAsync(cancellationToken))
        {
            resultList.AddRange(cursor.Current);
        }

        var data = resultList.Select(doc => BsonSerializer.Deserialize<TProjection>(doc)).ToList();

        return Pagination<TProjection>.Create(data, totalCount, criteria.Skip, criteria.Limit);
    }

    /// <summary>
    /// Sorts the query based on the specified sort expression and order type.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="criteria">The criteria containing the sort expression and order type.</param>
    /// <param name="tenant">The tenant identifier only for entities that inherit from <see cref="AggregateRoot"/>.</param>
    /// <returns>The sorted query.</returns>
    private IFindFluent<TEntity, TEntity> Query<TEntity>(C.Criteria criteria, Guid tenant)
        where TEntity : class, IEntityBase
    {
        var collection = this.GetCollection<TEntity>();
        var filterExpression = criteria.GetFilterExpression<TEntity>();
        var sortBy = criteria.GetSortByExpression<TEntity>();

        var filter = filterExpression.ToFilterDefinition().BuildFilter(tenant);

        var query = collection.Find(filter);

        if (sortBy != null)
            if (criteria.OrderType == OrderTypes.Ascending)
                query = query.SortBy(sortBy);
            else
                query = query.SortByDescending(sortBy);

        if (criteria.Skip.HasValue)
        {
            query = query.Skip(criteria.Skip.Value);
        }

        if (criteria.Limit.HasValue)
        {
            query = query.Limit(criteria.Limit.Value);
        }

        return query;
    }

    /// <summary>
    /// Gets the property name from the specified projection expression.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="projection">The projection expression.</param>
    /// <returns>The property name.</returns>
    /// <exception cref="Exceptions.MongoException">Thrown when the expression is not a MemberExpression.</exception>
    private static string GetPropertyName<TEntity, TResult>(Expression<Func<TEntity, TResult>> projection)
        where TEntity : class, IEntityBase
    {
        if (projection.Body is MemberExpression memberExpression)
            return memberExpression.Member.Name;
        else
            throw new Exceptions.MongoException("The expression must be a MemberExpression.");
    }
}