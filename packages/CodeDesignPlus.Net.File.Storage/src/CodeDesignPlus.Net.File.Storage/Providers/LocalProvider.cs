using CodeDesignPlus.Net.File.Storage.Abstractions.Models;
using CodeDesignPlus.Net.File.Storage.Abstractions.Options;
using CodeDesignPlus.Net.File.Storage.Abstractions.Providers;
using CodeDesignPlus.Net.Security.Abstractions;
using Microsoft.Extensions.Hosting;

namespace CodeDesignPlus.Net.File.Storage;

public class LocalProvider<TKeyUser, TTenant>(
    IOptions<FileStorageOptions> options,
    ILogger<LocalProvider<TKeyUser, TTenant>> logger,
    IHostEnvironment environment,
    IUserContext<TKeyUser, TTenant> userContext
) : BaseProvider(logger, environment), ILocalProvider
{
    private readonly IUserContext<TKeyUser, TTenant> UserContext = userContext;
    private readonly FileStorageOptions Options = options.Value;

    public Task<Response> DownloadAsync(string file, string target, CancellationToken cancellationToken = default)
    {
        return base.ProcessAsync(new Abstractions.Models.File(file), TypeProviders.LocalProvider, async response =>
        {
            var path = System.IO.Path.Combine(this.Options.Local.Folder, this.UserContext.Tenant.ToString(), target, file);

            if (System.IO.File.Exists(path))
            {
                var memoryStream = new MemoryStream();
                var str = new FileStream(path, FileMode.Open);

                await str.CopyToAsync(memoryStream, cancellationToken: cancellationToken);

                str.Close();

                response.Stream = memoryStream;
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

    public Task<Response> UploadAsync(Stream stream, Abstractions.Models.File file, string target, CancellationToken cancellationToken = default)
    {
        return base.ProcessAsync(file, TypeProviders.LocalProvider, async response =>
        {
            var path = this.GetFullPath(file, target);

            using var fileStream = new FileStream(path, FileMode.Create);

            await stream.CopyToAsync(fileStream, cancellationToken: cancellationToken);


            file.Path = new Abstractions.Models.Path(this.Options.UriDownload, target, file.FullName, TypeProviders.LocalProvider);

            response.Success = System.IO.File.Exists(path);

            return response;
        });
    }

    public Task<Response> DeleteAsync(string file, string target, CancellationToken cancellationToken = default)
    {
        return base.ProcessAsync(new Abstractions.Models.File(file), TypeProviders.LocalProvider, response =>
        {
            var path = System.IO.Path.Combine(this.Options.Local.Folder, target, file);

            if (!System.IO.File.Exists(path))
            {
                response.Success = false;
                response.Message = "The system cannot find the file specified";
            }

            System.IO.File.Delete(path);

            response.Success = true;

            return Task.FromResult(response);
        });
    }

    private string GetPath(string target)
    {
        var path = System.IO.Path.Combine(this.Options.Local.Folder, this.UserContext.Tenant.ToString(), target);

        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        return path;
    }

    private string GetFullPath(Abstractions.Models.File file, string target)
    {
        var path = this.GetPath(target);

        if (!file.Renowned)
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
            file.Renowned = true;

            fileName = $@"{file.Name} ({count}){file.Extension}";

            fullPath = System.IO.Path.Combine(path, fileName);

            count += 1;
        }

        file.FullName = fileName;

        return fullPath;
    }
}

