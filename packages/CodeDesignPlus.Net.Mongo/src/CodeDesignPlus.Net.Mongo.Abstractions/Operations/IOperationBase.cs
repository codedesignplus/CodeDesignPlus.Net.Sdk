namespace CodeDesignPlus.Net.Mongo.Abstractions.Operations;

/// <summary>
/// Defines the base operations for an entity.
/// </summary>
/// <typeparam name="TEntity">The type of the entity.</typeparam>
public interface IOperationBase<in TEntity> :
    ICreateOperation<TEntity>,
    IUpdateOperation<TEntity>,
    IDeleteOperation,
    IRepositoryBase
    where TEntity : class, IEntityBase
{ }