using CodeDesignPlus.Net.Core.Abstractions;
using CodeDesignPlus.Net.EFCore.Abstractions.Operations;
using CodeDesignPlus.Net.EFCore.Repository;
using CodeDesignPlus.Net.Security.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace CodeDesignPlus.Net.EFCore.Operations;

/// <summary>
/// It allows the repository to create, update and delete a record by assigning the information to the transversal properties of the entity
/// </summary>
/// <typeparam name="TEntity">The entity type to be configured.</typeparam>
/// <param name="authenticatetUser">Information of the authenticated user during the request</param>
/// <param name="context">Represents a session with the database and can be used to query and save instances of your entities</param>
public abstract class OperationBase<TEntity>(
    IUserContext authenticatetUser,
    DbContext context
)
: RepositoryBase(context), IOperationBase<TEntity>
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
    protected readonly IUserContext AuthenticateUser = authenticatetUser;

    /// <summary>
    /// Method to create a record in the database
    /// </summary>
    /// <param name="entity">Entity to create</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>Represents an asynchronous operation.</returns>
    public virtual async Task CreateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        if (entity is IEntity auditTrailEntity)
        {
            auditTrailEntity.CreatedBy = this.AuthenticateUser.IdUser;
            auditTrailEntity.CreatedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }

        await base.CreateAsync(entity, cancellationToken);
    }

    /// <summary>
    /// Method that deletes a record in the database
    /// </summary>
    /// <param name="id">Id of the record to delete</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>Represents an asynchronous operation.</returns>
    public virtual Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return base.DeleteAsync<TEntity>(x => x.Id.Equals(id), cancellationToken);
    }

    /// <summary>
    /// Method that updates a record in the database
    /// </summary>
    /// <param name="id">Id of the record to update</param>
    /// <param name="entity">Entity with the information to update</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>Represents an asynchronous operation.</returns>
    public virtual async Task UpdateAsync(Guid id, TEntity entity, CancellationToken cancellationToken = default)
    {
        var entityUpdated = await base.GetEntity<TEntity>().FindAsync([id], cancellationToken: cancellationToken);

        if (entityUpdated != null)
        {
            var properties = typeof(TEntity).GetProperties().Where(x => !this.blacklist.Contains(x.Name)).ToList();

            foreach (var property in properties)
            {
                var value = entity.GetType().GetProperty(property.Name).GetValue(entity);

                if (value != null)
                    entityUpdated.GetType().GetProperty(property.Name).SetValue(entityUpdated, value, null);
            }

            await base.Context.SaveChangesAsync(cancellationToken);
        }
    }
}