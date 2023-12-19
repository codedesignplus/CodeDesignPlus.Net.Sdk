using Azure.Identity;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using CodeDesignPlus.Net.File.Storage.Abstractions.Models;
using CodeDesignPlus.Net.File.Storage.Abstractions.Options;
using CodeDesignPlus.Net.File.Storage.Abstractions.Providers;
using Microsoft.Extensions.Hosting;

namespace CodeDesignPlus.Net.File.Storage.Providers;

public class AzureBlobProvider<TTenant> : IAzureBlobProvider<TTenant>
{
    private readonly FileStorageOptions fileOptions;
    private readonly IHostEnvironment environment;
    private readonly ILogger<AzureBlobProvider<TTenant>> logger;

    public AzureBlobProvider(IOptions<FileStorageOptions> options, ILogger<AzureBlobProvider<TTenant>> logger, IHostEnvironment environment)
    {
        if (environment is null)
            throw new ArgumentNullException(nameof(environment));

        if (options is null)
            throw new ArgumentNullException(nameof(options));

        this.fileOptions = options.Value;
        this.environment = environment;
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public Task<Response> ReadFileAsync(TTenant tenant, string file, string target)
    {
        throw new NotImplementedException();
    }

    public async Task<IList<Response>> WriteFileAsync(TTenant tenant, Stream stream, string target, Abstractions.Models.File file)
    {
        var response = new Response(file, TypeProviders.AzureBlobProvider);

        var container = this.fileOptions.AzureBlob.BlobServiceClient.GetBlobContainerClient(tenant.ToString());

        await container.CreateIfNotExistsAsync(); //PublicAccessType.BlobContainer

        var name = GetName(target, file);

        var blobClient = container.GetBlobClient(name);

        await blobClient.UploadAsync(stream, new BlobUploadOptions
        {
            AccessTier = AccessTier.Hot,
            Metadata = file.GetMetadata(this.fileOptions.UriDownload),
            Tags = file.GetTags(tenant),
            HttpHeaders = new BlobHttpHeaders
            {
                ContentType = file.Mime.MimeType
            },
        });

        return null;
    }

    private static string GetName(string target, Abstractions.Models.File file)
    {
        if (string.IsNullOrEmpty(target))
            return file.Name;

        return $"{target}/{file.Name}";
    }

    // private async Task<CloudBlockBlob> GetNameAsync(IUpload data)
    // {
    //     if (data.Overwrite)
    //     {
    //         return await this.storage.GetBlobAsync(data.IdClient, data.IdCampaign, data.Target, data.FileInfo.Name);
    //     }
    //     else
    //     {
    //         return await this.GetNextNameAsync(data);
    //     }
    // }


    // private async Task<string> GetNextNameAsync(TTenant tenant, Abstractions.Models.File file)
    // {
    //     var fileName = file.Name;

    //     var container = this.fileOptions.AzureBlob.BlobServiceClient.GetBlobContainerClient(tenant.ToString());

    //     var count = 1;

    //     while (await container.ExistsAsync())
    //     {
    //         file.Renowned = true;

    //         fileName = $"{file.Name} ({count}){file.Extension}";

    //         file = //await container.GetBlobClient(fileName);

    //         count += 1;
    //     }

    //     return file;
    // }
}
