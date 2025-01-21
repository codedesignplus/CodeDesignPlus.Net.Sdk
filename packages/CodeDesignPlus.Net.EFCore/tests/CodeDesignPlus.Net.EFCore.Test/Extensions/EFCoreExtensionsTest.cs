using CodeDesignPlus.Entities;
using CodeDesignPlus.InMemory;
using CodeDesignPlus.InMemory.EntityConfiguration;
using CodeDesignPlus.InMemory.Repositories;
using CodeDesignPlus.Net.EFCore.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace CodeDesignPlus.Net.EFCore.Test.Extensions;

public class EFCoreExtensionsTest
{
    [Fact]
    public void ConfigurationBase_ValidateConfigProperties_ConfigDefaults()
    {
        // Arrange
        var builder = new DbContextOptionsBuilder<CodeDesignPlusContextInMemory>();

        var options = builder.UseInMemoryDatabase(nameof(EFCoreExtensionsTest)).Options;

        var context = new CodeDesignPlusContextInMemory(options);

        var modelBuilder = new ModelBuilder(ConventionSet.CreateConventionSet(context));

        var entityTypeBuilder = modelBuilder.Entity<Permission>();

        var customerEntityConfiguration = new PermissionEntityConfiguration();

        customerEntityConfiguration.Configure(entityTypeBuilder);

        // Act
        var idProperty = entityTypeBuilder.Metadata.FindDeclaredProperty(nameof(Permission.Id));
        var idUserCreatorProperty = entityTypeBuilder.Metadata.FindDeclaredProperty(nameof(Permission.CreatedBy));
        var IsActiveProperty = entityTypeBuilder.Metadata.FindDeclaredProperty(nameof(Permission.IsActive));
        var dateCreatedProperty = entityTypeBuilder.Metadata.FindDeclaredProperty(nameof(Permission.CreatedAt));

        // Assert
        Assert.NotNull(idProperty);
        Assert.NotNull(idUserCreatorProperty);
        Assert.NotNull(IsActiveProperty);
        Assert.NotNull(dateCreatedProperty);

        Assert.True(idProperty.IsPrimaryKey());
        Assert.False(idProperty.IsNullable);
        Assert.Equal(ValueGenerated.OnAdd, idProperty.ValueGenerated);
        Assert.False(idUserCreatorProperty.IsNullable);
        Assert.False(IsActiveProperty.IsNullable);
        Assert.False(dateCreatedProperty.IsNullable);
    }

    [Fact]
    public async Task ToPageAsync_ArgumentsInvalid_Null()
    {
        // Arrange
        var builder = new DbContextOptionsBuilder<CodeDesignPlusContextInMemory>();

        var options = builder.UseInMemoryDatabase(nameof(EFCoreExtensionsTest)).Options;

        var context = new CodeDesignPlusContextInMemory(options);

        var repository = new ApplicationRepository(context);

        // Act
        var pager = await repository.GetEntity<Application>().ToPageAsync(0, 0);

        // Assert
        Assert.Null(pager);
    }

    [Fact]
    public async Task ToPageAsync_PageFromDb_Pager()
    {
        // Arrange
        var currentPage = 1;
        var pageSize = 10;
        var totalItems = 500;
        var maxPages = 10;
        var startIndex = (currentPage - 1) * pageSize;
        var endIndex = Math.Min(startIndex + pageSize - 1, totalItems - 1);
        var totalPages = (int)Math.Ceiling(totalItems / (decimal)pageSize);

        var builder = new DbContextOptionsBuilder<CodeDesignPlusContextInMemory>();

        var options = builder.UseInMemoryDatabase(nameof(EFCoreExtensionsTest)).Options;

        var context = new CodeDesignPlusContextInMemory(options);

        var repository = new ApplicationRepository(context);

        var applications = new List<Application>();

        for (int i = 0; i < totalItems; i++)
        {
            applications.Add(new Application()
            {
                Name = $"{nameof(Application.Name)}-{i}",
                CreatedBy = Guid.NewGuid(),
                IsActive = true,
                CreatedAt = SystemClock.Instance.GetCurrentInstant(),
                Description = $"{nameof(Application.Description)}-{i}"
            });
        }

        await repository.CreateRangeAsync(applications);

        // Act
        var pager = await repository.GetEntity<Application>().ToPageAsync(currentPage, pageSize);

        // Assert
        Assert.Equal(totalItems, pager.TotalItems);
        Assert.Equal(currentPage, pager.CurrentPage);
        Assert.Equal(pageSize, pager.PageSize);
        Assert.Equal(totalPages, pager.TotalPages);
        Assert.Equal(pager.Pages.Min(), pager.StartPage);
        Assert.Equal(pager.Pages.Max(), pager.EndPage);
        Assert.Equal(maxPages, pager.Pages.Count());
        Assert.Equal(startIndex, pager.StartIndex);
        Assert.Equal(endIndex, pager.EndIndex);
    }

    [Fact]
    public void RegisterEntityConfigurations_ScanAndInvokeConfigure_EntityConfigurationInvoked()
    {
        var modelBuilder = new ModelBuilder();

        modelBuilder.RegisterEntityConfigurations<CodeDesignPlusContextInMemory>();

        Assert.True(ApplicationEntityConfiguration.IsInvoked);
        Assert.True(AppPermissionEntityConfiguration.IsInvoked);
        Assert.True(PermissionEntityConfiguration.IsInvoked);
        Assert.True(RolePermissionEntityConfiguration.IsInvoked);
    }
}