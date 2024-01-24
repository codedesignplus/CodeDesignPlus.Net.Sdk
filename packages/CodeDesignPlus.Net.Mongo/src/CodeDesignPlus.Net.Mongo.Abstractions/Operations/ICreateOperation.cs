using CodeDesignPlus.Net.Core.Abstractions;

namespace CodeDesignPlus.Net.Mongo.Abstractions.Operations;

/// <summary>
/// It allows the repository to create a record by assigning the information to the transversal properties of the entity
/// </summary>
/// <typeparam name="TKey">Type of data that will identify the record</typeparam>
/// <typeparam name="TUserKey">Type of data that the user will identify</typeparam>
/// <typeparam name="TEntity">Type of entity to create</typeparam>
public interface ICreateOperation<TKey, TUserKey, TEntity> where TEntity : class, IEntity<TKey, TUserKey>
{
    /// <summary>
    /// Method to create a record in the database
    /// </summary>
    /// <param name="entity">Entity to create</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>Represents an asynchronous operation that can return a value.</returns>
    Task<TKey> CreateAsync(TEntity entity, CancellationToken cancellationToken = default);
}
