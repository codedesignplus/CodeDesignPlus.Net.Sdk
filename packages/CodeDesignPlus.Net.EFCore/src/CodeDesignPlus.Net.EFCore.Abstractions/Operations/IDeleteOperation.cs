using CodeDesignPlus.Net.Core.Abstractions;

namespace CodeDesignPlus.Net.EFCore.Abstractions.Operations;

/// <summary>
/// Allows the repository to delete a record by assigning the information to the transversal properties of the entity
/// </summary>
/// <typeparam name="TKey">Type of data that will identify the record</typeparam>
/// <typeparam name="TUserKey">Type of data that the user will identify</typeparam>
/// <typeparam name="TEntity">Type of entity to delete</typeparam>
public interface IDeleteOperation<TKey, TUserKey, TEntity> where TEntity : class, IEntityBase<TKey, TUserKey>
{
    /// <summary>
    /// Method that deletes a record in the database
    /// </summary>
    /// <param name="id">Id of the record to delete</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>Represents an asynchronous operation that can return a value.</returns>
    Task<bool> DeleteAsync(TKey id, CancellationToken cancellationToken = default);
}