using Azure.Storage.Blobs;
using CodeDesignPlus.Net.File.Storage.Abstractions.Options;
using CodeDesignPlus.Net.Security.Abstractions;

namespace CodeDesignPlus.Net.File.Storage.Abstractions.Factories;


public interface IAzureBlobFactory<TKeyUser, TTenant>
{
    FileStorageOptions Options { get; }
    IUserContext<TKeyUser, TTenant> UserContext { get; }
    BlobServiceClient Client { get; }
    IAzureBlobFactory<TKeyUser, TTenant> Create();
    BlobContainerClient GetContainerClient();
}