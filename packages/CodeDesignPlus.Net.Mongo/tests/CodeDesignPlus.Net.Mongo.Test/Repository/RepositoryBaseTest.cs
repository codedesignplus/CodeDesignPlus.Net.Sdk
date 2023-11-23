using CodeDesignPlus.Net.xUnit.Helpers.MongoContainer;
using MongoDB.Driver;
using CodeDesignPlus.Net.Mongo.Test.Helpers.Models;
using Moq;
using CodeDesignPlus.Net.xUnit.Helpers;

namespace CodeDesignPlus.Net.Mongo.Test
{
    public class RepositoryBaseTest : IClassFixture<MongoContainer>
    {
        private readonly Mock<ILogger<RepositoryBase<Guid, Guid>>> loggerMock;
        private readonly MongoContainer mongoContainer;

        private readonly IOptions<MongoOptions> options;

        public RepositoryBaseTest(MongoContainer mongoContainer)
        {
            this.mongoContainer = mongoContainer;
            this.loggerMock = new Mock<ILogger<RepositoryBase<Guid, Guid>>>();
            this.options = Microsoft.Extensions.Options.Options.Create(OptionsUtil.GetOptions(this.mongoContainer.Port));
        }

        [Fact]
        public async Task ChangeStateAsync_WhenEntityIsValid_ReturnTrue()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;
            var (client, collection) = GetCollection();
            var serviceProvider = GetServiceProvider(client, collection);

            var repository = new RepositoryBase<Guid, Guid>(serviceProvider, this.options, loggerMock.Object);

            var entity = new Client()
            {
                Id = Guid.NewGuid(),
                Name = "Test",
                IsActive = true
            };

            _ = await repository.CreateAsync(entity, cancellationToken);

            // Act
            var isSuccess = await repository.ChangeStateAsync<Client>(entity.Id, false, cancellationToken);

            var result = await collection.Find(x => x.Id == entity.Id).FirstOrDefaultAsync(cancellationToken);

            // Assert
            Assert.NotNull(result);
            Assert.True(isSuccess);
            Assert.Equal(entity.Id, result.Id);
            Assert.Equal(entity.Name, result.Name);
            Assert.NotEqual(entity.IsActive, result.IsActive);
            Assert.Equal(entity.CreatedAt, result.CreatedAt);
            Assert.False(result.IsActive);
        }

        [Fact]
        public async Task CreateAsync_WhenEntityIsValid_ReturnEntityCreated()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;
            var (client, collection) = GetCollection();
            var serviceProvider = GetServiceProvider(client, collection);

            var repository = new RepositoryBase<Guid, Guid>(serviceProvider, this.options, loggerMock.Object);

            var entity = new Client()
            {
                Id = Guid.NewGuid(),
                Name = "Test",
                IsActive = true
            };

            // Act
            _ = await repository.CreateAsync(entity, cancellationToken);

            var result = await collection.Find(x => x.Id == entity.Id).FirstOrDefaultAsync(cancellationToken);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(entity.Id, result.Id);
            Assert.Equal(entity.Name, result.Name);
            Assert.Equal(entity.IsActive, result.IsActive);
            Assert.Equal(entity.CreatedAt, result.CreatedAt);
        }

        [Fact]
        public async Task CreateRangeAsync_WhenEntityIsValid_ReturnEntityCreated()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;
            var (client, collection) = GetCollection();
            var serviceProvider = GetServiceProvider(client, collection);

            var repository = new RepositoryBase<Guid, Guid>(serviceProvider, this.options, loggerMock.Object);

            var entities = new List<Client>()
            {
                new ()
                {
                    Id = Guid.NewGuid(),
                    Name = "Test",
                    IsActive = true
                },
                new ()
                {
                    Id = Guid.NewGuid(),
                    Name = "Test",
                    IsActive = true
                }
            };

            // Act
            _ = await repository.CreateRangeAsync(entities, cancellationToken);

            // Assert
            foreach (var entity in entities)
            {
                var result = await collection.Find(x => x.Id == entity.Id).FirstOrDefaultAsync(cancellationToken);

                Assert.NotNull(result);
                Assert.Equal(entity.Id, result.Id);
                Assert.Equal(entity.Name, result.Name);
                Assert.Equal(entity.IsActive, result.IsActive);
                Assert.Equal(entity.CreatedAt, result.CreatedAt);
            }
        }

        [Fact]
        public async Task DeleteAsync_WhenEntityIsValid_ReturnTrue()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;
            var (client, collection) = GetCollection();
            var serviceProvider = GetServiceProvider(client, collection);

            var repository = new RepositoryBase<Guid, Guid>(serviceProvider, this.options, loggerMock.Object);

            var entity = new Client()
            {
                Id = Guid.NewGuid(),
                Name = "Test",
                IsActive = true
            };

            _ = await repository.CreateAsync(entity, cancellationToken);

            // Act
            var filter = Builders<Client>.Filter.Eq(x => x.Id, entity.Id);
            var isSuccess = await repository.DeleteAsync(filter, cancellationToken);

            var result = await collection.Find(x => x.Id == entity.Id).FirstOrDefaultAsync(cancellationToken);

            // Assert
            Assert.Null(result);
            Assert.True(isSuccess);
        }

        [Fact]
        public async Task DeleteRangeAsync_WhenEntityIsValid_ReturnTrue()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;
            var (client, collection) = GetCollection();
            var serviceProvider = GetServiceProvider(client, collection);

            var repository = new RepositoryBase<Guid, Guid>(serviceProvider, this.options, loggerMock.Object);

            var entities = new List<Client>()
            {
                new ()
                {
                    Id = Guid.NewGuid(),
                    Name = "Test",
                    IsActive = true
                },
                new ()
                {
                    Id = Guid.NewGuid(),
                    Name = "Test",
                    IsActive = true
                }
            };

            _ = await repository.CreateRangeAsync(entities, cancellationToken);

            // Act
            var isSuccess = await repository.DeleteRangeAsync(entities, cancellationToken);

            // Assert
            foreach (var entity in entities)
            {
                var result = await collection.Find(x => x.Id == entity.Id).FirstOrDefaultAsync(cancellationToken);

                Assert.Null(result);
            }

            Assert.True(isSuccess);
        }

        [Fact]
        public async Task UpdateAsync_WhenEntityIsValid_ReturnTrue()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;
            var (client, collection) = GetCollection();
            var serviceProvider = GetServiceProvider(client, collection);

            var repository = new RepositoryBase<Guid, Guid>(serviceProvider, this.options, loggerMock.Object);

            var entity = new Client()
            {
                Id = Guid.NewGuid(),
                Name = "Test",
                IsActive = true
            };

            _ = await repository.CreateAsync(entity, cancellationToken);

            // Act
            entity.Name = "Test 2";
            var isSuccess = await repository.UpdateAsync(entity, cancellationToken);

            var result = await collection.Find(x => x.Id == entity.Id).FirstOrDefaultAsync(cancellationToken);

            // Assert
            Assert.NotNull(result);
            Assert.True(isSuccess);
            Assert.Equal(entity.Id, result.Id);
            Assert.Equal(entity.Name, result.Name);
            Assert.Equal(entity.IsActive, result.IsActive);
            Assert.Equal(entity.CreatedAt, result.CreatedAt);
        }

        [Fact]
        public async Task UpdateRangeAsync_WhenEntityIsValid_ReturnTrue()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;
            var (client, collection) = GetCollection();
            var serviceProvider = GetServiceProvider(client, collection);

            var repository = new RepositoryBase<Guid, Guid>(serviceProvider, this.options, loggerMock.Object);

            var entities = new List<Client>()
            {
                new ()
                {
                    Id = Guid.NewGuid(),
                    Name = "Test",
                    IsActive = true
                },
                new ()
                {
                    Id = Guid.NewGuid(),
                    Name = "Test",
                    IsActive = true
                }
            };

            _ = await repository.CreateRangeAsync(entities, cancellationToken);

            // Act
            foreach (var entity in entities)
            {
                entity.Name = "Test 2";
            }

            var isSuccess = await repository.UpdateRangeAsync(entities, cancellationToken);

            // Assert
            foreach (var entity in entities)
            {
                var result = await collection.Find(x => x.Id == entity.Id).FirstOrDefaultAsync(cancellationToken);

                Assert.NotNull(result);
                Assert.True(isSuccess);
                Assert.Equal(entity.Id, result.Id);
                Assert.Equal(entity.Name, result.Name);
                Assert.Equal(entity.IsActive, result.IsActive);
                Assert.Equal(entity.CreatedAt, result.CreatedAt);
            }
        }

        [Fact]
        public async Task TransactionAsync_WhenEntityIsValid_CommitSuccess()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;
            var (client, collection) = GetCollection();
            var serviceProvider = GetServiceProvider(client, collection);

            var repository = new RepositoryBase<Guid, Guid>(serviceProvider, this.options, loggerMock.Object);

            var entity = new Client()
            {
                Id = Guid.NewGuid(),
                Name = "Test",
                IsActive = true
            };

            // Act
            var result = await repository.TransactionAsync(async (database, session) =>
            {
                var clientCollection = database.GetCollection<Client>(typeof(Client).Name);

                await clientCollection.InsertOneAsync(session, entity, cancellationToken: cancellationToken);

                var result = await collection.Find(session, x => x.Id == entity.Id).FirstOrDefaultAsync(cancellationToken);

                return result;
            }, cancellationToken);


            // Assert
            Assert.NotNull(result);
            Assert.Equal(entity.Id, result.Id);
            Assert.Equal(entity.Name, result.Name);
            Assert.Equal(entity.IsActive, result.IsActive);
            Assert.Equal(entity.CreatedAt, result.CreatedAt);
        }

        [Fact]
        public async Task TransactionAsync_ThrowException_CommitFailed()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;
            var (client, collection) = GetCollection();
            var serviceProvider = GetServiceProvider(client, collection);

            var repository = new RepositoryBase<Guid, Guid>(serviceProvider, this.options, loggerMock.Object);

            var entity = new Client()
            {
                Id = Guid.NewGuid(),
                Name = "Test",
                IsActive = true
            };

            // Act
            var isSuccess = await repository.TransactionAsync<bool>(async (database, session) =>
            {
                var clientCollection = database.GetCollection<Client>(typeof(Client).Name);

                await clientCollection.InsertOneAsync(session, entity, cancellationToken: cancellationToken);

                throw new Exception("Custom Message");
            }, cancellationToken);


            // Assert
            var result = await collection.Find(x => x.Id == entity.Id).FirstOrDefaultAsync(cancellationToken);

            Assert.False(isSuccess);
            Assert.Null(result);
            this.loggerMock.VerifyLogging("Failed to execute transaction", LogLevel.Error);
        }

        private static ServiceProvider GetServiceProvider(IMongoClient mongoClient, IMongoCollection<Client> collection)
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton(mongoClient);
            serviceCollection.AddSingleton(collection);
            var serviceProvider = serviceCollection.BuildServiceProvider();
            return serviceProvider;
        }

        private (IMongoClient, IMongoCollection<Client>) GetCollection()
        {
            var connectionString = OptionsUtil.GetOptions(this.mongoContainer.Port).ConnectionString;

            var client = new MongoClient(connectionString);
            var database = client.GetDatabase("dbtestmongo");
            var collection = database.GetCollection<Client>(typeof(Client).Name);
            return (client, collection);
        }
    }
}