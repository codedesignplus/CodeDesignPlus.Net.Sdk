using CodeDesignPlus.Net.EFCore.Abstractions.Operations;
using CodeDesignPlus.Net.EFCore.Sample.OperationBase.Entities;

namespace CodeDesignPlus.Net.EFCore.Sample.OperationBase.Repositories;

public interface IOrderRepository: IOperationBase<OrderAggregate>
{

}
