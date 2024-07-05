using CodeDesignPlus.Net.Core.Abstractions;

namespace CodeDesignPlus.Net.Mongo.Abstractions.Operations;

/// <summary>
/// Enables standardized CRUD operations in the SDK
/// </summary>
/// <typeparam name="TEntity">The entity type to be configured.</typeparam>
public interface IOperationBase<in TEntity> :
    ICreateOperation<TEntity>,
    IUpdateOperation<TEntity>,
    IDeleteOperation,
    IRepositoryBase
    where TEntity : class, IEntityBase
{ }