using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Console;

namespace CodeDesignPlus.Net.xUnit.Helpers.Loggers;

public static class XunitExtensions
{
    public  static bool UsesScopes(this ILoggingBuilder builder)
    {
        var serviceProvider = builder.Services.BuildServiceProvider();

        // look for other host builders on this chain calling ConfigureLogging explicitly
        var options = serviceProvider.GetService<SimpleConsoleFormatterOptions>() ??
                      serviceProvider.GetService<JsonConsoleFormatterOptions>() ??
                      serviceProvider.GetService<ConsoleFormatterOptions>();

        if (options != default)
            return options.IncludeScopes;

        // look for other configuration sources
        // See: https://docs.microsoft.com/en-us/dotnet/core/extensions/logging?tabs=command-line#set-log-level-by-command-line-environment-variables-and-other-configuration

        var config = serviceProvider.GetService<IConfigurationRoot>() ?? serviceProvider.GetService<IConfiguration>();
        var logging = config?.GetSection("Logging");
        if (logging == default)
            return false;

        var includeScopes = logging?.GetValue("Console:IncludeScopes", false);
        if (!includeScopes.Value)
            includeScopes = logging?.GetValue("IncludeScopes", false);

        return includeScopes.GetValueOrDefault(false);
    }

}
