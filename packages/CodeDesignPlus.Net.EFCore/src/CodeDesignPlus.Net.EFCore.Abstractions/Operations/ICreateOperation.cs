using CodeDesignPlus.Net.Core.Abstractions;

namespace CodeDesignPlus.Net.EFCore.Abstractions.Operations;

/// <summary>
/// It allows the repository to create a record by assigning the information to the transversal properties of the entity
/// </summary>
/// <typeparam name="TEntity">Type of entity to create</typeparam>
public interface ICreateOperation< TEntity> where TEntity : class, IEntity
{
    /// <summary>
    /// Method to create a record in the database
    /// </summary>
    /// <param name="entity">Entity to create</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>Represents an asynchronous operation.</returns>
    Task CreateAsync(TEntity entity, CancellationToken cancellationToken = default);
}
