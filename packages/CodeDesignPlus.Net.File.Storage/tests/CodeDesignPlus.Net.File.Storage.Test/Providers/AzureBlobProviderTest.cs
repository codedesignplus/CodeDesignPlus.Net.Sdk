using CodeDesignPlus.Net.File.Storage.Providers;
using Microsoft.Extensions.Hosting;
using Moq;

namespace CodeDesignPlus.Net.File.Storage.Test;

public class AzureBlobProviderTest
{

    [Fact]
    public async Task WriteFile()
    {
        // Arrange
        var path = AppDomain.CurrentDomain.BaseDirectory;

        var file = new Abstractions.Models.File(Path.Join(path, "Helpers", "Files", "FakeDocument.txt"));

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

        var logger = new Mock<ILogger<AzureBlobProvider<Guid>>>();

        var environment = new Mock<IHostEnvironment>();

        var provider = new AzureBlobProvider<Guid>(options, logger.Object, environment.Object);

        // Act
        var result = await provider.WriteFileAsync(Guid.NewGuid(), stream, "docs/temp/info", file);

        // Assert
    }
}
