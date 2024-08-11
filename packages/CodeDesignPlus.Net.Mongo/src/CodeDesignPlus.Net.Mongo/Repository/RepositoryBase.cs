
namespace CodeDesignPlus.Net.Mongo.Repository;

/// <summary>
/// Represents a base repository class for MongoDB operations.
/// </summary>
public abstract class RepositoryBase : IRepositoryBase
{
    private readonly IServiceProvider serviceProvider;
    private readonly ILogger logger;
    private readonly MongoOptions mongoOptions;

    /// <summary>
    /// Initializes a new instance of the RepositoryBase class using the specified options.
    /// </summary>
    /// <param name="serviceProvider">The IServiceProvider to add services to the container.</param>
    /// <param name="mongoOptions">The options to configure a MongoDB.</param>
    /// <param name="logger">Represents a type used to perform logging.</param>
    protected RepositoryBase(IServiceProvider serviceProvider, IOptions<MongoOptions> mongoOptions, ILogger logger)
    {
        this.serviceProvider = serviceProvider;
        this.logger = logger;
        this.mongoOptions = mongoOptions.Value;
    }

    /// <summary>
    /// Method that returns a list of records from the database.
    /// </summary>
    /// <typeparam name="TEntity">The entity type to be configured.</typeparam>
    /// <returns>Represents an asynchronous operation</returns>
    public IMongoCollection<TEntity> GetCollection<TEntity>()
        where TEntity : class, IEntityBase
    {
        var client = this.serviceProvider.GetRequiredService<IMongoClient>();

        var database = client.GetDatabase(this.mongoOptions.Database);

        return database.GetCollection<TEntity>(typeof(TEntity).Name);
    }

    /// <summary>
    /// Changes the state of a record in the database.
    /// </summary>
    /// <typeparam name="TEntity">The entity type to be configured.</typeparam>
    /// <param name="id">The ID of the record to search.</param>
    /// <param name="state">The state of the record to set.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>Represents an asynchronous operation</returns>
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
    /// Creates a new record in the database.
    /// </summary>
    /// <typeparam name="TEntity">The entity type to be configured.</typeparam>
    /// <param name="entity">The entity to be created.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>Represents an asynchronous operation</returns>
    public Task CreateAsync<TEntity>(TEntity entity, CancellationToken cancellationToken)
        where TEntity : class, IEntityBase
    {
        var collection = this.GetCollection<TEntity>();

        return collection.InsertOneAsync(entity, cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Creates multiple records in the database.
    /// </summary>
    /// <typeparam name="TEntity">The entity type to be configured.</typeparam>
    /// <param name="entities">The list of entities to be created.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>Represents an asynchronous operation</returns>
    public Task CreateRangeAsync<TEntity>(List<TEntity> entities, CancellationToken cancellationToken)
        where TEntity : class, IEntityBase
    {
        var collection = this.GetCollection<TEntity>();

        return collection.InsertManyAsync(entities, cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Deletes a record from the database.
    /// </summary>
    /// <typeparam name="TEntity">The entity type to be configured.</typeparam>
    /// <param name="filter">The filter to search for the record to delete.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>Represents an asynchronous operation</returns>
    public Task DeleteAsync<TEntity>(FilterDefinition<TEntity> filter, CancellationToken cancellationToken)
        where TEntity : class, IEntityBase
    {
        var collection = this.GetCollection<TEntity>();

        return collection.DeleteOneAsync(filter, cancellationToken);
    }

    /// <summary>
    /// Deletes multiple records from the database.
    /// </summary>
    /// <typeparam name="TEntity">The entity type to be configured.</typeparam>
    /// <param name="entities">The list of entities to be deleted.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>Represents an asynchronous operation</returns>
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
    /// Updates a record in the database.
    /// </summary>
    /// <typeparam name="TEntity">The entity type to be configured.</typeparam>
    /// <param name="entity">The entity to be updated.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>Represents an asynchronous operation</returns>
    public Task UpdateAsync<TEntity>(TEntity entity, CancellationToken cancellationToken)
        where TEntity : class, IEntityBase
    {
        var collection = this.GetCollection<TEntity>();

        FilterDefinition<TEntity> filter = Builders<TEntity>.Filter.Eq(e => e.Id, entity.Id);

        return collection.ReplaceOneAsync(filter, entity, cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Updates multiple records in the database.
    /// </summary>
    /// <typeparam name="TEntity">The entity type to be configured.</typeparam>
    /// <param name="entities">The list of entities to be updated.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>Represents an asynchronous operation</returns>
    public async Task UpdateRangeAsync<TEntity>(List<TEntity> entities, CancellationToken cancellationToken)
        where TEntity : class, IEntityBase
    {
        foreach (var entity in entities)
        {
            await this.UpdateAsync(entity, cancellationToken);
        }
    }

    /// <summary>
    /// Executes a transactional operation on the MongoDB database.
    /// </summary>
    /// <param name="process">The function that represents the transactional operation.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>Represents an asynchronous operation</returns>
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
    /// Method that returns a list of records from the database.
    /// </summary>
    /// <typeparam name="TEntity">The entity type to be configured.</typeparam>
    /// <param name="criteria">The criteria to filter the records.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>Represents an asynchronous operation</returns>    
    public Task<List<TEntity>> MatchingAsync<TEntity>(C.Criteria criteria, CancellationToken cancellationToken)
        where TEntity : class, IEntityBase
    {
        var query = Query<TEntity>(criteria);

        return query.ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Method that returns a list of records from the database.
    /// </summary>
    /// <typeparam name="TEntity">The entity type to be configured.</typeparam>
    /// <typeparam name="TResult">The type of the result to project.</typeparam>
    /// <param name="criteria">The criteria to filter the records.</param>
    /// <param name="projection">The projection to apply to the records.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>Represents an asynchronous operation</returns>
    public Task<List<TResult>> MatchingAsync<TEntity, TResult>(C.Criteria criteria, Expression<Func<TEntity, TResult>> projection, CancellationToken cancellationToken)
        where TEntity : class, IEntityBase
    {
        var query = Query<TEntity>(criteria);

        return query.Project(projection).ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Method that returns a list of records from the database.
    /// </summary>
    /// <typeparam name="TEntity">The entity type to be configured.</typeparam>
    /// <param name="id">The ID of the record to search.</param>
    /// <param name="criteria">The criteria to filter the records.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>Represents an asynchronous operation</returns>
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

    private static string GetPropertyName<TEntity, TResult>(Expression<Func<TEntity, TResult>> projection)
        where TEntity : class, IEntityBase
    {
        if (projection.Body is MemberExpression memberExpression)
            return memberExpression.Member.Name;
        else
            throw new Exceptions.MongoException("The expression must be a MemberExpression.");
    }

    public static BsonDocument GetBsonDocument<TEntity>(Expression<Func<TEntity, bool>> expression, string alias)
        where TEntity : class, IEntityBase
    {
        var parameter = expression.Parameters[0];

        var converter = new ExpressionConverter(parameter, alias);

        return converter.Convert(expression.Body);
    }

}