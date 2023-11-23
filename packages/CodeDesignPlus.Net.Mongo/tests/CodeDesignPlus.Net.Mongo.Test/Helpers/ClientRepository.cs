
using CodeDesignPlus.Net.Mongo.Repository;

namespace CodeDesignPlus.Net.Mongo.Test.Helpers;

public class ClientRepository : RepositoryBase<Guid, Guid>, IClientRepository
{
    public ClientRepository(IServiceProvider serviceProvider, IOptions<MongoOptions> mongoOptions, ILogger<RepositoryBase<Guid, Guid>> logger) 
        : base(serviceProvider, mongoOptions, logger)
    {
    }
}

public interface IClientRepository: IRepositoryBase<Guid, Guid>
{
}