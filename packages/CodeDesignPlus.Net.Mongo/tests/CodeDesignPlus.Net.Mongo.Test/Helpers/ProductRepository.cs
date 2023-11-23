using CodeDesignPlus.Net.Mongo.Repository;
using CodeDesignPlus.Net.Mongo.Test.Helpers.Models;
using CodeDesignPlus.Net.Security.Abstractions;

namespace CodeDesignPlus.Net.Mongo.Test.Helpers;

public class ProductRepository : OperationBase<Guid, Guid, Product>
{
    public ProductRepository(IUserContext<Guid> authenticatetUser, IServiceProvider serviceProvider, IOptions<MongoOptions> options, ILogger<RepositoryBase<Guid, Guid>> logger) 
        : base(authenticatetUser, serviceProvider, options, logger)
    {
    }
}
