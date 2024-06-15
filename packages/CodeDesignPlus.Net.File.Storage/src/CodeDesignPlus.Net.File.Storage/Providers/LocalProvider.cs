using CodeDesignPlus.Net.File.Storage.Abstractions.Models;
using CodeDesignPlus.Net.File.Storage.Abstractions.Options;
using CodeDesignPlus.Net.File.Storage.Abstractions.Providers;
using CodeDesignPlus.Net.Security.Abstractions;
using Microsoft.Extensions.Hosting;

namespace CodeDesignPlus.Net.File.Storage;

public class LocalProvider(
    IOptions<FileStorageOptions> options,
    ILogger<LocalProvider> logger,
    IHostEnvironment environment,
    IUserContext userContext
) : BaseProvider(logger, environment), ILocalProvider
{
    private readonly IUserContext UserContext = userContext;
    private readonly FileStorageOptions options = options.Value;

    public Task<Response> DownloadAsync(string filename, string target, CancellationToken cancellationToken = default)
    {
        return base.ProcessAsync(this.options.Local.Enable, filename, TypeProviders.LocalProvider, async (file, response) =>
        {
            var path = System.IO.Path.Combine(this.options.Local.Folder, this.UserContext.Tenant.ToString(), target, filename);

            if (System.IO.File.Exists(path))
            {
                var memoryStream = new MemoryStream();
                var str = new FileStream(path, FileMode.Open);

                await str.CopyToAsync(memoryStream, cancellationToken: cancellationToken).ConfigureAwait(false);

                str.Close();

                response.Stream = memoryStream;
                response.Stream.Position = 0;
                response.Success = true;
            }
            else
            {
                response.Success = false;
                response.Message = "The system cannot find the file specified";
            }

            return response;
        });
    }

    public Task<Response> UploadAsync(Stream stream, string filename, string target, bool renowned = false, CancellationToken cancellationToken = default)
    {
        return base.ProcessAsync(this.options.Local.Enable, filename, TypeProviders.LocalProvider, async (file, response) =>
        {
            var path = this.GetFullPath(file, target, renowned);

            using var fileStream = new FileStream(path, FileMode.Create);

            await stream.CopyToAsync(fileStream, cancellationToken: cancellationToken).ConfigureAwait(false);

            file.Detail = new Abstractions.Models.FileDetail(this.options.UriDownload, target, file.FullName, TypeProviders.LocalProvider);

            response.Success = System.IO.File.Exists(path);

            return response;
        });
    }

    public Task<Response> DeleteAsync(string filename, string target, CancellationToken cancellationToken = default)
    {
        return base.ProcessAsync(this.options.Local.Enable, filename, TypeProviders.LocalProvider, (file, response) =>
        {
            var path = System.IO.Path.Combine(this.GetPath(target), filename);

            if (!System.IO.File.Exists(path))
            {
                response.Success = false;
                response.Message = "The system cannot find the file specified";

                return Task.FromResult(response);
            }

            System.IO.File.Delete(path);

            response.Success = true;

            return Task.FromResult(response);
        });
    }

    private string GetPath(string target)
    {
        var path = System.IO.Path.Combine(this.options.Local.Folder, this.UserContext.Tenant.ToString(), target);

        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        return path;
    }

    private string GetFullPath(Abstractions.Models.File file, string target, bool renowned)
    {
        var path = this.GetPath(target);

        if (!renowned)
            return System.IO.Path.Combine(path, file.FullName);

        return GetNextName(file, path);
    }

    private static string GetNextName(Abstractions.Models.File file, string path)
    {
        var fileName = file.Name;
        var fullPath = System.IO.Path.Combine(path, fileName);
        var count = 1;

        while (System.IO.File.Exists(fullPath))
        {
            count += 1;

            file.Renowned = true;

            fileName = $@"{file.Name} ({count}){file.Extension}";

            fullPath = System.IO.Path.Combine(path, fileName);
        }

        file.FullName = fileName;

        return fullPath;
    }
}

