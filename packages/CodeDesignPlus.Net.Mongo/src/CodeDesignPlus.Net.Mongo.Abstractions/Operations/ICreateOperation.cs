namespace CodeDesignPlus.Net.Mongo.Abstractions.Operations;

/// <summary>
/// It allows the repository to create a record by assigning the information to the transversal properties of the entity
/// </summary>
/// <typeparam name="TEntity">Type of entity to create</typeparam>
public interface ICreateOperation<in TEntity> where TEntity : class, IEntityBase
{
    /// <summary>
    /// Method to create a record in the database
    /// </summary>
    /// <param name="entity">Entity to create</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>Represents an asynchronous operation that can return a value.</returns>
    Task CreateAsync(TEntity entity, CancellationToken cancellationToken = default);
}
