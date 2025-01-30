using CodeDesignPlus.Net.Mongo.Abstractions.Options;
using CodeDesignPlus.Net.Mongo.Sample.RepositoryBase.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CodeDesignPlus.Net.Mongo.Sample.RepositoryBase.Respositories;

public class UserRepository(IServiceProvider serviceProvider, IOptions<MongoOptions> mongoOptions, ILogger<UserRepository> logger) 
    : CodeDesignPlus.Net.Mongo.Repository.RepositoryBase(serviceProvider, mongoOptions, logger), IUserRepository
{
    public Task CustomMethodAsync()
    {
        var collection = this.GetCollection<UserAggregate>();

        // Do something with the collection

        return Task.CompletedTask;
    }
}
