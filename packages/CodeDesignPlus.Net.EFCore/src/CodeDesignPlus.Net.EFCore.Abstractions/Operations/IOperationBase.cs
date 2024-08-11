namespace CodeDesignPlus.Net.EFCore.Abstractions.Operations;

/// <summary>
/// It allows the repository to create, update and delete a record by assigning the information to the transversal properties of the entity
/// </summary>
/// <typeparam name="TEntity">The entity type to be configured.</typeparam>
public interface IOperationBase<in TEntity> :
    ICreateOperation<TEntity>,
    IUpdateOperation<TEntity>,
    IDeleteOperation,
    IRepositoryBase
    where TEntity : class, IEntityBase
{ }