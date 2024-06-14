using CodeDesignPlus.Net.Core.Abstractions.Options;
using CodeDesignPlus.Net.Logger.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Exceptions;
using Serilog.Exceptions.Core;
using Serilog.Exceptions.EntityFrameworkCore.Destructurers;
using Serilog.Sinks.OpenTelemetry;

namespace CodeDesignPlus.Net.Logger.Extensions;

/// <summary>
/// The <see cref="IHostBuilder"/> extensions for Serilog
/// </summary>
public static class HostBuilderExtension
{

    /// <summary>
    /// Add CodeDesignPlus.EFCore configuration options
    /// </summary>
    /// <param name="services">The Microsoft.Extensions.DependencyInjection.IServiceCollection to add the service to.</param>
    /// <param name="configuration">The configuration being bound.</param>
    /// <returns>The Microsoft.Extensions.DependencyInjection.IServiceCollection so that additional calls can be chained.</returns>
    public static IServiceCollection AddLogger(this IServiceCollection services, IConfiguration configuration)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (configuration == null)
            throw new ArgumentNullException(nameof(configuration));

        var section = configuration.GetSection(LoggerOptions.Section);

        if (!section.Exists())
            throw new Exceptions.LoggerException($"The section {LoggerOptions.Section} is required.");

        services
            .AddOptions<LoggerOptions>()
            .Bind(section)
            .ValidateDataAnnotations();


        var options = section.Get<LoggerOptions>();


        return services;
    }


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
            var coreOptions = services.GetService<IOptions<CoreOptions>>();
            var loggerOptions = services.GetService<IOptions<LoggerOptions>>();

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
                .Enrich.WithProperty("AppName", coreOptions.Value.AppName)
                .Enrich.WithExceptionDetails(new DestructuringOptionsBuilder()
                    .WithDefaultDestructurers()
                    .WithDestructurers([new DbUpdateExceptionDestructurer()])
                )
                .Enrich.With(new ExceptionEnricher());

            if (!string.IsNullOrEmpty(loggerOptions.Value.OTelEndpoint))
            {
                configuration.WriteTo.OpenTelemetry(options => {
                    options.Endpoint = loggerOptions.Value.OTelEndpoint;
                    options.Protocol = OtlpProtocol.Grpc;

                    options.IncludedData =
                        IncludedData.SpanIdField
                        | IncludedData.TraceIdField
                        | IncludedData.MessageTemplateTextAttribute
                        | IncludedData.MessageTemplateMD5HashAttribute;


                    options.BatchingOptions.BatchSizeLimit = 10;
                    //options.BatchingOptions.Period = TimeSpan.FromSeconds(1);
                    options.BatchingOptions.QueueLimit = 10;

                    options.ResourceAttributes = new Dictionary<string, object>
                    {
                        { "service.name", coreOptions.Value.AppName }
                    };

                });
            }

            configureLogger?.Invoke(configuration);
        });

        return builder;
    }
}
