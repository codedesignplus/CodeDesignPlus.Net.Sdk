using CodeDesignPlus.Net.Core.Abstractions.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Exceptions;
using Serilog.Exceptions.Core;
using Serilog.Exceptions.EntityFrameworkCore.Destructurers;

namespace CodeDesignPlus.Net.Logger.Extensions;

/// <summary>
/// The <see cref="IHostBuilder"/> extensions for Serilog
/// </summary>
public static class HostBuilderExtension
{
    /// <summary>
    /// Add Serilog configuration options
    /// </summary>
    /// <param name="builder">The Microsoft.Extensions.Hosting.IHostBuilder to add the service to.</param>
    /// <param name="configureLogger">An optional action to configure the provided Microsoft.Extensions.Logging.ILoggingBuilder.</param>
    /// <returns>The Microsoft.Extensions.Hosting.IHostBuilder so that additional calls can be chained.</returns>
    public static IHostBuilder UseSerilog(this IHostBuilder builder, Action<LoggerConfiguration> configureLogger = null)
    {
        builder.UseSerilog((context, services, configuration) =>
        {
            var options = services.GetService<IOptions<CoreOptions>>();

            configuration
                .ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(services)
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .Enrich.WithThreadId()
                .Enrich.WithThreadName()
                .Enrich.WithProcessId()
                .Enrich.WithProcessName()
                .Enrich.WithEnvironmentUserName()
                .Enrich.WithProperty("AppName", options.Value.AppName)
                .Enrich.WithExceptionDetails(new DestructuringOptionsBuilder()
                    .WithDefaultDestructurers()
                    .WithDestructurers(new[] { new DbUpdateExceptionDestructurer() })
                )
                .Enrich.With(new ExceptionEnricher());

            configureLogger?.Invoke(configuration);
        });

        return builder;
    }
}
