using CodeDesignPlus.Net.File.Storage.Abstractions.Models;
using CodeDesignPlus.Net.File.Storage.Abstractions.Providers;
using Microsoft.Extensions.Hosting;
using M = CodeDesignPlus.Net.File.Storage.Abstractions.Models;

namespace CodeDesignPlus.Net.File.Storage.Providers;

public abstract class BaseProvider(ILogger logger, IHostEnvironment environment)
{
    protected readonly IHostEnvironment Environment = environment;
    protected readonly ILogger Logger = logger;

    protected async Task<Response> ProcessAsync(bool isEnable, string filename, TypeProviders typeProviders, Func<M.File, Response, Task<Response>> process)
    {
        if (!isEnable)
            return null;

        var file = new M.File(filename);
        var response = new Response(file, typeProviders);

        try
        {
            response = await process(file, response).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "An error occurred while processing the file {filename}", filename);

            response.Success = false;
            response.Message = ex.Message;

            if (Environment.IsDevelopment())
                response.Exception = ex;
        }

        return response;
    }


}
