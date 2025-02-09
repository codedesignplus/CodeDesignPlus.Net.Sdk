﻿namespace CodeDesignPlus.Net.xUnit.Output.Loggers;

/// <summary>
/// Provides extension methods for xUnit logging.
/// </summary>
public static class XunitExtensions
{
    /// <summary>
    /// Determines whether the logging builder uses scopes.
    /// </summary>
    /// <param name="builder">The logging builder.</param>
    /// <returns><c>true</c> if the logging builder uses scopes; otherwise, <c>false</c>.</returns>
    public static bool UsesScopes(this ILoggingBuilder builder)
    {
        var serviceProvider = builder.Services.BuildServiceProvider();

        // Look for other host builders on this chain calling ConfigureLogging explicitly
        var options = serviceProvider.GetService<SimpleConsoleFormatterOptions>() ??
                      serviceProvider.GetService<JsonConsoleFormatterOptions>() ??
                      serviceProvider.GetService<ConsoleFormatterOptions>();

        if (options != default)
            return options.IncludeScopes;

        // Look for other configuration sources
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