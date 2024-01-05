using CodeDesignPlus.Net.File.Storage.Factories;
using CodeDesignPlus.Net.File.Storage.Providers;
using CodeDesignPlus.Net.Security.Abstractions;
using Microsoft.Extensions.Hosting;
using Moq;
using M = CodeDesignPlus.Net.File.Storage.Abstractions.Models;

namespace CodeDesignPlus.Net.File.Storage.Test.Integration;

public class AzureBlobProviderIntegrationTest
{

    [Fact(Skip = "This unit test is for integration test")]
    public async Task WriteFile()
    {
        // Arrange
        var path = AppDomain.CurrentDomain.BaseDirectory;

        var file = new M.File(Path.Join(path, "Helpers", "Files", "FakeDocument.txt"))
        {
            Renowned = true
        };

        var stream = new MemoryStream(System.IO.File.ReadAllBytes(file.FullName));

        var options = Microsoft.Extensions.Options.Options.Create(new FileStorageOptions()
        {
            UriDownload = new Uri("https://server.codedesignplus.com"),
            AzureBlob = new AzureBlobOptions()
            {
                Enable = true,
                Uri = new Uri("https://codedesignplusstorage.blob.core.windows.net"),
                UsePasswordLess = false,
                DefaultEndpointsProtocol = "https",
                AccountName = "codedesignplusstorage",
                AccountKey = "4/e5AP+hRQ46e6oQwO9LgEq/ylB1bO3fyf2lAkb7BMFU5Mt8IH9eh/iziyqvnw0UD7sRh7NKVA+1+AStkJ7+6w==",
                EndpointSuffix = "core.windows.net"
            }
        });

        var logger = new Mock<ILogger<AzureBlobProvider<string, string>>>();

        var environment = new Mock<IHostEnvironment>();

        var userContext = new Mock<IUserContext<string, string>>();

        userContext.SetupGet(u => u.Tenant).Returns("client-001");

        var azureBlobFactory = new AzureBlobFactory<string, string>(options, userContext.Object);

        var provider = new AzureBlobProvider<string, string>(azureBlobFactory, logger.Object, environment.Object);

        // Act
        var result = await provider.UploadAsync(stream, file, "docs/temp/info");

        // Assert
    }


    [Fact(Skip = "This unit test is for integration test")]
    public async Task ReadFile()
    {
        // Arrange
        var path = AppDomain.CurrentDomain.BaseDirectory;

        var file = new M.File(Path.Join(path, "Helpers", "Files", "FakeDocument.txt"))
        {
            Renowned = true
        };

        var stream = new MemoryStream(System.IO.File.ReadAllBytes(file.FullName));

        var options = Microsoft.Extensions.Options.Options.Create(new FileStorageOptions()
        {
            UriDownload = new Uri("https://server.codedesignplus.com"),
            AzureBlob = new AzureBlobOptions()
            {
                Enable = true,
                Uri = new Uri("https://codedesignplusstorage.blob.core.windows.net"),
                UsePasswordLess = false,
                DefaultEndpointsProtocol = "https",
                AccountName = "codedesignplusstorage",
                AccountKey = "4/e5AP+hRQ46e6oQwO9LgEq/ylB1bO3fyf2lAkb7BMFU5Mt8IH9eh/iziyqvnw0UD7sRh7NKVA+1+AStkJ7+6w==",
                EndpointSuffix = "core.windows.net"
            }
        });

        var logger = new Mock<ILogger<AzureBlobProvider<string, string>>>();

        var environment = new Mock<IHostEnvironment>();

        var userContext = new Mock<IUserContext<string, string>>();

        userContext.SetupGet(u => u.Tenant).Returns("client-001");

        var azureBlobFactory = new AzureBlobFactory<string, string>(options, userContext.Object);

        var provider = new AzureBlobProvider<string, string>(azureBlobFactory, logger.Object, environment.Object);

        // Act
        var result = await provider.DownloadAsync(System.IO.Path.GetFileName(file.FullName), "docs/temp/info");

        // Assert
    }

    
    [Fact(Skip = "This unit test is for integration test")]
    public async Task DeleteFile()
    {
        // Arrange
        var path = AppDomain.CurrentDomain.BaseDirectory;

        var file = new M.File(Path.Join(path, "Helpers", "Files", "FakeDocument.txt"))
        {
            Renowned = true
        };

        var stream = new MemoryStream(System.IO.File.ReadAllBytes(file.FullName));

        var options = Microsoft.Extensions.Options.Options.Create(new FileStorageOptions()
        {
            UriDownload = new Uri("https://server.codedesignplus.com"),
            AzureBlob = new AzureBlobOptions()
            {
                Enable = true,
                Uri = new Uri("https://codedesignplusstorage.blob.core.windows.net"),
                UsePasswordLess = false,
                DefaultEndpointsProtocol = "https",
                AccountName = "codedesignplusstorage",
                AccountKey = "4/e5AP+hRQ46e6oQwO9LgEq/ylB1bO3fyf2lAkb7BMFU5Mt8IH9eh/iziyqvnw0UD7sRh7NKVA+1+AStkJ7+6w==",
                EndpointSuffix = "core.windows.net"
            }
        });

        var logger = new Mock<ILogger<AzureBlobProvider<string, string>>>();

        var environment = new Mock<IHostEnvironment>();

        var userContext = new Mock<IUserContext<string, string>>();

        userContext.SetupGet(u => u.Tenant).Returns("client-001");

        var azureBlobFactory = new AzureBlobFactory<string, string>(options, userContext.Object);

        var provider = new AzureBlobProvider<string, string>(azureBlobFactory, logger.Object, environment.Object);

        // Act
        var result = await provider.DeleteAsync(System.IO.Path.GetFileName(file.FullName), "docs/temp/info");

        // Assert
    }
}
