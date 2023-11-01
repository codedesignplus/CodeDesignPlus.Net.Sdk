using CodeDesignPlus.Entities;
using CodeDesignPlus.InMemory;
using CodeDesignPlus.InMemory.EntityConfiguration;
using CodeDesignPlus.InMemory.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using CodeDesignPlus.Net.EFCore.Extensions;

namespace CodeDesignPlus.Net.EFCore.Test;

/// <summary>
/// Unit tests to the EFCoreExtensions class
/// </summary>
public class EFCoreExtensionsTest
{
    /// <summary>
    /// Validate that the EFCoreExtensions.ConfigurationBase extension method assigns the base configurations to the entity
    /// </summary>
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
        var idUserCreatorProperty = entityTypeBuilder.Metadata.FindDeclaredProperty(nameof(Permission.IdUserCreator));
        var IsActiveProperty = entityTypeBuilder.Metadata.FindDeclaredProperty(nameof(Permission.IsActive));
        var dateCreatedProperty = entityTypeBuilder.Metadata.FindDeclaredProperty(nameof(Permission.DateCreated));

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

    /// <summary>
    /// Validate that the EFCoreExtensions.ToPageAsync extension method returns the default object
    /// </summary>
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

    /// <summary>
    /// Validate that the EFCoreExtensions.ToPageAsync extension method returns the Pager object with the information
    /// </summary>
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
        var totalPages = (int)Math.Ceiling((decimal)totalItems / (decimal)pageSize);

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
                IdUserCreator = new Random().Next(1, 15),
                IsActive = true,
                DateCreated = DateTime.UtcNow,
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


    /// <summary>
    /// Validate that the EFCoreExtensions.RegisterEntityConfigurations method scans, instance and 
    /// invokes the Configure method of the classes that implement the IEntityTypeConfiguration <TEntity> interface
    /// </summary>
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