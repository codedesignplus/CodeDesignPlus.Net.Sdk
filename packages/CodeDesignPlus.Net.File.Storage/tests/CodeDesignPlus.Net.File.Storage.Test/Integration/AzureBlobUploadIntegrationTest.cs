using Azure.Storage.Blobs;
using CodeDesignPlus.Net.File.Storage.Abstractions.Factories;
using CodeDesignPlus.Net.File.Storage.Providers;
using Microsoft.Extensions.Hosting;
using Moq;
using M = CodeDesignPlus.Net.File.Storage.Abstractions.Models;

namespace CodeDesignPlus.Net.File.Storage.Test.Integration;

/// <summary>
/// La subida de un archivo cuyo nombre lleva tildes, eñe o paréntesis, contra Azurite (pendings/526).
/// </summary>
[Collection(AzuriteCollectionFixture.Collection)]
public class AzureBlobUploadIntegrationTest(AzuriteCollectionFixture fixture)
{
    private readonly Guid tenant = Guid.NewGuid();
    private readonly BlobServiceClient client = fixture.Container.CreateClient();

    private AzureBlobProvider Provider()
    {
        var factory = new Mock<IAzureBlobFactory>();

        factory.SetupGet(x => x.Options).Returns(OptionsUtil.FileStorageOptions);
        factory.SetupGet(x => x.Client).Returns(client);
        factory.Setup(x => x.Create()).Returns(factory.Object);
        factory.Setup(x => x.GetContainerClient(It.IsAny<Guid>())).Returns<Guid>(id => client.GetBlobContainerClient(id.ToString()));

        var environment = Mock.Of<IHostEnvironment>(x => x.EnvironmentName == Environments.Development);

        return new AzureBlobProvider(factory.Object, Mock.Of<ILogger<AzureBlobProvider>>(), environment);
    }

    [Theory]
    [InlineData("Cotización (1).pdf")]
    [InlineData("Acta de reunión – año 2026.docx")]
    [InlineData("Señalización & más.png")]
    public async Task UploadAsync_NameWithAccentsOrParentheses_KeepsTheNameAndUploads(string fileName)
    {
        // Arrange
        using var stream = new MemoryStream([1, 2, 3]);
        var target = $"quotations/{Guid.NewGuid()}";

        // Act
        var response = await Provider().UploadAsync(stream, fileName, target, false, tenant);

        // Assert
        Assert.True(response.Success, response.Exception?.ToString() ?? response.Message);
        var blob = client.GetBlobContainerClient(tenant.ToString()).GetBlobClient($"{target}/{fileName}");
        var tags = (await blob.GetTagsAsync()).Value.Tags;

        Assert.Equal((M.File.ToTagValue(System.IO.Path.GetFileNameWithoutExtension(fileName)), fileName), (tags["Name"], response.File.Detail.File));
    }
}
