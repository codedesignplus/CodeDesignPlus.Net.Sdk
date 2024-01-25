using CodeDesignPlus.Net.Core.Abstractions;

namespace CodeDesignPlus.Net.EFCore.Abstractions.Operations;

/// <summary>
/// Allows the repository to delete a record by assigning the information to the transversal properties of the entity
/// </summary>
/// <typeparam name="TEntity">Type of entity to delete</typeparam>
public interface IDeleteOperation<TEntity> where TEntity : class, IEntityBase
{
    /// <summary>
    /// Method that deletes a record in the database
    /// </summary>
    /// <param name="id">Id of the record to delete</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>Represents an asynchronous operation.</returns>
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}