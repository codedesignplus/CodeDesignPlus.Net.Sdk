using CodeDesignPlus.Net.Core.Abstractions.Options;
using CodeDesignPlus.Net.Core.Exceptions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;


namespace CodeDesignPlus.Net.Microservice.Commons.EntryPoints.Rest.Swagger;

/// <summary>
/// Extensions for configuring Swagger in the service container.
/// </summary>
public static class SwaggerExtensions
{
    /// <summary>
    /// Adds and configures Swagger in the service container.
    /// </summary>
    /// <typeparam name="TProgram">The type of the program class.</typeparam>
    /// <param name="services">The service container.</param>
    /// <param name="configuration">The configuration settings.</param>
    /// <returns>The service container with Swagger configured.</returns>
    public static IServiceCollection AddCoreSwagger<TProgram>(this IServiceCollection services, IConfiguration configuration) where TProgram : class
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

            var xmlFile = $"{typeof(TProgram).Assembly.GetName().Name}.xml";

            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

            // Sin el XML el Swagger se genera igual, sin las descripciones; antes fallaba la generación entera.
            if (File.Exists(xmlPath))
                x.IncludeXmlComments(xmlPath);

            // El contrato que se describe es el que serializa el micro, y el micro serializa con Newtonsoft
            // (AddSwaggerGenNewtonsoftSupport, abajo). Estos filtros cubren lo que ese soporte no describe bien
            // (pendings/753).
            x.MapNodaTimeTypes();
            x.SchemaFilter<ConstructorParameterSchemaFilter>();
            x.SchemaFilter<EnumNamesSchemaFilter>();
            x.SchemaFilter<MoneySchemaFilter>();
            x.SchemaFilter<FluentValidationSchemaFilter>();
            x.OperationFilter<TenantHeaderOperationFilter>();

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

        // Swashbuckle describe los tipos por defecto con System.Text.Json, pero los micros serializan con Newtonsoft
        // (AddNewtonsoftJson en los 37 REST). Sin esto el Swagger ignoraba [Newtonsoft.Json.JsonIgnore] y
        // [JsonProperty] y obedecía atributos de System.Text.Json que en el cable no tienen efecto (pendings/753).
        services.AddSwaggerGenNewtonsoftSupport();

        // Los ids de esquema necesitan ver todas las operaciones antes de dar el primero (SchemaIdSelector), y eso
        // pide los servicios de la aplicación: por eso van por IConfigureOptions y no en la lambda de AddSwaggerGen.
        services.AddSingleton<SchemaIdSelector>();
        services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSchemaIds>();

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

        // Todo el Swagger, la UI y el JSON, vive bajo /swagger/: así el ingress lo cierra hacia afuera con una sola
        // regla (/svc/{*}/swagger/{**}) y dentro del clúster se sigue leyendo en ms-xxx-rest:5000/swagger/v1/swagger.json,
        // que es por donde lo lee el gateway MCP (pendings/753 y 754). Antes la UI estaba en la raíz del micro.
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("v1/swagger.json", $"{options.AppName} {options.Version}");
            c.RoutePrefix = "swagger";
        });

        return app;
    }
}