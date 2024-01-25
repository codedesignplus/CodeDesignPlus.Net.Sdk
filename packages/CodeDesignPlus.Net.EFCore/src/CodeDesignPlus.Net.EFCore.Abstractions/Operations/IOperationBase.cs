using CodeDesignPlus.Net.Core.Abstractions;

namespace CodeDesignPlus.Net.EFCore.Abstractions.Operations;

/// <summary>
/// It allows the repository to create, update and delete a record by assigning the information to the transversal properties of the entity
/// </summary>
/// <typeparam name="TEntity">The entity type to be configured.</typeparam>
public interface IOperationBase<TEntity> :
    ICreateOperation<TEntity>,
    IUpdateOperation<TEntity>,
    IDeleteOperation<TEntity>,
    IRepositoryBase
    where TEntity : class, IEntityBase
{ }