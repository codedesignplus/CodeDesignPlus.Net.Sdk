using CodeDesignPlus.Net.File.Storage.Abstractions.Options;

namespace CodeDesignPlus.Net.File.Storage.Test.Helpers;

public static class OptionsUtil
{
    public static readonly FileStorageOptions FileStorageOptions = new()
    {
        AzureBlob = new()
        {
            UsePasswordLess = false,
            DefaultEndpointsProtocol = "https",
            AccountName = "codedesignplusstorage",
            AccountKey = "4/e5AP+hRQ46e6oQwO9LgEq/ylB1bO3fyf2lAnb7BMFU5Mt8IH9eh/iziyqvnw0UD7sRh7NKVA+1+AStkJ7+6w==",
            EndpointSuffix = "core.windows.net",
            Enable = true
        },
        AzureFile = new()
        {
            AccountName = "codedesignplusstorage",
            AccountKey = "4/e5AP+hRQ46e6oQwO9LgEq/ylB1bO3fyf2lAnb7BMFU5Mt8IH9eh/iziyqvnw0UD7sRh7NKVA+1+AStkJ7+6w==",
            DefaultEndpointsProtocol = "https",
            Enable = true,
            Folder = "docs/images",
            EndpointSuffix = "core.windows.net",
        },
        Local = new()
        {
            Enable = true,
            Folder = "docs/images"
        },
        UriDownload = new Uri("https://localhost:5001/api/v1/file/download")
    };

}
