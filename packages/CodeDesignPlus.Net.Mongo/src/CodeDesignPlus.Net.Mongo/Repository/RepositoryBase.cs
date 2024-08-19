
namespace CodeDesignPlus.Net.Mongo.Repository;

/// <summary>
/// Base class for MongoDB repository operations.
/// </summary>
public abstract class RepositoryBase : IRepositoryBase
{
    private readonly IServiceProvider serviceProvider;
    private readonly ILogger logger;
    private readonly MongoOptions mongoOptions;

    /// <summary>
    /// Initializes a new instance of the <see cref="RepositoryBase"/> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    /// <param name="mongoOptions">The MongoDB options.</param>
    /// <param name="logger">The logger instance.</param>
    /// <exception cref="ArgumentNullException">Thrown when any of the parameters are null.</exception>
    protected RepositoryBase(IServiceProvider serviceProvider, IOptions<MongoOptions> mongoOptions, ILogger logger)
    {
        this.serviceProvider = serviceProvider;
        this.logger = logger;
        this.mongoOptions = mongoOptions.Value;
    }

    /// <summary>
    /// Gets the MongoDB collection for the specified entity type.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <returns>The MongoDB collection.</returns>
    public IMongoCollection<TEntity> GetCollection<TEntity>()
        where TEntity : class, IEntityBase
    {
        var client = this.serviceProvider.GetRequiredService<IMongoClient>();

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
        var collection = this.GetCollection<TEntity>();

        var filter = Builders<TEntity>.Filter.Eq(e => e.Id, id);
        var update = Builders<TEntity>.Update
            .Set(e => e.IsActive, state)
            .Set(e => e.UpdatedAt, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());

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
    /// Deletes an entity by its filter asynchronously.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="filter">The filter definition.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous delete operation.</returns>
    public Task DeleteAsync<TEntity>(FilterDefinition<TEntity> filter, CancellationToken cancellationToken)
        where TEntity : class, IEntityBase
    {
        var collection = this.GetCollection<TEntity>();

        return collection.DeleteOneAsync(filter, cancellationToken);
    }

    /// <summary>
    /// Deletes a range of entities asynchronously.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="entities">The entities to delete.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous delete range operation.</returns>
    public async Task DeleteRangeAsync<TEntity>(List<TEntity> entities, CancellationToken cancellationToken)
        where TEntity : class, IEntityBase
    {
        foreach (var entity in entities)
        {
            var filter = Builders<TEntity>.Filter.Eq(e => e.Id, entity.Id);

            await this.DeleteAsync(filter, cancellationToken);
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
        var client = this.serviceProvider.GetRequiredService<IMongoClient>();

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

            this.logger.LogError(ex, "Failed to execute transaction");
        }
    }

    /// <summary>
    /// Finds entities matching the specified criteria asynchronously.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="criteria">The criteria to match.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous matching operation.</returns>
    public Task<List<TEntity>> MatchingAsync<TEntity>(C.Criteria criteria, CancellationToken cancellationToken)
        where TEntity : class, IEntityBase
    {
        var query = Query<TEntity>(criteria);

        return query.ToListAsync(cancellationToken);
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
    public Task<List<TResult>> MatchingAsync<TEntity, TResult>(C.Criteria criteria, Expression<Func<TEntity, TResult>> projection, CancellationToken cancellationToken)
        where TEntity : class, IEntityBase
    {
        var query = Query<TEntity>(criteria);

        return query.Project(projection).ToListAsync(cancellationToken);
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
    public async Task<List<TProjection>> MatchingAsync<TEntity, TProjection>(Guid id, C.Criteria criteria, Expression<Func<TEntity, List<TProjection>>> projection, CancellationToken cancellationToken)
        where TEntity : class, IEntityBase
        where TProjection : class, IEntityBase
    {
        var collection = this.GetCollection<TEntity>();

        var propertyName = GetPropertyName(projection);

        var filterCriteria = criteria.GetFilterExpression<TProjection>();

        var bsonFilter = GetBsonDocument(filterCriteria, "entity");

        var pipeline = new[]
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
            new BsonDocument("$replaceRoot", new BsonDocument("newRoot", $"${propertyName}"))
        };

        var cursor = await collection.AggregateAsync<BsonDocument>(pipeline, cancellationToken: cancellationToken).ConfigureAwait(false);
        var resultList = new List<BsonDocument>();

        while (await cursor.MoveNextAsync(cancellationToken))
        {
            resultList.AddRange(cursor.Current);
        }

        return resultList.Select(doc => BsonSerializer.Deserialize<TProjection>(doc)).ToList();
    }

    /// <summary>
    /// Sorts the query based on the specified sort expression and order type.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="criteria">The criteria containing the sort expression and order type.</param>
    /// <returns>The sorted query.</returns>
    private IFindFluent<TEntity, TEntity> Query<TEntity>(C.Criteria criteria)
        where TEntity : class, IEntityBase
    {
        var collection = this.GetCollection<TEntity>();
        var filter = criteria.GetFilterExpression<TEntity>();
        var sortBy = criteria.GetSortByExpression<TEntity>();

        var query = collection.Find(filter);

        if (sortBy != null)
            if (criteria.OrderType == OrderTypes.Ascending)
                query = query.SortBy(sortBy);
            else
                query = query.SortByDescending(sortBy);

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

    /// <summary>
    /// Converts the specified expression to a BSON document.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="expression">The expression to convert.</param>
    /// <param name="alias">The alias to use in the BSON document.</param>
    /// <returns>The BSON document representing the expression.</returns>
    public static BsonDocument GetBsonDocument<TEntity>(Expression<Func<TEntity, bool>> expression, string alias)
        where TEntity : class, IEntityBase
    {
        var parameter = expression.Parameters[0];

        var converter = new ExpressionConverter(parameter, alias);

        return converter.Convert(expression.Body);
    }

}