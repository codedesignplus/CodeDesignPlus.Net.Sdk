using CodeDesignPlus.Net.File.Storage.Abstractions.Models;
using CodeDesignPlus.Net.File.Storage.Abstractions.Options;
using CodeDesignPlus.Net.File.Storage.Abstractions.Providers;

namespace CodeDesignPlus.Net.File.Storage.Services;

public class FileStorageService(ILogger<FileStorageService> logger, IOptions<FileStorageOptions> options, IEnumerable<IProvider> providers) : IFileStorageService
{
    private readonly ILogger<FileStorageService> logger = logger;
    private readonly FileStorageOptions options = options.Value;
    private readonly IEnumerable<IProvider> providers = providers;


    public Task<Response[]> DeleteAsync(string file, string target, CancellationToken cancellationToken = default)
    {        
        var tasks = new List<Task<Response>>();

        foreach (var provider in this.providers)
        {
            var task = provider.DeleteAsync(file, target, cancellationToken);

            tasks.Add(task);
        }

        return Task.WhenAll(tasks);
    }

    public Task<Response> DownloadAsync(string file, string target, CancellationToken cancellationToken = default)
    {
        foreach (var provider in this.providers)
        {
            var response = provider.DownloadAsync(file, target, cancellationToken);

            if (response.Result.Success)
                return response;
        }

        return Task.FromResult((Response)null);
    }

    public Task<Response[]> UploadAsync(Stream stream, string filename, string target, bool renowned, CancellationToken cancellationToken = default)
    {
        var tasks = new List<Task<Response>>();

        foreach (var provider in this.providers)
        {
            var task = provider.UploadAsync(stream, filename, target, renowned, cancellationToken);

            tasks.Add(task);
        }

        return Task.WhenAll(tasks);
    }

}
