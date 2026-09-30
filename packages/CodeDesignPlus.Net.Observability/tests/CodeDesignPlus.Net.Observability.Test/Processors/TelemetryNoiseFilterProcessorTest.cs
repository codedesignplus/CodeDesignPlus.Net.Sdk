using System.Diagnostics;
using CodeDesignPlus.Net.Observability.Processors;
using OpenTelemetry;
using OpenTelemetry.Trace;

namespace CodeDesignPlus.Net.Observability.Test.Processors;

public class TelemetryNoiseFilterProcessorTest : IDisposable
{
    private const string AppSourceName = "CodeDesignPlus.Net.Observability.Test.Noise";

    private readonly ActivitySource appSource = new(AppSourceName);
    private readonly ActivitySource redisSource = new(TelemetryNoiseFilterProcessor.RedisSourceName);
    private readonly ActivityListener listener;

    public TelemetryNoiseFilterProcessorTest()
    {
        // Sin un listener que pida todos los datos, las actividades no se crean o nacen sin la marca
        // Recorded, y las aserciones pasarian en falso.
        this.listener = new ActivityListener
        {
            ShouldListenTo = candidate => candidate.Name is AppSourceName or TelemetryNoiseFilterProcessor.RedisSourceName,
            Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllDataAndRecorded,
        };

        ActivitySource.AddActivityListener(this.listener);
    }

    public void Dispose()
    {
        this.listener.Dispose();
        this.appSource.Dispose();
        this.redisSource.Dispose();

        GC.SuppressFinalize(this);
    }

    [Fact]
    public void OnEnd_WhenRedisCommandHasNoParent_DropsIt()
    {
        using var command = this.redisSource.StartActivity("ZRANGEBYSCORE", ActivityKind.Client, parentContext: default)!;

        new TelemetryNoiseFilterProcessor().OnEnd(command);

        Assert.False(command.Recorded);
    }

    [Fact]
    public void OnEnd_WhenRedisCommandHangsFromARequest_KeepsIt()
    {
        using var request = this.appSource.StartActivity("GET /api/Units", ActivityKind.Server)!;
        using var command = this.redisSource.StartActivity("GET", ActivityKind.Client)!;

        new TelemetryNoiseFilterProcessor().OnEnd(command);

        // Es la latencia de Redis en el camino de un usuario: la que la regla 13 obliga a conservar.
        Assert.True(command.Recorded);
    }

    [Fact]
    public void OnEnd_WhenGrpcCallGoesToTheCollector_DropsIt()
    {
        using var export = this.appSource.StartActivity("opentelemetry.proto.collector.logs.v1.LogsService/Export", ActivityKind.Client)!;

        new TelemetryNoiseFilterProcessor().OnEnd(export);

        Assert.False(export.Recorded);
    }

    [Fact]
    public void OnEnd_WhenHttpCallGoesToTheCollector_DropsIt()
    {
        using var post = this.appSource.StartActivity("POST", ActivityKind.Client)!;
        post.SetTag("url.full", "http://signoz-k8s-infra-otel-agent.signoz.svc.cluster.local:4317/opentelemetry.proto.collector.logs.v1.LogsService/Export");

        new TelemetryNoiseFilterProcessor().OnEnd(post);

        Assert.False(post.Recorded);
    }

    [Fact]
    public void OnEnd_WhenSpanIsOrdinaryWork_KeepsIt()
    {
        using var call = this.appSource.StartActivity("ms-tenants.Tenant/GetTenant", ActivityKind.Client, parentContext: default)!;
        call.SetTag("url.full", "http://ms-tenants-grpc:5001/ms-tenants.Tenant/GetTenant");

        new TelemetryNoiseFilterProcessor().OnEnd(call);

        Assert.True(call.Recorded);
    }

    [Fact]
    public void OnEnd_WhenActivityIsNull_DoesNotThrow()
    {
        var exception = Record.Exception(() => new TelemetryNoiseFilterProcessor().OnEnd(null));

        Assert.Null(exception);
    }

    [Fact]
    public void Pipeline_WhenRegisteredBeforeTheExporter_OnlyExportsWhatMatters()
    {
        var exported = new List<string>();

        using (var provider = Sdk.CreateTracerProviderBuilder()
            .AddSource(AppSourceName, TelemetryNoiseFilterProcessor.RedisSourceName)
            .AddProcessor(new TelemetryNoiseFilterProcessor())
            .AddProcessor(new SimpleActivityExportProcessor(new CollectingExporter(exported)))
            .Build())
        {
            this.redisSource.StartActivity("EXEC", ActivityKind.Client, parentContext: default)?.Dispose();

            using (this.appSource.StartActivity("GET /api/Units", ActivityKind.Server))
                this.redisSource.StartActivity("GET", ActivityKind.Client)?.Dispose();

            this.appSource.StartActivity("opentelemetry.proto.collector.logs.v1.LogsService/Export", ActivityKind.Client, parentContext: default)?.Dispose();
        }

        // El exportador solo recibe la peticion y su comando Redis; el sondeo huerfano y el envio de
        // logs al colector nunca salen del proceso.
        Assert.Equal(["GET", "GET /api/Units"], exported);
    }

    /// <summary>
    /// Exportador que solo anota el nombre de cada span recibido.
    /// </summary>
    private sealed class CollectingExporter(List<string> exported) : BaseExporter<Activity>
    {
        public override ExportResult Export(in Batch<Activity> batch)
        {
            foreach (var activity in batch)
                exported.Add(activity.DisplayName);

            return ExportResult.Success;
        }
    }
}
