using CodeDesignPlus.Net.Core.Abstractions;
using CodeDesignPlus.Net.EFCore.Abstractions.Operations;
using CodeDesignPlus.Net.EFCore.Repository;
using CodeDesignPlus.Net.Security.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace CodeDesignPlus.Net.EFCore.Operations;

/// <summary>
/// Implementación de las operaciones CRUD estandarizadas en el SDK
/// </summary>
/// <typeparam name="TKey">Type of data that will identify the record</typeparam>
/// <typeparam name="TUserKey">Type of data that the user will identify</typeparam>
/// <typeparam name="TEntity">The entity type to be configured.</typeparam>
public abstract class OperationBase<TKey, TUserKey, TEntity> : RepositoryBase<TKey, TUserKey>, IOperationBase<TKey, TUserKey, TEntity>
    where TEntity : class, IEntityBase<TKey, TUserKey>
{
    /// <summary>
    /// List of properties that will not be updated
    /// </summary>
    private readonly List<string> blacklist = new () {
         nameof(IEntityBase<TKey, TUserKey>.Id),
         nameof(IEntityBase<TKey, TUserKey>.CreatedAt),
         nameof(IEntityBase<TKey, TUserKey>.IdUserCreator)
     };

    /// <summary>
    /// Provide the information of the authenticated user during the request
    /// </summary>
    protected readonly IUserContext<TUserKey> AuthenticateUser;

    /// <summary>
    /// Initializes a new instance of CodeDesignPlus.EFCore.Operations.Operation class using the speciffied options.
    /// </summary>
    /// <param name="authenticatetUser">Information of the authenticated user during the request</param>
    /// <param name="context">Represents a session with the database and can be used to query and save instances of your entities</param>
    protected OperationBase(IUserContext<TUserKey> authenticatetUser, DbContext context) : base(context)
    {
        this.AuthenticateUser = authenticatetUser;
    }

    /// <summary>
    /// Method to create a record in the database
    /// </summary>
    /// <param name="entity">Entity to create</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>Represents an asynchronous operation that can return a value.</returns>
    public virtual async Task<TKey> CreateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        entity.IdUserCreator = this.AuthenticateUser.IdUser;
        entity.CreatedAt = DateTime.UtcNow;

        entity = await base.CreateAsync(entity, cancellationToken);

        return entity.Id;
    }

    /// <summary>
    /// Method that deletes a record in the database
    /// </summary>
    /// <param name="id">Id of the record to delete</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>Represents an asynchronous operation that can return a value.</returns>
    public virtual Task<bool> DeleteAsync(TKey id, CancellationToken cancellationToken = default)
    {
        return base.DeleteAsync<TEntity>(x => x.Id.Equals(id), cancellationToken);
    }

    /// <summary>
    /// Method that updates a record in the database
    /// </summary>
    /// <param name="id">Id of the record to update</param>
    /// <param name="entity">Entity with the information to update</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>Represents an asynchronous operation that can return a value.</returns>
    public virtual async Task<bool> UpdateAsync(TKey id, TEntity entity, CancellationToken cancellationToken = default)
    {
        var entityUpdated = await base.GetEntity<TEntity>().FindAsync(id);

        if (entityUpdated != null)
        {
            var properties = typeof(TEntity).GetProperties().Where(x => !this.blacklist.Contains(x.Name)).ToList();

            foreach (var property in properties)
            {
                var value = entity.GetType().GetProperty(property.Name).GetValue(entity);

                if (value != null)
                    entityUpdated.GetType().GetProperty(property.Name).SetValue(entityUpdated, value, null);
            }

            return await base.Context.SaveChangesAsync() > 0;
        }

        return false;
    }
}