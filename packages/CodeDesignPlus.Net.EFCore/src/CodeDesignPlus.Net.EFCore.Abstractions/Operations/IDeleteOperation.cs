namespace CodeDesignPlus.Net.EFCore.Abstractions.Operations;

/// <summary>
/// Provides an interface for deleting a record in the repository and assigning information to the transversal properties of the entity.
/// </summary>
public interface IDeleteOperation
{
    /// <summary>
    /// Deletes a record in the database.
    /// </summary>
    /// <param name="id">The ID of the record to delete.</param>
    /// <param name="cancellationToken">A token to propagate notification that operations should be canceled.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}