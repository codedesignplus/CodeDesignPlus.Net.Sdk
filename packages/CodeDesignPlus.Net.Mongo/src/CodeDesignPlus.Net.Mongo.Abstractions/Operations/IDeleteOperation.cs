namespace CodeDesignPlus.Net.Mongo.Abstractions.Operations;

/// <summary>
/// Defines the delete operation for an entity.
/// </summary>
public interface IDeleteOperation
{
    /// <summary>
    /// Deletes an entity by its identifier asynchronously.
    /// </summary>
    /// <param name="id">The identifier of the entity to delete.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous delete operation.</returns>
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}