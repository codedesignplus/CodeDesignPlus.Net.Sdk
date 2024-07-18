namespace CodeDesignPlus.Net.File.Storage.Services;

public class FileStorageService(IEnumerable<IProvider> providers) : IFileStorageService
{

    public Task<M.Response[]> DeleteAsync(string file, string target, CancellationToken cancellationToken = default)
    {
        var tasks = new List<Task<M.Response>>();

        foreach (var provider in providers)
        {
            var task = provider.DeleteAsync(file, target, cancellationToken);

            tasks.Add(task);
        }

        return Task.WhenAll(tasks);
    }

    public Task<M.Response> DownloadAsync(string file, string target, CancellationToken cancellationToken = default)
    {
        foreach (var provider in providers)
        {
            var response = provider.DownloadAsync(file, target, cancellationToken);

            if (response.Result.Success)
                return response;
        }

        return Task.FromResult((M.Response)null);
    }

    public Task<M.Response[]> UploadAsync(Stream stream, string file, string target, bool renowned, CancellationToken cancellationToken = default)
    {
        var tasks = new List<Task<M.Response>>();

        foreach (var provider in providers)
        {
            var task = provider.UploadAsync(stream, file, target, renowned, cancellationToken);

            tasks.Add(task);
        }

        return Task.WhenAll(tasks);
    }

}
