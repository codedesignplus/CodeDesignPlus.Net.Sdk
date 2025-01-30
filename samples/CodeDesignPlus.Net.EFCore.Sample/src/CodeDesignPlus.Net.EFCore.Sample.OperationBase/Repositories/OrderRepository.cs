using CodeDesignPlus.Net.EFCore.Sample.OperationBase.Entities;
using CodeDesignPlus.Net.Security.Abstractions;

namespace CodeDesignPlus.Net.EFCore.Sample.OperationBase.Repositories;

public class OrderRepository(IUserContext userContext, OrderContext context) 
    : Operations.OperationBase<OrderAggregate>(userContext, context), IOrderRepository
{
}
