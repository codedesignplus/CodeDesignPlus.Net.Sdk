using Azure.Storage.Blobs.Models;
using CodeDesignPlus.Net.File.Storage.Abstractions.Factories;
using CodeDesignPlus.Net.File.Storage.Abstractions.Models;
using CodeDesignPlus.Net.File.Storage.Abstractions.Providers;
using Microsoft.Extensions.Hosting;
using Semver;

namespace CodeDesignPlus.Net.File.Storage.Providers;

public class AzureBlobProvider(
    IAzureBlobFactory factory,
    ILogger<AzureBlobProvider> logger,
    IHostEnvironment environment
    ) : BaseProvider(logger, environment), IAzureBlobProvider
{
    private readonly IAzureBlobFactory factory = factory.Create();

    public Task<Response> DownloadAsync(string filename, string target, CancellationToken cancellationToken = default)
    {
        return base.ProcessAsync(factory.Options.AzureBlob.Enable, filename, TypeProviders.AzureBlobProvider, async (file, response) =>
        {
            var name = GetName(target, filename);

            var blobClient = this.factory.GetContainerClient().GetBlobClient(name);

            if (!await blobClient.ExistsAsync(cancellationToken).ConfigureAwait(false))
            {
                response.Success = false;
                response.Message = $"The file {filename} not exist in the container {this.factory.UserContext.Tenant}";

                return response;
            }

            var stream = new MemoryStream();

            await blobClient.DownloadToAsync(stream, cancellationToken: cancellationToken).ConfigureAwait(false);

            response.Stream = stream;
            response.Stream.Position = 0;
            response.Success = true;

            return response;
        });
    }

    public Task<Response> UploadAsync(Stream stream, string filename, string target, bool renowned = false, CancellationToken cancellationToken = default)
    {
        return base.ProcessAsync(factory.Options.AzureBlob.Enable, filename, TypeProviders.AzureBlobProvider, async (file, response) =>
        {
            var container = this.factory.GetContainerClient();

            await container.CreateIfNotExistsAsync(cancellationToken: cancellationToken).ConfigureAwait(false);

            var name = GetName(target, System.IO.Path.GetFileName(file.FullName));

            var blobClient = container.GetBlobClient(name);

            if (renowned)
            {
                var count = 1;

                while (await blobClient.ExistsAsync(cancellationToken).ConfigureAwait(false))
                {
                    count += 1;

                    file.Renowned = true;
                    file.Version = SemVersion.ParsedFrom(count, 0, 0);

                    name = GetName(target, $"{file.Name} ({count}){file.Extension}");

                    blobClient = container.GetBlobClient(name);
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
            }, cancellationToken: cancellationToken).ConfigureAwait(false);

            file.Size = stream.Length;
            file.Detail = new Abstractions.Models.FileDetail(this.factory.Options.UriDownload, target, System.IO.Path.GetFileName(name), TypeProviders.AzureBlobProvider);

            response.Success = true;

            return response;
        });
    }

    public Task<Response> DeleteAsync(string filename, string target, CancellationToken cancellationToken = default)
    {
        return base.ProcessAsync(factory.Options.AzureBlob.Enable, filename, TypeProviders.AzureBlobProvider, async (file, response) =>
        {
            var container = this.factory.GetContainerClient();

            var name = GetName(target, filename);

            var blobClient = container.GetBlobClient(name);

            var deleted = await blobClient.DeleteIfExistsAsync(cancellationToken: cancellationToken).ConfigureAwait(false);

            response.Success = deleted;

            if (!deleted)
                response.Message = $"The file {filename} not exist in the container {this.factory.UserContext.Tenant}";

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
