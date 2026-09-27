using Moq;
using System.Text;
using CodeDesignPlus.Net.File.Storage.Abstractions.Options;
using CodeDesignPlus.Net.File.Storage.Abstractions.Providers;
using CodeDesignPlus.Net.File.Storage.Providers;
using CodeDesignPlus.Net.File.Storage.Services;
using CodeDesignPlus.Net.File.Storage.Test.Helpers;
using Microsoft.Extensions.Hosting;
using M = CodeDesignPlus.Net.File.Storage.Abstractions.Models;

namespace CodeDesignPlus.Net.File.Storage.Test.Providers;

/// <summary>
/// Purging a tenant removed from the platform deletes every file it stored, and nothing of another tenant.
/// </summary>
public class DeleteTenantTest
{
    private static LocalProvider Local() => new(Microsoft.Extensions.Options.Options.Create(OptionsUtil.FileStorageOptions), new Mock<ILogger<LocalProvider>>().Object, new Mock<IHostEnvironment>().Object);

    [Fact]
    public async Task DeleteTenantAsync_LocalWithFiles_DeletesOnlyThatTenantFolder()
    {
        // Arrange
        var purged = Guid.NewGuid();
        var kept = Guid.NewGuid();
        var provider = Local();

        await provider.UploadAsync(new MemoryStream(Encoding.UTF8.GetBytes("a")), "a.txt", "documents", false, purged);
        await provider.UploadAsync(new MemoryStream(Encoding.UTF8.GetBytes("b")), "b.txt", "photos", false, purged);
        await provider.UploadAsync(new MemoryStream(Encoding.UTF8.GetBytes("c")), "c.txt", "documents", false, kept);

        // Act
        var response = await provider.DeleteTenantAsync(purged);

        // Assert
        Assert.True(response.Success);
        Assert.False(Directory.Exists(Path.Combine(OptionsUtil.FileStorageOptions.Local.Folder, purged.ToString())));
        Assert.True(Directory.Exists(Path.Combine(OptionsUtil.FileStorageOptions.Local.Folder, kept.ToString())));
    }

    [Fact]
    public async Task DeleteTenantAsync_LocalWithoutFiles_Succeeds()
    {
        // Arrange
        var provider = Local();

        // Act
        var response = await provider.DeleteTenantAsync(Guid.NewGuid());

        // Assert
        Assert.True(response.Success);
    }

    [Fact]
    public async Task DeleteTenantAsync_Service_AsksEveryProvider()
    {
        // Arrange
        var tenant = Guid.NewGuid();
        var providers = new List<Mock<IProvider>>
        {
            new Mock<IAzureBlobProvider>().As<IProvider>(),
            new Mock<IAzureFileProvider>().As<IProvider>(),
            new Mock<ILocalProvider>().As<IProvider>(),
        };

        providers.ForEach(provider => provider
            .Setup(x => x.DeleteTenantAsync(tenant, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new M.Response(new M.File(tenant.ToString()), TypeProviders.LocalProvider) { Success = true }));

        var service = new FileStorageService(providers.Select(p => p.Object));

        // Act
        var responses = await service.DeleteTenantAsync(tenant);

        // Assert
        Assert.Equal(3, responses.Length);
        providers.ForEach(p => p.Verify(x => x.DeleteTenantAsync(tenant, It.IsAny<CancellationToken>()), Times.Once));
    }

    [Fact]
    public async Task DeleteTenantAsync_Service_DisabledProvider_IsLeftOut()
    {
        // Arrange
        var tenant = Guid.NewGuid();
        var enabled = new Mock<IAzureBlobProvider>().As<IProvider>();
        var disabled = new Mock<IAzureFileProvider>().As<IProvider>();

        enabled
            .Setup(x => x.DeleteTenantAsync(tenant, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new M.Response(new M.File(tenant.ToString()), TypeProviders.AzureBlobProvider) { Success = true });

        // A disabled provider answers null: ProviderBase.ProcessAsync does not run it.
        disabled
            .Setup(x => x.DeleteTenantAsync(tenant, It.IsAny<CancellationToken>()))
            .ReturnsAsync((M.Response)null!);

        var service = new FileStorageService([enabled.Object, disabled.Object]);

        // Act
        var responses = await service.DeleteTenantAsync(tenant);

        // Assert
        Assert.Single(responses);
        Assert.All(responses, response => Assert.NotNull(response));
    }

    [Fact]
    public async Task DeleteTenantAsync_Service_EmptyTenantThrows()
    {
        // Arrange
        var service = new FileStorageService([]);

        // Act & Assert
        await Assert.ThrowsAsync<FileStorageException>(() => service.DeleteTenantAsync(Guid.Empty));
    }
}
