
using CodeDesignPlus.Net.Mongo.Repository;

namespace CodeDesignPlus.Net.Mongo.Test.Helpers;

public class ClientRepository : RepositoryBase, IClientRepository
{
    public ClientRepository(IServiceProvider serviceProvider, IOptions<MongoOptions> mongoOptions, ILogger<RepositoryBase> logger) 
        : base(serviceProvider, mongoOptions, logger)
    {
    }
}

public interface IClientRepository: IRepositoryBase
{
}