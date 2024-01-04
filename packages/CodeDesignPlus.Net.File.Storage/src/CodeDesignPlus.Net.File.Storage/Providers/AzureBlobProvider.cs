using Azure.Storage.Blobs.Models;
using CodeDesignPlus.Net.File.Storage.Abstractions.Factories;
using CodeDesignPlus.Net.File.Storage.Abstractions.Models;
using CodeDesignPlus.Net.File.Storage.Abstractions.Providers;
using CodeDesignPlus.Net.File.Storage.Factories;
using Microsoft.Extensions.Hosting;
using Semver;

namespace CodeDesignPlus.Net.File.Storage.Providers;

public class AzureBlobProvider<TKeyUser, TTenant>(
    IAzureBlobFactory<TKeyUser, TTenant> factory,
    ILogger<AzureBlobProvider<TKeyUser, TTenant>> logger,
    IHostEnvironment environment
    ) : BaseProvider(logger, environment), IAzureBlobProvider
{
    private readonly IAzureBlobFactory<TKeyUser, TTenant> factory  = factory.Create();

    public Task<Response> DownloadAsync(string file, string target, CancellationToken cancellationToken = default)
    {
        return base.ProcessAsync(new Abstractions.Models.File(file), TypeProviders.AzureBlobProvider, async response =>
        {
            var name = GetName(target, file);

            var blobClient = this.factory.GetContainerClient().GetBlobClient(name);

            if (!await blobClient.ExistsAsync(cancellationToken))
            {
                response.Success = false;
                response.Message = $"The file {file} not exist in the container {this.factory.UserContext.Tenant}";

                return response;
            }

            using var stream = new MemoryStream();

            var download = await blobClient.DownloadToAsync(stream, cancellationToken: cancellationToken);

            response.Stream = stream;
            response.Stream.Position = 0;
            response.Success = true;

            return response;
        });
    }

    public Task<Response> UploadAsync(Stream stream, Abstractions.Models.File file, string target, CancellationToken cancellationToken = default)
    {
        return base.ProcessAsync(file, TypeProviders.AzureBlobProvider, async response =>
        {
            var container = this.factory.GetContainerClient();

            await container.CreateIfNotExistsAsync(cancellationToken: cancellationToken);

            var name = GetName(target, System.IO.Path.GetFileName(file.FullName));

            var blobClient = container.GetBlobClient(name);

            if (file.Renowned)
            {
                var count = 1;

                while (await blobClient.ExistsAsync(cancellationToken))
                {
                    file.Renowned = true;
                    file.Version = SemVersion.ParsedFrom(count, 0, 0);

                    name = GetName(target, $"{file.Name} ({count}){file.Extension}");

                    blobClient = container.GetBlobClient(name);

                    count += 1;
                }
            }

            await blobClient.UploadAsync(stream, new BlobUploadOptions
            {
                AccessTier = AccessTier.Hot,
                Metadata = file.GetMetadata(this.factory.Options.UriDownload),
                Tags = file.GetTags(this.factory.UserContext.Tenant),
                HttpHeaders = new BlobHttpHeaders
                {
                    ContentType = file.Mime.MimeType
                },
            }, cancellationToken: cancellationToken);

            file.Size = stream.Length;
            file.Path = new Abstractions.Models.Path(this.factory.Options.UriDownload, target, name, TypeProviders.AzureBlobProvider);

            response.Success = true;

            return response;
        });
    }

    public Task<Response> DeleteAsync(string file, string target, CancellationToken cancellationToken = default)
    {
        return base.ProcessAsync(new Abstractions.Models.File(file), TypeProviders.AzureBlobProvider, async response =>
        {
            var container = this.factory.GetContainerClient();

            var name = GetName(target, file);

            var blobClient = container.GetBlobClient(name);

            var download = await blobClient.DeleteIfExistsAsync(cancellationToken: cancellationToken);

            response.Success = download;

            if (!download)
                response.Message = $"The file {file} not exist in the container {this.factory.UserContext.Tenant}";

            return response;
        });

    }

    protected string GetName(string target, string name)
    {
        if (string.IsNullOrEmpty(target))
            return name;

        return $"{target}/{name}";
    }
}
