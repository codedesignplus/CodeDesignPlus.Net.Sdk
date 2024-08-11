using CodeDesignPlus.Entities;
using CodeDesignPlus.InMemory;
using CodeDesignPlus.InMemory.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CodeDesignPlus.Net.EFCore.Test.Operations;

/// <summary>
/// Unit tests to the OperationBase class
/// </summary>
public class OperationBaseTest
{
    /// <summary>
    /// Validate that an entity can be created and the record id is returned
    /// </summary>
    [Fact]
    public async Task CreateAsync_CreateEntity_ReturnId()
    {
        // Arrange
        var permission = new Permission()
        {
            Id = Guid.NewGuid(),
            Name = "Create - Permissions",
            Description = "Can create to permissions",
            Controller = "Permission",
            Action = "Post",
            IsActive = true
        };

        var userContext = new UserContext()
        {
            Name = "codedesignplus",
            Email = "codedesignplus@outlook.com",
            IdUser = Guid.NewGuid(),
            IsAuthenticated = true,
            IsApplication = false,
        };

        var builder = new DbContextOptionsBuilder<CodeDesignPlusContextInMemory>();

        var options = builder.UseInMemoryDatabase(nameof(OperationBaseTest)).Options;

        var context = new CodeDesignPlusContextInMemory(options);

        var repository = new PermissionRepository(userContext, context);

        // Act
        await repository.CreateAsync(permission);

        var result = await repository.GetEntity<Permission>().FirstOrDefaultAsync(x => x.Id == permission.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(permission.Id, result.Id);
        Assert.Equal("Create - Permissions", result.Name);
        Assert.Equal("Can create to permissions", result.Description);
        Assert.Equal("Permission", result.Controller);
        Assert.Equal("Post", result.Action);
        Assert.True(result.IsActive);
        Assert.Equal(userContext.IdUser, result.CreatedBy);
        Assert.Equal(permission.CreatedAt, result.CreatedAt);
    }

    /// <summary>
    /// Validate that an entity can be updated and true is returned
    /// </summary>
    [Fact]
    public async Task UpdateAsync_UpdateEntity_ReturnTrue()
    {
        // Arrange
        var permission = new Permission()
        {
            Id = Guid.NewGuid(),
            Name = "Create - Permissions",
            Description = "Can create to permissions",
            Controller = "Permission",
            Action = "Post",
            IsActive = true
        };

        var userContext = new UserContext()
        {
            Name = "codedesignplus",
            Email = "codedesignplus@outlook.com",
            IdUser = Guid.NewGuid(),
            IsAuthenticated = true,
            IsApplication = false,
        };

        var builder = new DbContextOptionsBuilder<CodeDesignPlusContextInMemory>();

        var options = builder.UseInMemoryDatabase(nameof(OperationBaseTest)).Options;

        var context = new CodeDesignPlusContextInMemory(options);

        var repository = new PermissionRepository(userContext, context);

        await repository.CreateAsync(permission);

        // Act
        var entityUpdate = new Permission()
        {
            Id = permission.Id,
            Name = "Update - Permissions",
            Description = "Can update to permissions",
            Controller = "Permission",
            Action = "Put",
            IsActive = false
        };

        await repository.UpdateAsync(permission.Id, entityUpdate);

        // Assert
        var entity = await repository.GetEntity<Permission>().FindAsync(permission.Id);

        Assert.NotNull(entity);
        Assert.Equal(permission.Id, entity.Id);
        Assert.Equal("Update - Permissions", entity.Name);
        Assert.Equal("Can update to permissions", entity.Description);
        Assert.Equal("Permission", entity.Controller);
        Assert.Equal("Put", entity.Action);
        Assert.False(entity.IsActive);
        Assert.Equal(userContext.IdUser, entity.CreatedBy);
        Assert.Equal(permission.CreatedAt, entity.CreatedAt);
    }

    /// <summary>
    /// Validate that an entity cannot be updated and false is returned
    /// </summary>
    [Fact]
    public async Task UpdateAsync_EntityNotExist_ReturnFalse()
    {
        // Arrange
        var userContext = new UserContext()
        {
            Name = "codedesignplus",
            Email = "codedesignplus@outlook.com",
            IdUser = Guid.NewGuid(),
            IsAuthenticated = true,
            IsApplication = false,
        };

        var builder = new DbContextOptionsBuilder<CodeDesignPlusContextInMemory>();

        var options = builder.UseInMemoryDatabase(nameof(OperationBaseTest)).Options;

        var context = new CodeDesignPlusContextInMemory(options);

        var repository = new PermissionRepository(userContext, context);

        // Act
        var entityUpdate = new Permission()
        {
            Id = Guid.NewGuid(),
            Name = "Update - Permissions",
            Description = "Can update to permissions",
            Controller = "Permission",
            Action = "Put",
            IsActive = false
        };

        await repository.UpdateAsync(entityUpdate.Id, entityUpdate);

        var result = await repository.GetEntity<Permission>().FirstOrDefaultAsync(x => x.Id == entityUpdate.Id);

        // Assert
        Assert.Null(result);
    }

    /// <summary>
    /// Validate that an entity can be removed and true is returned
    /// </summary>
    [Fact]
    public async Task DeleteAsync_DeleteEntity_ReturnTrue()
    {
        // Arrange
        var permission = new Permission()
        {
            Id = Guid.NewGuid(),
            Name = "Create - Permissions",
            Description = "Can create to permissions",
            Controller = "Permission",
            Action = "Post",
            IsActive = true
        };

        var userContext = new UserContext()
        {
            Name = "codedesignplus",
            Email = "codedesignplus@outlook.com",
            IdUser = Guid.NewGuid(),
            IsAuthenticated = true,
            IsApplication = false,
        };

        var builder = new DbContextOptionsBuilder<CodeDesignPlusContextInMemory>();

        var options = builder.UseInMemoryDatabase(nameof(OperationBaseTest)).Options;

        var context = new CodeDesignPlusContextInMemory(options);

        var repository = new PermissionRepository(userContext, context);

        await repository.CreateAsync(permission);

        // Act
        await repository.DeleteAsync(permission.Id);

        var result = await repository.GetEntity<Permission>().FirstOrDefaultAsync(x => x.Id == permission.Id);

        // Assert
        Assert.Null(result);
    }

    /// <summary>
    /// Validate that an entity cannot be deleted and false is returned
    /// </summary>
    [Fact]
    public async Task DeleteAsync_EntityNotExist_ReturnFalse()
    {
        // Arrange
        var userContext = new UserContext()
        {
            Name = "codedesignplus",
            Email = "codedesignplus@outlook.com",
            IdUser = Guid.NewGuid(),
            IsAuthenticated = true,
            IsApplication = false,
        };

        var builder = new DbContextOptionsBuilder<CodeDesignPlusContextInMemory>();

        var options = builder.UseInMemoryDatabase(nameof(OperationBaseTest)).Options;

        var context = new CodeDesignPlusContextInMemory(options);

        var repository = new PermissionRepository(userContext, context);

        var id = Guid.NewGuid();

        // Act
        await repository.DeleteAsync(id);

        var result = await repository.GetEntity<Permission>().FirstOrDefaultAsync(x => x.Id == id);

        // Assert
        Assert.Null(result);
    }
}