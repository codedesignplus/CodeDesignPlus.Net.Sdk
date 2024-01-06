﻿using CodeDesignPlus.Net.File.Storage.Factories;
using CodeDesignPlus.Net.File.Storage.Providers;
using CodeDesignPlus.Net.Security.Abstractions;
using Microsoft.Extensions.Hosting;
using Moq;
using M = CodeDesignPlus.Net.File.Storage.Abstractions.Models;

namespace CodeDesignPlus.Net.File.Storage.Test.Integration;

public class AzureFileProviderIntegrationTest
{
    [Fact(Skip = "This unit test is for integration test")]
    public async Task WriteFile()
    {
        // Arrange
        var path = AppDomain.CurrentDomain.BaseDirectory;

        var file = new M.File(Path.Join(path, "Helpers", "Files", "FakeDocument.txt"))
        {
            Renowned = true,
        };

        var stream = new MemoryStream(System.IO.File.ReadAllBytes(file.FullName));

        var options = Microsoft.Extensions.Options.Options.Create(new FileStorageOptions()
        {
            UriDownload = new Uri("https://server.codedesignplus.com"),
            AzureFile = new AzureFileOptions()
            {
                Enable = true,
                Uri = new Uri("https://codedesignplusstorage.file.core.windows.net"),
                UsePasswordLess = false,
                DefaultEndpointsProtocol = "https",
                AccountName = "codedesignplusstorage",
                AccountKey = "4/e5AP+hRQ46e6oQwO9LgEq/ylB1bO3fyf2lAkb7BMFU5Mt8IH9eh/iziyqvnw0UD7sRh7NKVA+1+AStkJ7+6w==",
                EndpointSuffix = "core.windows.net"
            }
        });

        var logger = new Mock<ILogger<AzureFileProvider<string, string>>>();

        var environment = new Mock<IHostEnvironment>();

        var userContext = new Mock<IUserContext<string, string>>();

        userContext.SetupGet(u => u.Tenant).Returns("client-001");

        var azureFileFactory = new AzureFileFactory<string, string>(options, userContext.Object);

        var provider = new AzureFileProvider<string, string>(azureFileFactory, logger.Object, environment.Object);

        // Act
        var result = await provider.UploadAsync(stream, "FakeDocument.txt", null);

        // Assert
    }


    [Fact(Skip = "This unit test is for integration test")]
    public async Task ReadFileAsync()
    {
        // Arrange
        var path = AppDomain.CurrentDomain.BaseDirectory;

        var file = new M.File(Path.Join(path, "Helpers", "Files", "FakeDocument.txt"))
        {
            Renowned = true,
        };

        var stream = new MemoryStream(System.IO.File.ReadAllBytes(file.FullName));

        var options = Microsoft.Extensions.Options.Options.Create(new FileStorageOptions()
        {
            UriDownload = new Uri("https://server.codedesignplus.com"),
            AzureFile = new AzureFileOptions()
            {
                Enable = true,
                Uri = new Uri("https://codedesignplusstorage.file.core.windows.net"),
                UsePasswordLess = false,
                DefaultEndpointsProtocol = "https",
                AccountName = "codedesignplusstorage",
                AccountKey = "4/e5AP+hRQ46e6oQwO9LgEq/ylB1bO3fyf2lAkb7BMFU5Mt8IH9eh/iziyqvnw0UD7sRh7NKVA+1+AStkJ7+6w==",
                EndpointSuffix = "core.windows.net"
            }
        });

        var logger = new Mock<ILogger<AzureFileProvider<string, string>>>();

        var environment = new Mock<IHostEnvironment>();

        var userContext = new Mock<IUserContext<string, string>>();

        userContext.SetupGet(u => u.Tenant).Returns("client-001");

        var azureFileFactory = new AzureFileFactory<string, string>(options, userContext.Object);

        var provider = new AzureFileProvider<string, string>(azureFileFactory, logger.Object, environment.Object);

        // Act
        var result = await provider.DownloadAsync(System.IO.Path.GetFileName(file.FullName), "docs/temp/info");

        // Assert
    }


    [Fact(Skip = "This unit test is for integration test")]
    public async Task DeleteFileAsync()
    {
        // Arrange
        var path = AppDomain.CurrentDomain.BaseDirectory;

        var file = new M.File(Path.Join(path, "Helpers", "Files", "FakeDocument.txt"))
        {
            Renowned = true,
        };

        var stream = new MemoryStream(System.IO.File.ReadAllBytes(file.FullName));

        var options = Microsoft.Extensions.Options.Options.Create(new FileStorageOptions()
        {
            UriDownload = new Uri("https://server.codedesignplus.com"),
            AzureFile = new AzureFileOptions()
            {
                Enable = true,
                Uri = new Uri("https://codedesignplusstorage.file.core.windows.net"),
                UsePasswordLess = false,
                DefaultEndpointsProtocol = "https",
                AccountName = "codedesignplusstorage",
                AccountKey = "4/e5AP+hRQ46e6oQwO9LgEq/ylB1bO3fyf2lAkb7BMFU5Mt8IH9eh/iziyqvnw0UD7sRh7NKVA+1+AStkJ7+6w==",
                EndpointSuffix = "core.windows.net"
            }
        });

        var logger = new Mock<ILogger<AzureFileProvider<string, string>>>();

        var environment = new Mock<IHostEnvironment>();

        var userContext = new Mock<IUserContext<string, string>>();

        userContext.SetupGet(u => u.Tenant).Returns("client-001");

        var azureFileFactory = new AzureFileFactory<string, string>(options, userContext.Object);

        var provider = new AzureFileProvider<string, string>(azureFileFactory, logger.Object, environment.Object);

        // Act
        var result = await provider.DeleteAsync(System.IO.Path.GetFileName(file.FullName), "docs/temp/info");

        // Assert
    }
}