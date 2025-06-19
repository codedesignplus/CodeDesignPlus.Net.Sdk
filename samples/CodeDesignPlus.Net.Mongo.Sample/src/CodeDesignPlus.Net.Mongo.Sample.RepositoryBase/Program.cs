// See https://aka.ms/new-console-template for more information
using CodeDesignPlus.Net.Core.Abstractions.Models.Criteria;
using CodeDesignPlus.Net.Mongo.Extensions;
using CodeDesignPlus.Net.Mongo.Sample.RepositoryBase;
using CodeDesignPlus.Net.Mongo.Sample.RepositoryBase.DTOs;
using CodeDesignPlus.Net.Mongo.Sample.RepositoryBase.Entities;
using CodeDesignPlus.Net.Mongo.Sample.RepositoryBase.Respositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using NodaTime;

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

var serviceCollection = new ServiceCollection();

serviceCollection.AddLogging();
serviceCollection.AddMongo<Startup>(configuration);

var serviceProvider = serviceCollection.BuildServiceProvider();

var repository = serviceProvider.GetRequiredService<IUserRepository>();

var product = new ProductEntity
{
    Id = Guid.NewGuid(),
    Name = "Product 1",
    CreatedAt = SystemClock.Instance.GetCurrentInstant(),
    CreatedBy = Guid.NewGuid(),
    IsActive = true
};

var tenant = Guid.NewGuid();
var createdBy = Guid.NewGuid();
var idCountry = Guid.NewGuid();

var user = UserAggregate.Create(Guid.NewGuid(), "John Doe", "john.doe@codedesignplus.com", idCountry, tenant, createdBy);

user.AddProduct(product);

var users = new List<UserAggregate>
{
    UserAggregate.Create(Guid.NewGuid(), "Jane Doe", "jane.doe@codedesignplus.com", Guid.NewGuid(), tenant, createdBy),
    UserAggregate.Create(Guid.NewGuid(), "John Smith", "john.smith@codedesignplus.com", Guid.NewGuid(),tenant, createdBy)
};

// Create a new user
await repository.CreateAsync(user, CancellationToken.None);

// Create some users
await repository.CreateRangeAsync(users, CancellationToken.None);

// Change state
await repository.ChangeStateAsync<UserAggregate>(user.Id, false, tenant, CancellationToken.None);

// Find a user
var userFound = await repository.FindAsync<UserAggregate>(user.Id, tenant, CancellationToken.None);

// Criteris to find users
var criteria = new Criteria
{
    Filters = $"IdCountry={idCountry}",
    Skip = 0,
    Limit = 10
};

var usersFound = await repository.MatchingAsync<UserAggregate>(criteria, tenant, CancellationToken.None);

// Criteria with projection
var projection = await repository.MatchingAsync<UserAggregate, UserDto>(criteria, x => new UserDto
{
    Id = x.Id,
    Name = x.Name,
    Email = x.Email
}, tenant, CancellationToken.None);

// Criteria at subdocument level and projection
var criteriaSubDocument = new Criteria
{
    Filters = "IsActive=true",
    Skip = 0,
    Limit = 10
};

var projectionSubdocument = await repository.MatchingAsync<UserAggregate, ProductEntity>(user.Id, criteriaSubDocument, x => x.Products, tenant, CancellationToken.None);

// Update user
var userUpdate = await repository.FindAsync<UserAggregate>(user.Id, tenant, CancellationToken.None);

userUpdate.UpdateName("John Doe Updated");

await repository.UpdateAsync(userUpdate, CancellationToken.None);

// Update some users
var usersUpdate = await repository.MatchingAsync<UserAggregate>(criteria, tenant, CancellationToken.None);

foreach (var userToUpdate in usersUpdate.Data)
{
    userToUpdate.UpdateName($"{userToUpdate.Name} Updated");
}

await repository.UpdateRangeAsync(usersUpdate.Data.ToList(), CancellationToken.None);

// Transaction
await repository.TransactionAsync(async (database, session) =>
{
    var userTransaction = UserAggregate.Create(Guid.NewGuid(), "John Doe Transaction", "john.doe@codedesignplus.com", Guid.NewGuid(), tenant, createdBy);

    var collection = database.GetCollection<UserAggregate>(typeof(UserAggregate).Name);

    await collection.InsertOneAsync(session, userTransaction, cancellationToken: CancellationToken.None);

}, CancellationToken.None);

// Delete user
var filterUser = Builders<UserAggregate>.Filter.Eq(x => x.Id, user.Id);

await repository.DeleteAsync(filterUser, tenant, CancellationToken.None);

// Delete some users
await repository.DeleteRangeAsync(users, tenant, CancellationToken.None);

