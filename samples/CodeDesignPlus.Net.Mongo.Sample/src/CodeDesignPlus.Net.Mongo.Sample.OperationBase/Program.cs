using CodeDesignPlus.Net.Mongo.Extensions;
using CodeDesignPlus.Net.Mongo.Sample.OperationBase;
using CodeDesignPlus.Net.Mongo.Sample.OperationBase.Entities;
using CodeDesignPlus.Net.Mongo.Sample.OperationBase.Respositories;
using CodeDesignPlus.Net.Mongo.Sample.OperationBase.Services;
using CodeDesignPlus.Net.Security.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

var serviceCollection = new ServiceCollection();

serviceCollection.AddLogging();
serviceCollection.AddMongo<Startup>(configuration);
serviceCollection.AddSingleton<IUserContext, FakeUserContext>();

var serviceProvider = serviceCollection.BuildServiceProvider();

var repository = serviceProvider.GetRequiredService<IUserRepository>();

var tenant = Guid.NewGuid();
var createdBy = Guid.NewGuid();

var user = UserAggregate.Create(Guid.NewGuid(), "John Doe", "john.doe@codedesignplus.com", tenant, createdBy);

// Create a new user
await repository.CreateAsync(user, CancellationToken.None);

// Find a user to update
user.UpdateName("John Doe Updated");

await repository.UpdateAsync(user.Id, user, tenant, CancellationToken.None);

// Delete a user
await repository.DeleteAsync(user.Id, tenant, CancellationToken.None);