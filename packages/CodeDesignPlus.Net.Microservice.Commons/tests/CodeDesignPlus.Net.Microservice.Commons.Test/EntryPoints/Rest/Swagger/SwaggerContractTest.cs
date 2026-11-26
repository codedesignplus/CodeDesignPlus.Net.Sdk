using CodeDesignPlus.Net.Microservice.Commons.EntryPoints.Rest.Swagger;
using CodeDesignPlus.Net.Microservice.Commons.FluentValidation;
using CodeDesignPlus.Net.ValueObjects.Financial;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using NodaTime;
using NodaTime.Serialization.JsonNet;
using Swashbuckle.AspNetCore.Swagger;

namespace CodeDesignPlus.Net.Microservice.Commons.Test.EntryPoints.Rest.Swagger;

/// <summary>
/// El Swagger que arma <c>AddCoreSwagger</c> describe el contrato que serializa Newtonsoft (pendings/753).
/// </summary>
/// <remarks>
/// La prueba de control (<see cref="Verify_WithoutTheSdkSetup_DetectsEveryDefect"/>) es la que da valor al resto:
/// con un Swashbuckle sin la configuración del SDK, el verificador tiene que encontrar cada defecto que se vio en
/// los Swagger de producción. Si un día deja de encontrarlos, el verde de las demás no prueba nada.
/// </remarks>
public class SwaggerContractTest
{
    private static readonly Type[] Inputs = [typeof(CreateSampleDto)];

    private static readonly Type[] Outputs = [typeof(SampleDto)];

    [Fact]
    public void Verify_WithTheSdkSetup_FindsNoMismatch()
    {
        var services = Build(withSdk: true);

        var mismatches = OpenApiContractVerifier.Verify(services, Inputs, Outputs);

        Assert.True(mismatches.Count == 0, string.Join(Environment.NewLine, mismatches));
    }

    [Fact]
    public void Verify_WithoutTheSdkSetup_DetectsEveryDefect()
    {
        var services = Build(withSdk: false);

        var mismatches = OpenApiContractVerifier.Verify(services, Inputs, Outputs);

        // El value object del dominio: Newtonsoft lo llena por el constructor y el esquema lo marca de solo lectura.
        Assert.Contains(mismatches, x => x.Type == typeof(SampleSpot) && x.Property == "number" && x.Kind == ContractMismatchKind.ReadOnlyButWritable);

        // NodaTime descrito como objeto.
        Assert.Contains(mismatches, x => x.Type == typeof(LocalDate) && x.Kind == ContractMismatchKind.ConverterNotDescribedAsValue);

        // [Newtonsoft.Json.JsonIgnore] que el esquema no ve.
        Assert.Contains(mismatches, x => x.Type == typeof(CreateSampleDto) && x.Property == "platformIssuer" && x.Kind == ContractMismatchKind.NotSerialized);
        Assert.Contains(mismatches, x => x.Type == typeof(SampleSpot) && x.Property == "isAssigned" && x.Kind == ContractMismatchKind.NotSerialized);

        // [System.Text.Json...JsonIgnore] que oculta lo que Newtonsoft sí escribe.
        Assert.Contains(mismatches, x => x.Type == typeof(SampleDto) && x.Property == "key" && x.Kind == ContractMismatchKind.MissingInSchema);
    }

    [Fact]
    public void Verify_ScanningTheControllers_ReachesTheBodyTypes()
    {
        var services = Build(withSdk: false);

        var mismatches = OpenApiContractVerifier.Verify(services);

        Assert.Contains(mismatches, x => x.Type == typeof(SampleSpot) && x.Kind == ContractMismatchKind.ReadOnlyButWritable);
    }

    [Fact]
    public void GetSwagger_NodaTimeTypes_AreStrings()
    {
        var document = Document();
        var properties = document.Components.Schemas[nameof(CreateSampleDto)].Properties;

        AssertString(properties["startDate"], "date", "2026-11-25");
        AssertString(properties["endDate"], "date", "2026-11-25");
        AssertString(properties["at"], "date-time", "2026-11-25T15:00:00Z");
        AssertString(properties["opensAt"], null, "10:00:00");
        AssertString(properties["length"], null, "1:30:00");

        Assert.True(properties["endDate"].Nullable);
        Assert.False(properties["startDate"].Nullable);
    }

    [Fact]
    public void GetSwagger_ValueObjectFilledByTheConstructor_IsWritable()
    {
        var spot = Document().Components.Schemas[nameof(SampleSpot)].Properties;

        Assert.False(spot["id"].ReadOnly);
        Assert.False(spot["number"].ReadOnly);

        // Calculada y sin parámetro en el constructor: sigue siendo de solo lectura.
        Assert.True(spot["label"].ReadOnly);

        Assert.False(spot.ContainsKey("isAssigned"));
    }

    [Fact]
    public void GetSwagger_JsonIgnoreOfEachEngine_FollowsNewtonsoft()
    {
        var schemas = Document().Components.Schemas;

        Assert.False(schemas[nameof(CreateSampleDto)].Properties.ContainsKey("platformIssuer"));
        Assert.True(schemas[nameof(SampleDto)].Properties.ContainsKey("key"));
    }

    [Fact]
    public void GetSwagger_Enum_KeepsIntegersAndNamesThem()
    {
        var status = Document().Components.Schemas[nameof(SampleStatus)];

        Assert.Equal("integer", status.Type);

        var names = Assert.IsType<OpenApiArray>(status.Extensions[EnumNamesSchemaFilter.VarNamesExtension]);

        Assert.Equal(["Draft", "Active", "Cancelled"], names.Cast<OpenApiString>().Select(x => x.Value));
        Assert.Contains("5 = Cancelled", status.Description, StringComparison.Ordinal);
    }

    [Fact]
    public void GetSwagger_Money_SaysTheUnitOfEachAmount()
    {
        var schemas = Document().Components.Schemas;

        Assert.Contains("major unit", schemas[nameof(MoneyInput)].Properties["amount"].Description, StringComparison.Ordinal);
        Assert.Contains("minor unit", schemas[nameof(Money)].Properties["amount"].Description, StringComparison.Ordinal);
    }

    [Fact]
    public void GetSwagger_TenantHeader_OnlyOnOperationsThatNeedAUser()
    {
        var paths = Document().Paths;

        var create = paths["/api/Sample"].Operations[OperationType.Post];
        var header = Assert.Single(create.Parameters, x => x.Name == TenantHeaderOperationFilter.HeaderName);

        Assert.Equal(ParameterLocation.Header, header.In);
        Assert.Equal("uuid", header.Schema.Format);

        var anonymous = paths["/api/Sample/public"].Operations[OperationType.Get];

        Assert.DoesNotContain(anonymous.Parameters ?? [], x => x.Name == TenantHeaderOperationFilter.HeaderName);
    }

    [Fact]
    public void GetSwagger_CommandValidator_ReachesTheDtoSchema()
    {
        var schema = Document().Components.Schemas[nameof(CreateSampleDto)];
        var properties = schema.Properties;

        Assert.Contains("id", schema.Required);
        Assert.Contains("code", schema.Required);

        // Regla con When: depende de otro valor, no es obligatorio.
        Assert.DoesNotContain("notes", schema.Required);

        Assert.Equal(1, properties["code"].MinLength);
        Assert.Equal(32, properties["code"].MaxLength);
        Assert.Equal("^[A-Z0-9-]+$", properties["code"].Pattern);
        Assert.Equal("email", properties["email"].Format);
        Assert.Equal(1, properties["seats"].Minimum);
        Assert.Equal(500, properties["seats"].Maximum);
        Assert.Equal(0, properties["rate"].Minimum);
        Assert.True(properties["rate"].ExclusiveMinimum);

        // La regla del comando sobre algo que el DTO no tiene no inventa una propiedad.
        Assert.False(properties.ContainsKey("internal"));
    }

    [Fact]
    public void GetSwagger_BodyThatIsTheCommand_UsesItsOwnValidator()
    {
        var schema = Document().Components.Schemas[nameof(RenameSampleCommand)];

        Assert.Contains("name", schema.Required);
        Assert.Equal(3, schema.Properties["name"].MinLength);
        Assert.Equal(64, schema.Properties["name"].MaxLength);
        Assert.DoesNotContain("id", schema.Required);
    }

    [Fact]
    public void GetSwagger_TwoTypesWithTheSameName_GetDistinctIds()
    {
        var schemas = Build(withSdk: true, typeof(DuplicateNamesController)).GetRequiredService<ISwaggerProvider>().GetSwagger("v1").Components.Schemas;

        var licenses = schemas.Where(x => x.Key.EndsWith("LicenseDto", StringComparison.Ordinal)).ToList();

        Assert.Equal(2, licenses.Count);
        Assert.Contains(licenses, x => x.Key == "LicenseDto");
        Assert.Contains(licenses, x => x.Value.Properties.ContainsKey("name"));
        Assert.Contains(licenses, x => x.Value.Properties.ContainsKey("seats"));
        Assert.Contains(licenses, x => x.Key is "LicenseLicenseDto" or "OrderLicenseDto");

        // Los ids de siempre no cambian.
        Assert.True(schemas.ContainsKey(nameof(SampleDto)));
    }

    [Fact]
    public void GetSwagger_TwoTypesWithTheSameName_FailWithoutTheSelector()
    {
        var provider = Build(withSdk: false, typeof(DuplicateNamesController)).GetRequiredService<ISwaggerProvider>();

        Assert.ThrowsAny<Exception>(() => provider.GetSwagger("v1"));
    }

    [Theory]
    [InlineData("/swagger/index.html", 200)]
    [InlineData("/swagger/v1/swagger.json", 200)]
    [InlineData("/index.html", 404)]
    public async Task UseCoreSwagger_ServesEverythingUnderSwagger(string path, int expected)
    {
        var builder = WebApplication.CreateBuilder();

        builder.WebHost.UseTestServer();
        builder.Configuration.AddInMemoryCollection(Settings);
        builder.Services.AddControllers().AddNewtonsoftJson();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddCoreSwagger<SampleController>(builder.Configuration);
        builder.Services.Configure<CodeDesignPlus.Net.Core.Abstractions.Options.CoreOptions>(builder.Configuration.GetSection("Core"));

        await using var app = builder.Build();

        app.UseCoreSwagger();

        await app.StartAsync();

        var response = await app.GetTestClient().GetAsync(path);

        Assert.Equal(expected, (int)response.StatusCode);
    }

    private static readonly Dictionary<string, string> Settings = new()
    {
        ["Core:Id"] = Guid.NewGuid().ToString(),
        ["Core:Business"] = "CodeDesignPlus",
        ["Core:AppName"] = "ms-sample",
        ["Core:TypeEntryPoint"] = "rest",
        ["Core:Version"] = "v1",
        ["Core:Description"] = "Sample contract",
        ["Core:Contact:Name"] = "CodeDesignPlus",
        ["Core:Contact:Email"] = "contact@codedesignplus.com"
    };

    private static void AssertString(OpenApiSchema schema, string format, string example)
    {
        Assert.Equal("string", schema.Type);
        Assert.Equal(format, schema.Format);
        Assert.Equal(example, Assert.IsType<OpenApiString>(schema.Example).Value);
        Assert.Empty(schema.Properties);
    }

    private static OpenApiDocument Document() => Build(withSdk: true).GetRequiredService<ISwaggerProvider>().GetSwagger("v1");

    /// <summary>
    /// Un REST como los de los micros: controllers con Newtonsoft y NodaTime, y el Swagger del SDK o el de serie.
    /// </summary>
    private static IServiceProvider Build(bool withSdk, params Type[] extraControllers)
    {
        var builder = WebApplication.CreateBuilder();

        builder.Configuration.AddInMemoryCollection(new Dictionary<string, string>
        {
            ["Core:Id"] = Guid.NewGuid().ToString(),
            ["Core:Business"] = "CodeDesignPlus",
            ["Core:AppName"] = "ms-sample",
            ["Core:TypeEntryPoint"] = "rest",
            ["Core:Version"] = "v1",
            ["Core:Description"] = "Sample contract",
            ["Core:Contact:Name"] = "CodeDesignPlus",
            ["Core:Contact:Email"] = "contact@codedesignplus.com"
        });

        builder.Services
            .AddControllers()
            .AddApplicationPart(typeof(SampleController).Assembly)
            .ConfigureApplicationPartManager(manager =>
            {
                // Solo el controller de la muestra: el proyecto tiene otros que no son de este contrato.
                foreach (var provider in manager.FeatureProviders.OfType<ControllerFeatureProvider>().ToList())
                    manager.FeatureProviders.Remove(provider);

                manager.FeatureProviders.Add(new SampleControllerFeatureProvider([typeof(SampleController), .. extraControllers]));
            })
            .AddNewtonsoftJson(options => options.SerializerSettings.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb));

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddFluentValidation<SampleController>();

        if (withSdk)
            builder.Services.AddCoreSwagger<SampleController>(builder.Configuration);
        else
            builder.Services.AddSwaggerGen(x => x.SwaggerDoc("v1", new OpenApiInfo { Title = "ms-sample", Version = "v1" }));

        var app = builder.Build();

        app.MapControllers();

        return app.Services;
    }

    private sealed class SampleControllerFeatureProvider(Type[] controllers) : ControllerFeatureProvider
    {
        protected override bool IsController(TypeInfo typeInfo) => controllers.Contains(typeInfo.AsType());
    }
}
