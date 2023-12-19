using CodeDesignPlus.Net.File.Storage.Abstractions.Models;
using CodeDesignPlus.Net.File.Storage.Abstractions.Options;
using CodeDesignPlus.Net.File.Storage.Abstractions.Providers;
using Microsoft.Extensions.Hosting;

namespace CodeDesignPlus.Net.File.Storage;

public class LocalProvider<TTenant> : ILocalProvider<TTenant>
{
    private readonly FileStorageOptions fileOptions;
    private readonly IHostEnvironment environment;
    private readonly ILogger<LocalProvider<TTenant>> logger;

    public LocalProvider(IOptions<FileStorageOptions> options, ILogger<LocalProvider<TTenant>> logger, IHostEnvironment environment)
    {
        if (environment is null)
            throw new ArgumentNullException(nameof(environment));

        if (options is null)
            throw new ArgumentNullException(nameof(options));

        this.fileOptions = options.Value;
        this.environment = environment;
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Response> ReadFileAsync(TTenant tenant, string file, string target)
    {
        var response = new Response(new Abstractions.Models.File(file), TypeProviders.LocalProvider);

        try
        {
            var path = System.IO.Path.Combine(this.fileOptions.Local.Folder, target, file);

            if (System.IO.File.Exists(path))
            {
                var memoryStream = new MemoryStream();
                var str = new FileStream(path, FileMode.Open);

                await str.CopyToAsync(memoryStream);

                str.Close();

                response.Stream = memoryStream;
                response.Success = true;
            }
            else
            {
                response.Success = false;
                response.Message = "The system cannot find the file specified";
            }
        }
        catch (Exception e_ex)
        {
            response.Success = false;
            response.Message = e_ex.Message;

            if (environment.IsDevelopment())
                response.Exception = e_ex;

            this.logger.LogError(e_ex, "The file could not be read");
        }

        return response;
    }

    public async Task<IList<Response>> WriteFileAsync(TTenant tenant, Stream stream, string target, Abstractions.Models.File file)
    {
        var result = new Response(file, TypeProviders.LocalProvider);

        try
        {
            var path = this.GetFullPath(file, target);

            using var fileStream = new FileStream(path, FileMode.Create);

            await stream.CopyToAsync(fileStream);


            file.Path = new Abstractions.Models.Path(this.fileOptions.UriDownload, target, file.FullName, TypeProviders.LocalProvider);

            result.Success = System.IO.File.Exists(path);
        }
        catch (Exception e_ex)
        {
            result.Success = false;
            result.Message = e_ex.Message;

            if (environment.IsDevelopment())
                result.Exception = e_ex;

            this.logger.LogError(e_ex, "The file could not be written");
        }

        return new List<Response>() { result };
    }

    private string GetPath(string target)
    {
        var path = System.IO.Path.Combine(this.fileOptions.Local.Folder, target);

        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        return path;
    }

    private string GetFullPath(Abstractions.Models.File file, string target)
    {
        var path = this.GetPath(target);

        if (file.Overwrite)
        {
            return System.IO.Path.Combine(path, file.FullName);
        }
        else
        {
            return GetNextName(file, path);
        }
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

