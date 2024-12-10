using CodeDesignPlus.Net.Mongo.Abstractions.Options;
using CodeDesignPlus.Net.Mongo.Repository;
using CodeDesignPlus.Net.Mongo.Sample.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CodeDesignPlus.Net.Mongo.Sample.Respositories;

public class UserRepository(IServiceProvider serviceProvider, IOptions<MongoOptions> mongoOptions, ILogger<UserRepository> logger) 
    : RepositoryBase(serviceProvider, mongoOptions, logger), IUserRepository
{
    public Task CustomMethodAsync()
    {
        var collection = this.GetCollection<UserEntity>();

        // Do something with the collection

        return Task.CompletedTask;
    }
}
