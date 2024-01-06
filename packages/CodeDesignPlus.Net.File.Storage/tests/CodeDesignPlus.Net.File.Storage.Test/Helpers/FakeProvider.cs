using M = CodeDesignPlus.Net.File.Storage.Abstractions.Models;
using CodeDesignPlus.Net.File.Storage.Abstractions.Providers;
using Microsoft.Extensions.Hosting;

namespace CodeDesignPlus.Net.File.Storage.Test;

public class FakeProvider(ILogger logger, IHostEnvironment environment) : BaseProvider(logger, environment)
{

    public Task<M.Response> InvokeProcessAsync(bool enable, string filename, TypeProviders typeProviders, Func<M.File, M.Response, Task<M.Response>> process)
    {
        this.Logger.LogInformation("ProcessAsync");

        return this.ProcessAsync(enable, filename, typeProviders, process);
    }
}
