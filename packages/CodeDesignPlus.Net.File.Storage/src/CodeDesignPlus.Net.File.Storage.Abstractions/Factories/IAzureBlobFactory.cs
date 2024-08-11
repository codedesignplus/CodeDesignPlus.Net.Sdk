namespace CodeDesignPlus.Net.File.Storage.Abstractions.Factories;


public interface IAzureBlobFactory
{
    FileStorageOptions Options { get; }
    IUserContext UserContext { get; }
    BlobServiceClient Client { get; }
    IAzureBlobFactory Create();
    BlobContainerClient GetContainerClient();
}