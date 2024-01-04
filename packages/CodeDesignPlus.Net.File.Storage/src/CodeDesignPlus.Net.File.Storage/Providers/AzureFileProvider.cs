using CodeDesignPlus.Net.File.Storage.Abstractions.Factories;
using CodeDesignPlus.Net.File.Storage.Abstractions.Options;
using CodeDesignPlus.Net.File.Storage.Abstractions.Providers;
using CodeDesignPlus.Net.File.Storage.Factories;
using CodeDesignPlus.Net.Security.Abstractions;
using Microsoft.Extensions.Hosting;
using Semver;
using M = CodeDesignPlus.Net.File.Storage.Abstractions.Models;

namespace CodeDesignPlus.Net.File.Storage.Providers;

public class AzureFileProvider<TKeyUser, TTenant>(
    IAzureFlieFactory<TKeyUser, TTenant> factory,
    ILogger<AzureFileProvider<TKeyUser, TTenant>> logger,
    IHostEnvironment environment
) : BaseProvider(logger, environment), IAzureFileProvider
{
    private readonly IAzureFlieFactory<TKeyUser, TTenant> factory = factory.Create();

    public Task<M.Response> DownloadAsync(string file, string target, CancellationToken cancellationToken = default)
    {
        return base.ProcessAsync(new Abstractions.Models.File(file), TypeProviders.AzureFileProvider, async response =>
        {
            var directory = this.factory.GetContainerClient().GetDirectoryClient(target);

            var fileClient = directory.GetFileClient(file);

            if (!await fileClient.ExistsAsync(cancellationToken))
            {
                response.Success = false;
                response.Message = $"The file {file} not exist in the container {this.factory.UserContext.Tenant}";

                return response;
            }

            var download = await fileClient.DownloadAsync(cancellationToken: cancellationToken);

            using var stream = new MemoryStream();

            download.Value.Content.CopyTo(stream);

            response.Stream = stream;
            response.Stream.Position = 0;
            response.Success = true;

            return response;
        });

    }

    public Task<M.Response> UploadAsync(Stream stream, M.File file, string target, CancellationToken cancellationToken = default)
    {
        return base.ProcessAsync(file, TypeProviders.AzureFileProvider, async response =>
        {
            var sharedClient = this.factory.GetContainerClient();

            await sharedClient.CreateIfNotExistsAsync(cancellationToken: cancellationToken);

            var directory = sharedClient.GetDirectoryClient(target);

            if (!await directory.ExistsAsync(cancellationToken))
            {
                var pathParts = target.Split('/');
                var currentPath = string.Empty;

                for (int i = 0; i < pathParts.Length; i++)
                {
                    currentPath += pathParts[i] + '/';

                    directory = sharedClient.GetDirectoryClient(currentPath);

                    await directory.CreateIfNotExistsAsync(cancellationToken: cancellationToken);
                }
            }

            var name = System.IO.Path.GetFileName(file.FullName);
            var fileClient = directory.GetFileClient(name);

            if (file.Renowned)
            {
                var count = 1;

                while (await fileClient.ExistsAsync(cancellationToken))
                {
                    file.Renowned = true;
                    file.Version = SemVersion.ParsedFrom(count, 0, 0);

                    name = $"{file.Name} ({count}){file.Extension}";

                    fileClient = directory.GetFileClient(name);

                    count += 1;
                }
            }

            await fileClient.CreateAsync(stream.Length, metadata: file.GetMetadata(this.factory.Options.UriDownload), cancellationToken: cancellationToken);

            await fileClient.UploadAsync(stream, cancellationToken: cancellationToken);

            file.Size = stream.Length;
            file.Path = new M.Path(this.factory.Options.UriDownload, target, name, TypeProviders.AzureBlobProvider);

            response.Success = true;

            return response;
        });
    }

    public Task<M.Response> DeleteAsync(string file, string target, CancellationToken cancellationToken = default)
    {
        return base.ProcessAsync(new Abstractions.Models.File(file), TypeProviders.AzureFileProvider, async response =>
        {
            var directory = this.factory.GetContainerClient().GetDirectoryClient(target);

            var fileClient = directory.GetFileClient(file);

            var download = await fileClient.DeleteIfExistsAsync(cancellationToken: cancellationToken);

            response.Success = download;

            if (!download)
                response.Message = $"The file {file} not exist in the container {this.factory.UserContext.Tenant}";

            return response;
        });

    }
}
