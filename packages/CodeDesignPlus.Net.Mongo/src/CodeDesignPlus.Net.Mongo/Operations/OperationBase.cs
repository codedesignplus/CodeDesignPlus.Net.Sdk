namespace CodeDesignPlus.Net.Mongo.Operations;

/// <summary>
/// Base class for MongoDB operations.
/// </summary>
/// <typeparam name="TEntity">The type of the entity.</typeparam>
public abstract class OperationBase<TEntity> : RepositoryBase, IOperationBase<TEntity>
    where TEntity : class, IEntityBase
{
    private readonly List<string> blacklist = new List<string>
    {
        nameof(IEntityBase.Id),
        nameof(IEntity.CreatedAt),
        nameof(IEntity.CreatedBy),
        nameof(IEntity.UpdatedAt),
        nameof(IEntity.UpdatedBy)
    };

    /// <summary>
    /// The authenticated user context.
    /// </summary>
    protected readonly IUserContext AuthenticateUser;

    /// <summary>
    /// Initializes a new instance of the <see cref="OperationBase{TEntity}"/> class.
    /// </summary>
    /// <param name="authenticatetUser">The authenticated user context.</param>
    /// <param name="serviceProvider">The service provider.</param>
    /// <param name="options">The MongoDB options.</param>
    /// <param name="logger">The logger instance.</param>
    /// <exception cref="ArgumentNullException">Thrown when any of the parameters are null.</exception>
    protected OperationBase(IUserContext authenticatetUser, IServiceProvider serviceProvider, IOptions<MongoOptions> options, ILogger logger)
         : base(serviceProvider, options, logger)
    {
        ArgumentNullException.ThrowIfNull(authenticatetUser);

        AuthenticateUser = authenticatetUser;
    }

    /// <summary>
    /// Creates a new entity asynchronously.
    /// </summary>
    /// <param name="entity">The entity to create.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous create operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the entity is null.</exception>
    public virtual Task CreateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        if (entity is IEntity auditTrailEntity)
        {
            auditTrailEntity.CreatedBy = AuthenticateUser.IdUser;
            auditTrailEntity.CreatedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }

        return base.CreateAsync(entity, cancellationToken);
    }

    /// <summary>
    /// Deletes an entity by its identifier asynchronously.
    /// </summary>
    /// <param name="id">The identifier of the entity to delete.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous delete operation.</returns>
    public virtual Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var filter = Builders<TEntity>.Filter.Eq(x => x.Id, id);

        return DeleteAsync(filter, cancellationToken);
    }

    /// <summary>
    /// Updates an entity by its identifier asynchronously.
    /// </summary>
    /// <param name="id">The identifier of the entity to update.</param>
    /// <param name="entity">The entity with updated values.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous update operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the entity is null.</exception>
    public virtual async Task UpdateAsync(Guid id, TEntity entity, CancellationToken cancellationToken = default)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        var filter = Builders<TEntity>.Filter.Eq(x => x.Id, id);

        var updates = new List<UpdateDefinition<TEntity>>();

        var properties = typeof(TEntity).GetProperties().Where(x => !blacklist.Contains(x.Name)).ToList();

        foreach (var property in properties.Select(property => property.Name))
        {
            var value = entity.GetType().GetProperty(property).GetValue(entity);

            if (value != null)
            {
                updates.Add(Builders<TEntity>.Update.Set(property, value));
            }
        }

        var update = Builders<TEntity>.Update.Combine(updates);
        await GetCollection<TEntity>().UpdateOneAsync(filter, update, cancellationToken: cancellationToken);
    }
}