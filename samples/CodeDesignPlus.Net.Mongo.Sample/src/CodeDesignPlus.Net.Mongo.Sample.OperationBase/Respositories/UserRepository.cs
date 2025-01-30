using CodeDesignPlus.Net.Mongo.Abstractions.Options;
using CodeDesignPlus.Net.Mongo.Repository;
using CodeDesignPlus.Net.Mongo.Sample.OperationBase.Entities;
using CodeDesignPlus.Net.Security.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CodeDesignPlus.Net.Mongo.Sample.OperationBase.Respositories;

public class UserRepository(IUserContext authenticatetUser, IServiceProvider serviceProvider, IOptions<MongoOptions> mongoOptions, ILogger<UserRepository> logger) 
    : CodeDesignPlus.Net.Mongo.Operations.OperationBase<UserAggregate>(authenticatetUser, serviceProvider, mongoOptions, logger), IUserRepository
{
}
