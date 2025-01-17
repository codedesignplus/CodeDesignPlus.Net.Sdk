using CodeDesignPlus.Net.Mongo.Abstractions;
using CodeDesignPlus.Net.Mongo.Operations;
using CodeDesignPlus.Net.Mongo.Repository;
using CodeDesignPlus.Net.Mongo.Test.Helpers.Models;
using CodeDesignPlus.Net.Security.Abstractions;

namespace CodeDesignPlus.Net.Mongo.Test.Helpers;

public class ProductRepository : OperationBase<ProductEntity>, IProductRepository
{
    public ProductRepository(IUserContext authenticatetUser, IServiceProvider serviceProvider, IOptions<MongoOptions> options, ILogger<RepositoryBase> logger)
        : base(authenticatetUser, serviceProvider, options, logger)
    {
    }
}

public interface IProductRepository : IRepositoryBase
{
}