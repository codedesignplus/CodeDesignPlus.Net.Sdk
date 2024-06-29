using CodeDesignPlus.Net.Core.Abstractions;

namespace CodeDesignPlus.Net.EFCore.Abstractions.Operations;

/// <summary>
/// Allows the repository to update a record by assigning the information to the transversal properties of the entity
/// </summary>
/// <typeparam name="TEntity">Type of entity to update</typeparam>
public interface IUpdateOperation<in TEntity> where TEntity : class, IEntityBase
{
    /// <summary>
    /// Method that updates a record in the database
    /// </summary>
    /// <param name="id">Id of the record to update</param>
    /// <param name="entity">Entity with the information to update</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>Represents an asynchronous operation.</returns>
    Task UpdateAsync(Guid id, TEntity entity, CancellationToken cancellationToken = default);
}