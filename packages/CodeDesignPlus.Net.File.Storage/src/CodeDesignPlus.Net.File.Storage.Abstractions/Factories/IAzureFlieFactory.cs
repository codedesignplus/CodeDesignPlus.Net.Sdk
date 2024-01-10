using Azure.Storage.Files.Shares;
using CodeDesignPlus.Net.File.Storage.Abstractions.Options;
using CodeDesignPlus.Net.Security.Abstractions;

namespace CodeDesignPlus.Net.File.Storage.Abstractions.Factories;

public interface IAzureFlieFactory<TKeyUser, TTenant>
{
    FileStorageOptions Options { get; }
    IUserContext<TKeyUser, TTenant> UserContext { get; }
    ShareServiceClient Client { get; }
    IAzureFlieFactory<TKeyUser, TTenant> Create();
    ShareClient GetContainerClient();
}