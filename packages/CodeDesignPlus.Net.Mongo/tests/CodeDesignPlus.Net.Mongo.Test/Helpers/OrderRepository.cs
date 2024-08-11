using CodeDesignPlus.Net.Mongo.Abstractions;
using CodeDesignPlus.Net.Mongo.Repository;

namespace CodeDesignPlus.Net.Mongo.Test.Helpers;


public class OrderRepository : RepositoryBase, IOrderRepository
{
    public OrderRepository(IServiceProvider serviceProvider, IOptions<MongoOptions> mongoOptions, ILogger<OrderRepository> logger)
        : base(serviceProvider, mongoOptions, logger)
    {
    }
}

public interface IOrderRepository : IRepositoryBase
{
}