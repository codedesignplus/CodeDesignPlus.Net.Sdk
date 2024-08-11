using CodeDesignPlus.Net.File.Storage.Abstractions.Providers;
using CodeDesignPlus.Net.File.Storage.Providers;
using Microsoft.Extensions.Hosting;
using M = CodeDesignPlus.Net.File.Storage.Abstractions.Models;

namespace CodeDesignPlus.Net.File.Storage.Test.Helpers;

public class FakeProvider(ILogger logger, IHostEnvironment environment) : BaseProvider(logger, environment)
{

    public Task<M.Response> InvokeProcessAsync(bool enable, string filename, TypeProviders typeProviders, Func<M.File, M.Response, Task<M.Response>> process)
    {
        Logger.LogInformation("ProcessAsync");

        return ProcessAsync(enable, filename, typeProviders, process);
    }
}
