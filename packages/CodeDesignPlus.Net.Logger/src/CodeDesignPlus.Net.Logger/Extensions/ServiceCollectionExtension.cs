using Serilog.Events;

namespace CodeDesignPlus.Net.Logger.Extensions;

/// <summary>
/// Provides extension methods for adding and configuring logging services.
/// </summary>
public static class ServiceCollectionExtension
{
    /// <summary>
    /// Adds logging services to the specified IServiceCollection.
    /// </summary>
    /// <param name="services">The IServiceCollection to add the services to.</param>
    /// <param name="configuration">The configuration being bound.</param>
    /// <returns>The IServiceCollection so that additional calls can be chained.</returns>
    /// <exception cref="ArgumentNullException">Thrown when services or configuration is null.</exception>
    /// <exception cref="Exceptions.LoggerException">Thrown when the LoggerOptions section is missing.</exception>
    public static IServiceCollection AddLogger(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        var section = configuration.GetSection(LoggerOptions.Section);

        if (!section.Exists())
            throw new Exceptions.LoggerException($"The section {LoggerOptions.Section} is required.");

        services
            .AddOptions<LoggerOptions>()
            .Bind(section)
            .ValidateDataAnnotations();

        services.AddCore(configuration);

        return services;
    }

    /// <summary>
    /// Configures Serilog for the specified IHostBuilder.
    /// </summary>
    /// <param name="builder">The IHostBuilder to configure.</param>
    /// <param name="configureLogger">An optional action to configure the LoggerConfiguration.</param>
    /// <returns>The IHostBuilder so that additional calls can be chained.</returns>
    /// <exception cref="ArgumentNullException">Thrown when builder is null.</exception>
    public static IHostBuilder UseSerilog(this IHostBuilder builder, Action<LoggerConfiguration> configureLogger = null)
    {
        builder.UseSerilog((context, services, configuration) =>
        {
            var coreOptions = services.GetRequiredService<IOptions<CoreOptions>>();
            var environment = services.GetRequiredService<IHostEnvironment>();
            var loggerOptions = context.Configuration.GetRequiredSection(LoggerOptions.Section).Get<LoggerOptions>();

            var levelLogger = ConvertToSerilogLevel(loggerOptions.Level);

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
                .Enrich.With(new ExceptionEnricher())
                .MinimumLevel.Is(levelLogger);

            if(environment.IsDevelopment())
                configuration.WriteTo.Console(restrictedToMinimumLevel: levelLogger);

            if (loggerOptions.Enable)
            {
                configuration.WriteTo.OpenTelemetry(options =>
                {
                    options.Endpoint = loggerOptions.OTelEndpoint;
                    options.Protocol = OtlpProtocol.Grpc;

                    options.IncludedData =
                        IncludedData.SpanIdField
                        | IncludedData.TraceIdField
                        | IncludedData.MessageTemplateTextAttribute
                        | IncludedData.MessageTemplateMD5HashAttribute;

                    options.BatchingOptions.BatchSizeLimit = 10;
                    options.BatchingOptions.QueueLimit = 10;

                    options.ResourceAttributes = new Dictionary<string, object>
                    {
                        { "service.name", coreOptions.Value.AppName },
                        { "service.version", coreOptions.Value.Version },
                        { "service.description", coreOptions.Value.Description },
                        { "service.business", coreOptions.Value.Business },
                        { "service.contact.name", coreOptions.Value.Contact.Name },
                        { "service.contact.email", coreOptions.Value.Contact.Email }
                    };
                });
            }

            configureLogger?.Invoke(configuration);
        });

        return builder;
    }

    /// <summary>
    /// Converts a string log level to a Serilog LogEventLevel.
    /// </summary>
    /// <param name="logLevel">The string representation of the log level.</param>
    /// <returns>The corresponding Serilog LogEventLevel.</returns>
    private static LogEventLevel ConvertToSerilogLevel(string logLevel)
    {
        if(string.IsNullOrEmpty(logLevel))
            return LogEventLevel.Error; 

        return Enum.Parse<LogEventLevel>(logLevel); 
    }
}