using System.Reflection;
using CodeDesignPlus.Net.Core.Abstractions.Options;
using CodeDesignPlus.Net.Core.Exceptions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;


namespace CodeDesignPlus.Net.Microservice.Commons.EntryPoints.Rest.Swagger;

/// <summary>
/// Extensions for configuring Swagger in the service container.
/// </summary>
public static class SwaggerExtensions
{
    /// <summary>
    /// Adds and configures Swagger in the service container.
    /// </summary>
    /// <param name="services">The service container.</param>
    /// <param name="configuration">The configuration settings.</param>
    /// <returns>The service container with Swagger configured.</returns>
    public static IServiceCollection AddCoreSwagger(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        var section = configuration.GetSection(CoreOptions.Section);

        if (!section.Exists())
            throw new CoreException($"The section {CoreOptions.Section} is required.");

        var coreOptions = section.Get<CoreOptions>();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        var info = new OpenApiInfo()
        {
            Title = coreOptions!.AppName,
            Version = coreOptions!.Version,
            Description = coreOptions!.Description,
            Contact = new OpenApiContact()
            {
                Name = coreOptions!.Contact.Name,
                Email = coreOptions!.Contact.Email,
            }
        };

        services.AddSwaggerGen(x =>
        {
            x.SwaggerDoc(coreOptions.Version, info);

            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";

            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

            x.IncludeXmlComments(xmlPath);

            x.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please enter a valid token",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = "Bearer"
            });

            x.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type=ReferenceType.SecurityScheme,
                            Id="Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });

        return services;
    }

    /// <summary>
    /// Configures the application to use Swagger.
    /// </summary>
    /// <param name="app">The application builder.</param>
    /// <returns>The application builder with Swagger configured.</returns>
    public static IApplicationBuilder UseCoreSwagger(this IApplicationBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app);

        var options = app.ApplicationServices.GetRequiredService<IOptions<CoreOptions>>().Value;

        app.UseSwagger();

        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", $"{options.AppName} {options.Version}");
        });

        return app;
    }
}