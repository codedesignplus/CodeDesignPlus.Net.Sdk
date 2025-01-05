// See https://aka.ms/new-console-template for more information
using CodeDesignPlus.Net.Core.Abstractions.Models.Criteria;
using CodeDesignPlus.Net.Mongo.Extensions;
using CodeDesignPlus.Net.Mongo.Sample.RepositoryBase;
using CodeDesignPlus.Net.Mongo.Sample.RepositoryBase.Entities;
using CodeDesignPlus.Net.Mongo.Sample.RepositoryBase.Respositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

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
    CreatedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
    CreatedBy = Guid.NewGuid(),
    IsActive = true
};

var user = new UserEntity
{
    Id = Guid.NewGuid(),
    Name = "John Doe",
    Email = "john.doe@codedesignplus.com",
    CreatedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
    CreatedBy = Guid.NewGuid(),
    IsActive = true
};

user.Products.Add(product);

var users = new List<UserEntity>
{
    new()
    {
        Id = Guid.NewGuid(),
        Name = "Jane Doe",
        Email = "jane.doe@codedesignplus.com",
        CreatedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
        CreatedBy = Guid.NewGuid(),
        IsActive = true
    },
    new()
    {
        Id = Guid.NewGuid(),
        Name = "John Smith",
        Email = "john.smith@codedesignplus.com",
        CreatedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
        CreatedBy = Guid.NewGuid(),
        IsActive = true
    }
};

// Create a new user
await repository.CreateAsync(user, CancellationToken.None);

// Create some users
await repository.CreateRangeAsync(users, CancellationToken.None);

// Change state
await repository.ChangeStateAsync<UserEntity>(user.Id, false, CancellationToken.None);

// Find a user
var userFound = await repository.FindAsync<UserEntity>(user.Id, CancellationToken.None);

// Criteris to find users
var criteria = new Criteria
{
    Filters = "IsActive=true"
};

var usersFound = await repository.MatchingAsync<UserEntity>(criteria, CancellationToken.None);

// Criteria with projection
var projection = await repository.MatchingAsync<UserEntity, UserEntity>(criteria, x => new UserEntity
{
    Id = x.Id,
    Name = x.Name,
    Email = x.Email
}, CancellationToken.None);

// Criteria at subdocument level and projection
var projectionSubdocument = await repository.MatchingAsync<UserEntity, ProductEntity>(user.Id, criteria, x => x.Products, CancellationToken.None);

// Update user
var userUpdate = await repository.FindAsync<UserEntity>(user.Id, CancellationToken.None);

userUpdate.Name = "John Doe Updated";

await repository.UpdateAsync(userUpdate, CancellationToken.None);

// Update some users
var usersUpdate = await repository.MatchingAsync<UserEntity>(criteria, CancellationToken.None);

usersUpdate.ForEach(x => x.Name = $"{x.Name} Updated");

await repository.UpdateRangeAsync(usersUpdate, CancellationToken.None);

// Transaction
await repository.TransactionAsync(async (database, session) =>
{
    var userTransaction = new UserEntity
    {
        Id = Guid.NewGuid(),
        Name = "John Doe Transaction",
        Email = "john.doe@codedesignplus.com"
    };

    var collection = database.GetCollection<UserEntity>(typeof(UserEntity).Name);

    await collection.InsertOneAsync(session, userTransaction, cancellationToken: CancellationToken.None);

}, CancellationToken.None);

// Delete user
var filterUser = Builders<UserEntity>.Filter.Eq(x => x.Id, user.Id);

await repository.DeleteAsync(filterUser, CancellationToken.None);

// Delete some users
await repository.DeleteRangeAsync(users, CancellationToken.None);

