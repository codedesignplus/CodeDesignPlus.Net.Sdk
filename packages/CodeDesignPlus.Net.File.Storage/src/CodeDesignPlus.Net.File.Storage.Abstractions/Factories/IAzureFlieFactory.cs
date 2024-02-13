using Azure.Storage.Files.Shares;
using CodeDesignPlus.Net.File.Storage.Abstractions.Options;
using CodeDesignPlus.Net.Security.Abstractions;

namespace CodeDesignPlus.Net.File.Storage.Abstractions.Factories;

public interface IAzureFlieFactory
{
    FileStorageOptions Options { get; }
    IUserContext UserContext { get; }
    ShareServiceClient Client { get; }
    IAzureFlieFactory Create();
    ShareClient GetContainerClient();
}