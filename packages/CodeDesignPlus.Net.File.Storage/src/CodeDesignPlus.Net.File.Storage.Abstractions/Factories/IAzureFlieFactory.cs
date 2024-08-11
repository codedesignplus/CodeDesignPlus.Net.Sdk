namespace CodeDesignPlus.Net.File.Storage.Abstractions.Factories;

public interface IAzureFlieFactory
{
    FileStorageOptions Options { get; }
    IUserContext UserContext { get; }
    ShareServiceClient Client { get; }
    IAzureFlieFactory Create();
    ShareClient GetContainerClient();
}