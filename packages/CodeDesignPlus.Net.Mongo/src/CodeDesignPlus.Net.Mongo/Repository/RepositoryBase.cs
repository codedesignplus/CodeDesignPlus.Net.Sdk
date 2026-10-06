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
    /// This method bypasses automatic tenant and soft-delete filters, providing direct access to all documents.
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

        if (entity is IVersionedAggregate versioned)
        {
            return InsertVersionedAsync(collection, entity, versioned, translateDuplicateKey: false, cancellationToken);
        }

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

        if (typeof(IVersionedAggregate).IsAssignableFrom(typeof(TEntity)))
        {
            return CreateRangeVersionedAsync(collection, entities, cancellationToken);
        }

        return collection.InsertManyAsync(entities, cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Inserts versioned aggregates one version up and leaves the in-memory versions as they were if the insert fails.
    /// </summary>
    private static async Task CreateRangeVersionedAsync<TEntity>(IMongoCollection<TEntity> collection, List<TEntity> entities, CancellationToken cancellationToken)
        where TEntity : class, IEntityBase
    {
        var read = entities.Select(entity => ((IVersionedAggregate)entity).Version).ToList();

        for (var i = 0; i < entities.Count; i++)
        {
            VersionAccessor.Set((IVersionedAggregate)entities[i], read[i] + 1);
        }

        try
        {
            await collection.InsertManyAsync(entities, cancellationToken: cancellationToken);
        }
        catch
        {
            for (var i = 0; i < entities.Count; i++)
            {
                VersionAccessor.Set((IVersionedAggregate)entities[i], read[i]);
            }

            throw;
        }
    }

    /// <summary>
    /// Creates a new entity or replaces it if one with the same Id already exists (upsert).
    /// Useful for event-driven projections where the order of Create/Update events is not guaranteed.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="entity">The entity to create or replace.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous upsert operation.</returns>
    /// <remarks>
    /// For an <see cref="IVersionedAggregate"/> a new aggregate (version 0) is inserted and a duplicate key becomes a
    /// <see cref="ConcurrencyConflictException"/>: replacing would erase what another writer created first. Any other
    /// versioned aggregate is saved as <see cref="UpdateAsync{TEntity}(TEntity, CancellationToken)"/> does.
    /// </remarks>
    /// <exception cref="ConcurrencyConflictException">Thrown for a versioned aggregate whose write found another writer first.</exception>
    public Task UpsertAsync<TEntity>(TEntity entity, CancellationToken cancellationToken)
        where TEntity : class, IEntityBase
    {
        var collection = this.GetCollection<TEntity>();

        if (entity is IVersionedAggregate versioned)
        {
            return SaveVersionedAsync(collection, entity, versioned, insertWhenNew: true, cancellationToken);
        }

        var filter = Builders<TEntity>.Filter.Eq(e => e.Id, entity.Id);
        var options = new ReplaceOptions { IsUpsert = true };

        return collection.ReplaceOneAsync(filter, entity, options, cancellationToken);
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
    /// Deletes every document of the entity that belongs to the tenant, in a single operation.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="tenant">The tenant whose documents are deleted. It cannot be empty.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The number of deleted documents.</returns>
    /// <exception cref="Exceptions.MongoException">Thrown when <paramref name="tenant"/> is empty.</exception>
    public async Task<long> DeleteByTenantAsync<TEntity>(Guid tenant, CancellationToken cancellationToken)
        where TEntity : class, IEntityBase
    {
        // An empty tenant would match nothing useful and, on a typo, everything scoped to Guid.Empty: refuse it.
        if (tenant == Guid.Empty)
        {
            throw new Exceptions.MongoException("The tenant cannot be empty.");
        }

        var collection = this.GetCollection<TEntity>();

        // By field name, not by AggregateRoot.Tenant: some entities declare their own tenant property.
        var filter = Builders<TEntity>.Filter.Eq("Tenant", tenant);

        var result = await collection.DeleteManyAsync(filter, cancellationToken);

        return result.DeletedCount;
    }

    /// <summary>
    /// Updates an entity asynchronously. Soft-deleted entities (IsDeleted == true) are not updated.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="entity">The entity to update.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous update operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the entity is null.</exception>
    /// <remarks>
    /// For an <see cref="IVersionedAggregate"/> the replace also filters by the version that was read and stores it
    /// raised by one. If nothing matched, someone else wrote first (or the document is gone) and the write is refused
    /// instead of silently erasing the other change.
    /// </remarks>
    /// <exception cref="ConcurrencyConflictException">Thrown for a versioned aggregate that changed, or no longer exists, since it was read.</exception>
    public Task UpdateAsync<TEntity>(TEntity entity, CancellationToken cancellationToken)
        where TEntity : class, IEntityBase
    {
        var collection = this.GetCollection<TEntity>();

        if (entity is IVersionedAggregate versioned)
        {
            return SaveVersionedAsync(collection, entity, versioned, insertWhenNew: false, cancellationToken);
        }

        FilterDefinition<TEntity> filter = Builders<TEntity>.Filter.Eq(e => e.Id, entity.Id);

        if (typeof(IEntity).IsAssignableFrom(typeof(TEntity)))
        {
            filter = Builders<TEntity>.Filter.And(filter, Builders<TEntity>.Filter.Eq("IsDeleted", false));
        }

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
    /// Reads a versioned aggregate, applies a change and saves it; on a concurrency conflict it reads again and
    /// reapplies the change, up to <see cref="MongoOptions.ConcurrencyMaxAttempts"/> times.
    /// </summary>
    /// <typeparam name="TEntity">The type of the versioned aggregate.</typeparam>
    /// <param name="load">Reads the current aggregate; it may return a new one (version 0) to create, or null to skip.</param>
    /// <param name="mutate">Applies the change and returns false when it was already applied, so nothing is written.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>True when the aggregate was written; false when <paramref name="load"/> found nothing or <paramref name="mutate"/> declined.</returns>
    /// <exception cref="ConcurrencyConflictException">Thrown when every attempt conflicted.</exception>
    public Task<bool> UpdateWithRetryAsync<TEntity>(Func<Task<TEntity>> load, Func<TEntity, bool> mutate, CancellationToken cancellationToken)
        where TEntity : class, IEntityBase, IVersionedAggregate
    {
        return this.UpdateWithRetryAsync(load, mutate, this.mongoOptions.ConcurrencyMaxAttempts, cancellationToken);
    }

    /// <summary>
    /// Reads a versioned aggregate, applies a change and saves it; on a concurrency conflict it reads again and
    /// reapplies the change, up to <paramref name="maxAttempts"/> times.
    /// </summary>
    /// <typeparam name="TEntity">The type of the versioned aggregate.</typeparam>
    /// <param name="load">Reads the current aggregate; it may return a new one (version 0) to create, or null to skip.</param>
    /// <param name="mutate">Applies the change and returns false when it was already applied, so nothing is written.</param>
    /// <param name="maxAttempts">How many times to read, reapply and save before giving up.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>True when the aggregate was written; false when <paramref name="load"/> found nothing or <paramref name="mutate"/> declined.</returns>
    /// <exception cref="ConcurrencyConflictException">Thrown when every attempt conflicted.</exception>
    /// <remarks>
    /// <paramref name="mutate"/> runs once per attempt, always on a freshly read aggregate, so it must be a pure change of
    /// that aggregate: anything it computes is only valid from the attempt that was saved. The wait between attempts is
    /// random so that writers that collided do not collide again in step.
    /// </remarks>
    public async Task<bool> UpdateWithRetryAsync<TEntity>(Func<Task<TEntity>> load, Func<TEntity, bool> mutate, int maxAttempts, CancellationToken cancellationToken)
        where TEntity : class, IEntityBase, IVersionedAggregate
    {
        ArgumentNullException.ThrowIfNull(load);
        ArgumentNullException.ThrowIfNull(mutate);

        if (maxAttempts < 1)
        {
            throw new Exceptions.MongoException("The number of attempts must be at least 1.");
        }

        var collection = this.GetCollection<TEntity>();

        for (var attempt = 1; ; attempt++)
        {
            var entity = await load();

            if (entity is null || !mutate(entity))
            {
                return false;
            }

            try
            {
                await SaveVersionedAsync(collection, entity, entity, insertWhenNew: true, cancellationToken);

                return true;
            }
            catch (ConcurrencyConflictException exception) when (attempt < maxAttempts)
            {
                logger.LogInformation("Concurrency conflict on {Entity} {Id} at version {Version}; retrying ({Attempt}/{MaxAttempts}).", exception.EntityType, exception.EntityId, exception.ExpectedVersion, attempt, maxAttempts);

                var bound = this.mongoOptions.ConcurrencyRetryDelayMilliseconds * attempt;

                if (bound > 0)
                {
                    await Task.Delay(Random.Shared.Next(bound + 1), cancellationToken);
                }
            }
        }
    }

    /// <summary>
    /// Saves a versioned aggregate checking the version it was read with, and raises that version by one.
    /// </summary>
    private static async Task SaveVersionedAsync<TEntity>(IMongoCollection<TEntity> collection, TEntity entity, IVersionedAggregate versioned, bool insertWhenNew, CancellationToken cancellationToken)
        where TEntity : class, IEntityBase
    {
        var read = versioned.Version;

        if (read == 0 && insertWhenNew)
        {
            try
            {
                await InsertVersionedAsync(collection, entity, versioned, translateDuplicateKey: true, cancellationToken);

                return;
            }
            catch (ConcurrencyConflictException)
            {
                // The id already exists. If it is a document saved before the aggregate was versioned, it has no Version and
                // reads as 0 like a new one: the replace below takes it. If another writer created it, it is at version 1
                // or more and the replace refuses it as a conflict.
            }
        }

        var filter = Builders<TEntity>.Filter.And(Builders<TEntity>.Filter.Eq(e => e.Id, entity.Id), VersionFilter<TEntity>(read));

        if (typeof(IEntity).IsAssignableFrom(typeof(TEntity)))
        {
            filter = Builders<TEntity>.Filter.And(filter, Builders<TEntity>.Filter.Eq("IsDeleted", false));
        }

        VersionAccessor.Set(versioned, read + 1);

        ReplaceOneResult result;

        try
        {
            result = await collection.ReplaceOneAsync(filter, entity, cancellationToken: cancellationToken);
        }
        catch
        {
            VersionAccessor.Set(versioned, read);

            throw;
        }

        if (result.MatchedCount == 0)
        {
            VersionAccessor.Set(versioned, read);

            throw new ConcurrencyConflictException(typeof(TEntity), entity.Id, read);
        }
    }

    /// <summary>
    /// Inserts a versioned aggregate one version above the one it carries.
    /// </summary>
    private static async Task InsertVersionedAsync<TEntity>(IMongoCollection<TEntity> collection, TEntity entity, IVersionedAggregate versioned, bool translateDuplicateKey, CancellationToken cancellationToken)
        where TEntity : class, IEntityBase
    {
        var read = versioned.Version;

        VersionAccessor.Set(versioned, read + 1);

        try
        {
            await collection.InsertOneAsync(entity, cancellationToken: cancellationToken);
        }
        catch (MongoWriteException exception) when (translateDuplicateKey && exception.WriteError?.Category == ServerErrorCategory.DuplicateKey)
        {
            VersionAccessor.Set(versioned, read);

            // Same id or same unique key: another writer created it first. Reading again finds theirs.
            throw new ConcurrencyConflictException(typeof(TEntity), entity.Id, read, exception);
        }
        catch
        {
            VersionAccessor.Set(versioned, read);

            throw;
        }
    }

    /// <summary>
    /// Matches the stored version; a document saved before the aggregate was versioned has no field and counts as 0.
    /// </summary>
    private static FilterDefinition<TEntity> VersionFilter<TEntity>(long version)
    {
        var builder = Builders<TEntity>.Filter;

        if (version == 0)
        {
            return builder.Or(builder.Eq(VersionAccessor.Field, 0L), builder.Exists(VersionAccessor.Field, false));
        }

        return builder.Eq(VersionAccessor.Field, version);
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

        return Pagination<TEntity>.Create(data, totalCount, limit: criteria.Limit, skip: criteria.Skip);
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

        return Pagination<TResult>.Create(data, totalCount, limit: criteria.Limit, skip: criteria.Skip);
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

        var filterDefinition = filterCriteria.ToFilterDefinition(true);

        var bsonFilter = ((BsonDocumentFilterDefinition<TProjection>)filterDefinition).Document;

        if (typeof(IEntity).IsAssignableFrom(typeof(TProjection)))
        {
            bsonFilter = new BsonDocument("$and", new BsonArray
            {
                bsonFilter,
                new BsonDocument("$eq", new BsonArray { "$$entity.IsDeleted", false })
            });
        }

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

        return Pagination<TProjection>.Create(data, totalCount, limit: criteria.Limit, skip: criteria.Skip);
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