using CodeDesignPlus.Net.File.Storage.Abstractions.Options;

namespace CodeDesignPlus.Net.File.Storage.Test.Helpers;

public static class OptionsUtil
{
    public static readonly FileStorageOptions FileStorageOptions = new()
    {
        AzureBlob = new() {
            AccountKey = "AccountKey",
            AccountName = "AccountName",
            DefaultEndpointsProtocol = "https",
            Enable = true,
            Uri = new Uri("https://localhost:44300"),
            UsePasswordLess = true
        },
        AzureFile = new () {
            AccountKey = "AccountKey",
            AccountName = "AccountName",
            DefaultEndpointsProtocol = "https",
            Enable = true,
            Folder = "docs/images",
        },
        Local = new()
        {
            Enable = true,
            Folder = "docs/images"
        }
    };

}
