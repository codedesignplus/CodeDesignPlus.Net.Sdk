using CodeDesignPlus.Net.Core.Abstractions;
using CodeDesignPlus.Net.Mongo.Abstractions.Operations;
using CodeDesignPlus.Net.Mongo.Abstractions.Options;
using CodeDesignPlus.Net.Mongo.Repository;
using CodeDesignPlus.Net.Security.Abstractions;
using MongoDB.Driver;

namespace CodeDesignPlus.Net.Mongo;

/// <summary>
/// Base class that provides the basic operations to perform on a database
/// </summary>
/// <typeparam name="TKey">Type of data that will identify the record</typeparam>
/// <typeparam name="TUserKey">Type of data that the user will identify</typeparam>
/// <typeparam name="TEntity">The entity type to be configured.</typeparam>
public abstract class OperationBase<TEntity> : RepositoryBase, IOperationBase<TEntity>
    where TEntity : class, IEntityBase
{
    /// <summary>
    /// List of properties that will not be updated
    /// </summary>
    private readonly List<string> blacklist = [
        nameof(IEntityBase.Id),
        nameof(IEntity.CreatedAt),
        nameof(IEntity.CreatedBy),
        nameof(IEntity.UpdatedAt),
        nameof(IEntity.UpdatedBy)
     ];

    /// <summary>
    /// Provide the information of the authenticated user during the request
    /// </summary>
    protected readonly IUserContext AuthenticateUser;

    /// <summary>
    /// Initializes a new instance of CodeDesignPlus.EFCore.Operations.Operation class using the speciffied options.
    /// </summary>
    /// <param name="authenticatetUser">Information of the authenticated user during the request</param>
    protected OperationBase(IUserContext authenticatetUser, IServiceProvider serviceProvider, IOptions<MongoOptions> options, ILogger<RepositoryBase> logger)
        : base(serviceProvider, options, logger)
    {
        this.AuthenticateUser = authenticatetUser;
    }

    /// <summary>
    /// Method that creates a record in the database
    /// </summary>
    /// <param name="entity">Entity to create</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>Represents an asynchronous operation that can return a value.</returns>
    public virtual Task CreateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        if (entity is IEntity auditTrailEntity)
        {
            auditTrailEntity.CreatedBy = this.AuthenticateUser.IdUser;
            auditTrailEntity.CreatedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }

        return base.CreateAsync(entity, cancellationToken);
    }

    /// <summary>
    /// Method that deletes a record in the database
    /// </summary>
    /// <param name="id">Id of the record to delete</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>Represents an asynchronous operation that can return a value.</returns>
    public virtual Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var filter = Builders<TEntity>.Filter.Eq(x => x.Id, id);

        return base.DeleteAsync<TEntity>(filter, cancellationToken);
    }

    /// <summary>
    /// Method that updates a record in the database
    /// </summary>
    /// <param name="id">Id of the record to update</param>
    /// <param name="entity">Entity with the information to update</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>Represents an asynchronous operation that can return a value.</returns>
    public virtual async Task UpdateAsync(Guid id, TEntity entity, CancellationToken cancellationToken = default)
    {
        var filter = Builders<TEntity>.Filter.Eq(x => x.Id, id);

        var updates = new List<UpdateDefinition<TEntity>>();

        var properties = typeof(TEntity).GetProperties().Where(x => !this.blacklist.Contains(x.Name)).ToList();

        foreach (var property in properties)
        {
            var value = entity.GetType().GetProperty(property.Name).GetValue(entity);

            if (value != null)
            {
                updates.Add(Builders<TEntity>.Update.Set(property.Name, value));
            }
        }

        var update = Builders<TEntity>.Update.Combine(updates);
        var result = await base.GetCollection<TEntity>().UpdateOneAsync(filter, update, cancellationToken: cancellationToken);
    }
}