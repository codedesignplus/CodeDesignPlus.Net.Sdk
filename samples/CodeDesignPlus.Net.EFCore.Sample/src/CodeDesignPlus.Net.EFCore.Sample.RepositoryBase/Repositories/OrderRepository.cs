namespace CodeDesignPlus.Net.EFCore.Sample.RepositoryBase.Repositories;

public class OrderRepository(OrderContext context) : Repository.RepositoryBase(context), IOrderRepository
{
}
