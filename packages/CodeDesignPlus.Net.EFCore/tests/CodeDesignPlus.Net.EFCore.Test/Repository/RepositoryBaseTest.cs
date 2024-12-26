using System.Net;
using CodeDesignPlus.Entities;
using CodeDesignPlus.InMemory;
using CodeDesignPlus.InMemory.Repositories;
using CodeDesignPlus.Net.xUnit.Containers.SqlServer;
using Microsoft.EntityFrameworkCore;

namespace CodeDesignPlus.Net.EFCore.Test.Repository;

[Collection(SqlCollectionFixture.Collection)]
public class RepositoryBaseTest(SqlCollectionFixture sqlCollectionFixture)
{
    private readonly SqlServerContainer sqlServerContainer = sqlCollectionFixture.Container;

    [Fact]
    public void Constructor_ArgumentIsNull_ArgumentNullException()
    {
        var exception = Assert.Throws<ArgumentNullException>(() => new ApplicationRepository(null));

        Assert.Equal("Value cannot be null. (Parameter 'context')", exception.Message);
    }

    [Fact]
    public void GetContext_CastContext()
    {
        // Arrange
        var builder = new DbContextOptionsBuilder<CodeDesignPlusContextInMemory>();

        var options = builder.UseInMemoryDatabase(nameof(RepositoryBaseTest)).Options;

        var context = new CodeDesignPlusContextInMemory(options);

        var repository = new ApplicationRepository(context);

        // Act
        var result = repository.GetContext<CodeDesignPlusContextInMemory>();

        // Assert
        Assert.IsType<CodeDesignPlusContextInMemory>(result);
    }

    [Fact]
    public void GetEntity_EntityExist_NotNull()
    {
        // Arrange 
        var builder = new DbContextOptionsBuilder<CodeDesignPlusContextInMemory>();

        var options = builder.UseInMemoryDatabase(nameof(RepositoryBaseTest)).Options;

        var context = new CodeDesignPlusContextInMemory(options);

        var repository = new ApplicationRepository(context);

        // Act
        var dbset = repository.GetEntity<Application>();
        // dbset.EntityType.FullName()
        // Assert 
        Assert.NotNull(dbset);
        Assert.Equal(nameof(Application), dbset.EntityType.ClrType.Name);
    }

    [Fact]
    public void GetEntity_EntityNotExist_Exception()
    {
        // Arrange 
        var builder = new DbContextOptionsBuilder<CodeDesignPlusContextInMemory>();

        var options = builder.UseInMemoryDatabase(nameof(RepositoryBaseTest)).Options;

        var context = new CodeDesignPlusContextInMemory(options);

        var repository = new ApplicationRepository(context);

        // Act
        var dbset = repository.GetEntity<FakeEntity>();

        // Assert 
        Assert.NotNull(dbset);

        var exception = Assert.Throws<InvalidOperationException>(() => dbset.EntityType);

        Assert.Equal($"Cannot create a DbSet for '{nameof(FakeEntity)}' because this type is not included in the model for the context.", exception.Message);
    }

    [Fact]
    public async Task CreateAsync_EntityIsNull_ArgumentNullException()
    {
        // Arrange 
        var builder = new DbContextOptionsBuilder<CodeDesignPlusContextInMemory>();

        var options = builder.UseInMemoryDatabase(nameof(RepositoryBaseTest)).Options;

        var context = new CodeDesignPlusContextInMemory(options);

        var repository = new ApplicationRepository(context);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => repository.CreateAsync<Application>(null!));

        Assert.Equal("Value cannot be null. (Parameter 'entity')", exception.Message);
    }

    [Fact]
    public async Task CreateAsync_EntityIsNotNull_IdIsGreeaterThanZero()
    {
        // Arrange 
        var entity = new Application()
        {
            Id = Guid.NewGuid(),
            Name = nameof(Application.Name),
            CreatedBy = Guid.NewGuid(),
            IsActive = true,
            CreatedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            Description = nameof(Application.Description)
        };

        var builder = new DbContextOptionsBuilder<CodeDesignPlusContextInMemory>();

        var options = builder.UseInMemoryDatabase(nameof(RepositoryBaseTest)).Options;

        var context = new CodeDesignPlusContextInMemory(options);

        var repository = new ApplicationRepository(context);

        // Act 
        await repository.CreateAsync(entity);

        var result = await repository.GetEntity<Application>().FirstOrDefaultAsync(x => x.Id == entity.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(entity.Id, result.Id);
        Assert.Equal(nameof(Application.Name), result.Name);
        Assert.Equal(nameof(Application.Description), result.Description);
        Assert.Equal(entity.CreatedBy, result.CreatedBy);
        Assert.Equal(entity.IsActive, result.IsActive);
        Assert.Equal(entity.CreatedAt, result.CreatedAt);
    }

    [Fact]
    public async Task UpdateAsync_EntityIsNull_ArgumentNullException()
    {
        // Arrange 
        var builder = new DbContextOptionsBuilder<CodeDesignPlusContextInMemory>();

        var options = builder.UseInMemoryDatabase(nameof(RepositoryBaseTest)).Options;

        var context = new CodeDesignPlusContextInMemory(options);

        var repository = new ApplicationRepository(context);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => repository.UpdateAsync<Application>(null!));

        Assert.Equal("Value cannot be null. (Parameter 'entity')", exception.Message);
    }

    [Fact]
    public async Task UpdateAsync_AssignUpdateInfo_Success()
    {
        // Arrange 
        var builder = new DbContextOptionsBuilder<CodeDesignPlusContextInMemory>();

        var options = builder.UseInMemoryDatabase(nameof(RepositoryBaseTest)).Options;

        var context = new CodeDesignPlusContextInMemory(options);

        var repository = new ApplicationRepository(context);

        var applicationCreated = new Application()
        {
            Id = Guid.NewGuid(),
            Name = nameof(Application.Name),
            CreatedBy = Guid.NewGuid(),
            IsActive = true,
            CreatedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            Description = nameof(Application.Description)
        };

        await repository.CreateAsync(applicationCreated);

        // Act
        var applicationUpdate = await repository.GetEntity<Application>().FirstOrDefaultAsync(x => x.Id == applicationCreated.Id);

        Assert.NotNull(applicationUpdate);

        applicationUpdate.Description = "New Description";
        applicationUpdate.Name = "New Name";
        applicationUpdate.CreatedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        applicationUpdate.IsActive = false;
        applicationUpdate.CreatedBy = Guid.NewGuid();

        await repository.UpdateAsync(applicationUpdate);

        // Assert
        var result = await repository.GetEntity<Application>().FirstOrDefaultAsync(x => x.Id == applicationUpdate.Id);

        Assert.NotNull(result);
        Assert.Equal("New Name", result.Name);
        Assert.Equal("New Description", result.Description);
        Assert.False(result.IsActive);
        Assert.Equal(applicationCreated.CreatedBy, result.CreatedBy);
        Assert.Equal(applicationCreated.CreatedAt, result.CreatedAt);
    }

    [Fact]
    public async Task DeleteAsync_EntityIsNull_ArgumentNullException()
    {
        // Arrange 
        var builder = new DbContextOptionsBuilder<CodeDesignPlusContextInMemory>();

        var options = builder.UseInMemoryDatabase(nameof(RepositoryBaseTest)).Options;

        var context = new CodeDesignPlusContextInMemory(options);

        var repository = new ApplicationRepository(context);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => repository.DeleteAsync<Application>(null));

        Assert.Equal("Value cannot be null. (Parameter 'predicate')", exception.Message);
    }

    [Fact]
    public async Task DeleteAsync_EntityNotExist_False()
    {
        // Arrange 
        var builder = new DbContextOptionsBuilder<CodeDesignPlusContextInMemory>();

        var options = builder.UseInMemoryDatabase(nameof(RepositoryBaseTest)).Options;

        var context = new CodeDesignPlusContextInMemory(options);

        var repository = new ApplicationRepository(context);

        var id = Guid.NewGuid();

        // Act
        await repository.DeleteAsync<Application>(x => x.Id == id);

        var result = await repository.GetEntity<Application>().FirstOrDefaultAsync(x => x.Id == id);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteAsync_EntityExist_True()
    {
        // Arrange 
        var builder = new DbContextOptionsBuilder<CodeDesignPlusContextInMemory>();

        var options = builder.UseInMemoryDatabase(nameof(RepositoryBaseTest)).Options;

        var context = new CodeDesignPlusContextInMemory(options);

        var repository = new ApplicationRepository(context);

        var applicationCreated = new Application()
        {
            Name = nameof(Application.Name),
            CreatedBy = Guid.NewGuid(),
            IsActive = true,
            CreatedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            Description = nameof(Application.Description)
        };

        await repository.CreateAsync(applicationCreated);

        // Act
        await repository.DeleteAsync<Application>(x => x.Id == applicationCreated.Id);

        var result = await repository.GetEntity<Application>().FirstOrDefaultAsync(x => x.Id == applicationCreated.Id);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CraeteRangeAsync_ListEmpty_ReturnListEmpty()
    {
        // Arrange 
        var entities = new List<Application>();

        var builder = new DbContextOptionsBuilder<CodeDesignPlusContextInMemory>();

        var options = builder.UseInMemoryDatabase(nameof(CraeteRangeAsync_ListEmpty_ReturnListEmpty)).Options;

        var context = new CodeDesignPlusContextInMemory(options);

        var repository = new ApplicationRepository(context);

        // Act
        await repository.CreateRangeAsync(entities);

        var result = await repository.GetEntity<Application>().ToListAsync();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task CreateRangeAsync_ListWithData_ReturnListAndIds()
    {
        // Arrange 
        var entities = new List<Application>
        {
            new ()
            {
                Id = Guid.NewGuid(),
                Name = nameof(Application.Name),
                CreatedBy = Guid.NewGuid(),
                IsActive = true,
                CreatedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                Description = nameof(Application.Description)
            },
            new ()
            {
                Id = Guid.NewGuid(),
                Name = nameof(Application.Name),
                CreatedBy = Guid.NewGuid(),
                IsActive = true,
                CreatedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                Description = nameof(Application.Description)
            }
        };

        var builder = new DbContextOptionsBuilder<CodeDesignPlusContextInMemory>();

        var options = builder.UseInMemoryDatabase(nameof(CreateRangeAsync_ListWithData_ReturnListAndIds)).Options;

        var context = new CodeDesignPlusContextInMemory(options);

        var repository = new ApplicationRepository(context);

        // Act
        await repository.CreateRangeAsync(entities);

        var result = await repository.GetEntity<Application>().ToListAsync();

        // Assert
        Assert.NotEmpty(result);
        Assert.Equal(entities.Count, result.Count);
        Assert.Equal(entities[0].Id, result[0].Id);
        Assert.Equal(entities[1].Id, result[1].Id);
    }

    [Fact]
    public async Task UpdateRangeAsync_ListEmpty_ReturnFalse()
    {
        // Arrange 
        var entities = new List<Application>();

        var builder = new DbContextOptionsBuilder<CodeDesignPlusContextInMemory>();

        var options = builder.UseInMemoryDatabase(nameof(UpdateRangeAsync_ListEmpty_ReturnFalse)).Options;

        var context = new CodeDesignPlusContextInMemory(options);

        var repository = new ApplicationRepository(context);

        // Act
        await repository.UpdateRangeAsync(entities);

        var result = await repository.GetEntity<Application>().ToListAsync();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task UpdateRangeAsync_AssignUpdateInfo_Success()
    {
        // Arrange
        var entities = new List<Application>
        {
            new ()
            {
                Id = Guid.NewGuid(),
                Name = nameof(Application.Name),
                CreatedBy = Guid.NewGuid(),
                IsActive = true,
                CreatedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                Description = nameof(Application.Description)
            },

            new ()
            {
                Id = Guid.NewGuid(),
                Name = nameof(Application.Name),
                CreatedBy = Guid.NewGuid(),
                IsActive = true,
                CreatedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                Description = nameof(Application.Description)
            }
        };

        var builder = new DbContextOptionsBuilder<CodeDesignPlusContextInMemory>();

        var options = builder.UseInMemoryDatabase(nameof(RepositoryBaseTest)).Options;

        var context = new CodeDesignPlusContextInMemory(options);

        var repository = new ApplicationRepository(context);

        await repository.CreateRangeAsync(entities);

        var entitiesCreated = await repository.GetEntity<Application>().ToListAsync();

        // Act
        var entitiesUpdate = await repository.GetEntity<Application>().Where(x => x.IsActive).ToListAsync();

        entitiesUpdate.ForEach(x =>
        {
            x.Description = "New Description";
            x.Name = "New Name";
            x.CreatedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            x.IsActive = false;
            x.CreatedBy = Guid.NewGuid();
        });

        await repository.UpdateRangeAsync(entitiesUpdate);

        // Assert
        var result = await repository.GetEntity<Application>().Where(x => !x.IsActive).ToListAsync();

        foreach (var item in result)
        {
            Assert.Equal("New Name", item.Name);
            Assert.Equal("New Description", item.Description);
            Assert.False(item.IsActive);
        }
    }

    [Fact]
    public async Task DeleteRangeAsync_ListEmpty_ReturnFalse()
    {
        // Arrange 
        var entities = new List<Application>();

        var builder = new DbContextOptionsBuilder<CodeDesignPlusContextInMemory>();

        var options = builder.UseInMemoryDatabase(nameof(DeleteRangeAsync_ListEmpty_ReturnFalse)).Options;

        var context = new CodeDesignPlusContextInMemory(options);

        var repository = new ApplicationRepository(context);

        // Act
        await repository.DeleteRangeAsync(entities);

        var result = await repository.GetEntity<Application>().ToListAsync();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task DeleteRangeAsync_EntityExist_True()
    {
        // Arrange 
        var entities = new List<Application>
        {
            new ()
            {
                Id = Guid.NewGuid(),
                Name = nameof(Application.Name),
                CreatedBy = Guid.NewGuid(),
                IsActive = true,
                CreatedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                Description = nameof(Application.Description)
            },
            new ()
            {
                Id = Guid.NewGuid(),
                Name = nameof(Application.Name),
                CreatedBy = Guid.NewGuid(),
                IsActive = true,
                CreatedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                Description = nameof(Application.Description)
            }
        };

        var builder = new DbContextOptionsBuilder<CodeDesignPlusContextInMemory>();

        var options = builder.UseInMemoryDatabase(nameof(DeleteRangeAsync_EntityExist_True)).Options;

        var context = new CodeDesignPlusContextInMemory(options);

        var repository = new ApplicationRepository(context);

        await repository.CreateRangeAsync(entities);

        var entitiesCreated = await repository.GetEntity<Application>().ToListAsync();

        // Act
        var entitiesDelete = await repository.GetEntity<Application>().Where(x => x.IsActive).ToListAsync();

        await repository.DeleteRangeAsync(entitiesDelete);

        var result = await repository.GetEntity<Application>().ToListAsync();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task ChangeStateAsync_EntityNotExist_ReturnFalse()
    {
        // Arrange 
        var builder = new DbContextOptionsBuilder<CodeDesignPlusContextInMemory>();

        var options = builder.UseInMemoryDatabase(nameof(RepositoryBaseTest)).Options;

        var context = new CodeDesignPlusContextInMemory(options);

        var repository = new ApplicationRepository(context);
        var id = Guid.NewGuid();

        // Act 
        await repository.ChangeStateAsync<Application>(id, false);

        var result = await repository.GetEntity<Application>().FirstOrDefaultAsync(x => x.Id == id);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task ChangeStateAsync_EntityExist_ReturnTrue()
    {
        // Arrange 
        var entity = new Application()
        {
            Id = Guid.NewGuid(),
            Name = nameof(Application.Name),
            CreatedBy = Guid.NewGuid(),
            IsActive = true,
            CreatedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            Description = nameof(Application.Description)
        };

        var builder = new DbContextOptionsBuilder<CodeDesignPlusContextInMemory>();

        var options = builder.UseInMemoryDatabase(nameof(RepositoryBaseTest)).Options;

        var context = new CodeDesignPlusContextInMemory(options);

        var repository = new ApplicationRepository(context);

        await repository.CreateAsync(entity);

        var entityCreated = await repository.GetEntity<Application>().FirstOrDefaultAsync(x => x.Id == entity.Id);

        Assert.NotNull(entityCreated);

        // Act 
        await repository.ChangeStateAsync<Application>(entityCreated.Id, !entityCreated.IsActive);

        var result = await repository.GetEntity<Application>().FirstOrDefaultAsync(x => x.Id == entityCreated.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(entityCreated.Id, result.Id);
        Assert.Equal(entityCreated.Name, result.Name);
        Assert.Equal(entityCreated.Description, result.Description);
        Assert.Equal(entityCreated.CreatedBy, result.CreatedBy);
        Assert.Equal(entityCreated.CreatedAt, result.CreatedAt);
        Assert.False(result.IsActive);
    }

    [Fact]
    public async Task TransactionAsync_CommitedTransaction_ReturnResultDelegate()
    {
        // Arrange 
        var applicationCreated = new Application()
        {
            Id = Guid.NewGuid(),
            Name = nameof(Application.Name),
            CreatedBy = Guid.NewGuid(),
            IsActive = true,
            CreatedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            Description = nameof(Application.Description)
        };

        var builder = new DbContextOptionsBuilder<CodeDesignPlusContextInMemory>();

        var connectionString = $"Server=localhost,{this.sqlServerContainer.Port};Database=temp1;User Id=sa;Password=Temporal1;Encrypt=True;TrustServerCertificate=True";

        var options = builder.UseSqlServer(connectionString, x =>
        {
            x.EnableRetryOnFailure(5, TimeSpan.FromSeconds(2), null);
        }).Options;

        var context = new CodeDesignPlusContextInMemory(options);

        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();

        var repository = new ApplicationRepository(context);

        // Act
        await repository.TransactionAsync<bool>(async context =>
        {
            await repository.CreateAsync(applicationCreated);
        });

        var result = await repository.GetEntity<Application>().FirstOrDefaultAsync(x => x.Id == applicationCreated.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(applicationCreated.Id, result.Id);
        Assert.Equal(applicationCreated.Name, result.Name);
        Assert.Equal(applicationCreated.Description, result.Description);
        Assert.Equal(applicationCreated.CreatedBy, result.CreatedBy);
        Assert.Equal(applicationCreated.CreatedAt, result.CreatedAt);
        Assert.Equal(applicationCreated.IsActive, result.IsActive);
    }

    [Fact]
    public async Task TransactionAsync_RollbackTransaction_InvalidOperationException()
    {
        // Arrange 
        var applicationCreated = new Application()
        {
            Id = Guid.NewGuid(),
            Name = nameof(Application.Name),
            CreatedBy = Guid.NewGuid(),
            IsActive = true,
            CreatedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            Description = nameof(Application.Description)
        };

        var builder = new DbContextOptionsBuilder<CodeDesignPlusContextInMemory>();

        var connectionString = $"Server=localhost,{sqlServerContainer.Port};Database=temp2;User Id=sa;Password=Temporal1;Encrypt=True;TrustServerCertificate=True";

        var options = builder.UseSqlServer(connectionString, x =>
        {
            x.EnableRetryOnFailure(5, TimeSpan.FromSeconds(2), null);
        }).Options;

        var context = new CodeDesignPlusContextInMemory(options);

        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();

        var repository = new ApplicationRepository(context);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            await repository.TransactionAsync<bool>(async context =>
            {
                await repository.CreateAsync(applicationCreated);

                var application = await repository.GetEntity<Application>().FirstOrDefaultAsync(x => x.Id == applicationCreated.Id);

                if (application != null)
                {
                    throw new InvalidOperationException("Failed Transaction");
                }
            });
        });

        // Assert
        var result = await repository.GetEntity<Application>().FirstOrDefaultAsync(x => x.Id == applicationCreated.Id);

        Assert.Null(result);
        Assert.Equal("Failed Transaction", exception.Message);
    }
}