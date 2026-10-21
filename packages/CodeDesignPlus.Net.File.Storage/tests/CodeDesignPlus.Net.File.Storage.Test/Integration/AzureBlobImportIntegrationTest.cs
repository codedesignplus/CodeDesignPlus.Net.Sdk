using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using CodeDesignPlus.Net.File.Storage.Abstractions.Factories;
using CodeDesignPlus.Net.File.Storage.Providers;
using Microsoft.Extensions.Hosting;
using Moq;

namespace CodeDesignPlus.Net.File.Storage.Test.Integration;

/// <summary>
/// The import of a call recording against Azurite, with the same names Azure Communication Services leaves in the
/// recordings container: an audio-only recording is an MP3 next to its metadata JSON.
/// </summary>
[Collection(AzuriteCollectionFixture.Collection)]
public class AzureBlobImportIntegrationTest(AzuriteCollectionFixture fixture)
{
    private const string SourceContainer = "acs-recordings";
    private const string RecordingFolder = "20261021/307da556-1f0e-4b6a-9d3c-2a5b7e8f9c10/cyNjcifQ-0d6e5f4a3b2c1d0e";

    private readonly Guid tenant = Guid.NewGuid();
    private readonly BlobServiceClient client = fixture.Container.CreateClient();

    private AzureBlobProvider Provider()
    {
        var factory = new Mock<IAzureBlobFactory>();

        factory.SetupGet(x => x.Options).Returns(OptionsUtil.FileStorageOptions);
        factory.SetupGet(x => x.Client).Returns(client);
        factory.Setup(x => x.Create()).Returns(factory.Object);
        factory.Setup(x => x.GetContainerClient(It.IsAny<Guid>())).Returns<Guid>(id => client.GetBlobContainerClient(id.ToString()));

        // In development the response carries the exception, so a failing assertion shows where it was thrown.
        var environment = Mock.Of<IHostEnvironment>(x => x.EnvironmentName == Environments.Development);

        return new AzureBlobProvider(factory.Object, Mock.Of<ILogger<AzureBlobProvider>>(), environment);
    }

    private async Task<string> LeaveRecordingAsync(string fileName, byte[] content)
    {
        var container = client.GetBlobContainerClient(SourceContainer);

        await container.CreateIfNotExistsAsync();

        // Each recording gets its own folder, as ACS does, so the tests do not share files.
        var folder = $"{RecordingFolder}/{Guid.NewGuid():N}";

        await container.GetBlobClient($"{folder}/{fileName}").UploadAsync(new BinaryData(content));
        await container.GetBlobClient($"{folder}/0-acsmetadata.json").UploadAsync(BinaryData.FromString("{}"));

        return $"{folder}/{fileName}";
    }

    [Fact]
    public async Task ImportAsync_AcsAudioRecording_CopiesItIntoTheTenantFolderAsMpegAudio()
    {
        // Arrange
        var content = Encoding.UTF8.GetBytes("ID3 fake mp3 content");
        var sourcePath = await LeaveRecordingAsync("0-audiomp3.mp3", content);
        var target = $"assembly-recordings/{Guid.NewGuid()}";

        // Act
        var response = await Provider().ImportAsync(SourceContainer, sourcePath, "grabacion-sesion-1-parte-1.mp3", target, tenant);

        // Assert
        Assert.True(response.Success, response.Exception?.ToString() ?? response.Message);

        var destination = client.GetBlobContainerClient(tenant.ToString()).GetBlobClient($"{target}/grabacion-sesion-1-parte-1.mp3");
        var properties = (await destination.GetPropertiesAsync()).Value;
        var tags = (await destination.GetTagsAsync()).Value.Tags;

        Assert.Equal((CopyStatus.Success, "audio/mpeg", (long)content.Length), (properties.CopyStatus, properties.ContentType, response.File.Size));
        Assert.Equal(("grabacion-sesion-1-parte-1", "audio/mpeg", tenant.ToString()), (tags["Name"], tags["Mime"], tags["Tenant"]));
        Assert.Equal("audio/mpeg", properties.Metadata["Mime"]);
        Assert.True((await client.GetBlobContainerClient(SourceContainer).GetBlobClient(sourcePath).ExistsAsync()).Value);
    }

    [Fact]
    public async Task ImportAsync_AcsVideoRecording_CopiesItAsVideo()
    {
        // Arrange
        var sourcePath = await LeaveRecordingAsync("0-mixed.mp4", [1, 2, 3, 4]);
        var target = $"assembly-recordings/{Guid.NewGuid()}";

        // Act
        var response = await Provider().ImportAsync(SourceContainer, sourcePath, "grabacion-sesion-1-parte-1.mp4", target, tenant);

        // Assert
        Assert.True(response.Success, response.Exception?.ToString() ?? response.Message);

        var properties = (await client.GetBlobContainerClient(tenant.ToString()).GetBlobClient($"{target}/grabacion-sesion-1-parte-1.mp4").GetPropertiesAsync()).Value;

        Assert.Equal("video/mp4", properties.ContentType);
    }

    [Fact]
    public async Task ImportAsync_FileTypeWithoutMime_CopiesItAsBinary()
    {
        // Arrange
        var sourcePath = await LeaveRecordingAsync("0-recording.acsraw", [9, 9]);
        var target = $"assembly-recordings/{Guid.NewGuid()}";

        // Act
        var response = await Provider().ImportAsync(SourceContainer, sourcePath, "recording.acsraw", target, tenant);

        // Assert
        Assert.True(response.Success, response.Exception?.ToString() ?? response.Message);

        var tags = (await client.GetBlobContainerClient(tenant.ToString()).GetBlobClient($"{target}/recording.acsraw").GetTagsAsync()).Value.Tags;

        Assert.Equal("application/octet-stream", tags["Mime"]);
    }

    [Fact]
    public async Task ImportAsync_SourceMissing_SaysWhichFileIsMissing()
    {
        // Arrange
        await client.GetBlobContainerClient(SourceContainer).CreateIfNotExistsAsync();

        // Act
        var response = await Provider().ImportAsync(SourceContainer, $"{RecordingFolder}/missing/0-audiomp3.mp3", "grabacion-sesion-1-parte-1.mp3", "assembly-recordings/x", tenant);

        // Assert
        Assert.False(response.Success);
        Assert.Contains("missing/0-audiomp3.mp3", response.Message);
    }
}
