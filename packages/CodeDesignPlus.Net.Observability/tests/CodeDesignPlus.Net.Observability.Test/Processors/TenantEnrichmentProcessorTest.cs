using System.Diagnostics;
using CodeDesignPlus.Net.Observability.Abstractions;
using CodeDesignPlus.Net.Observability.Processors;
using OpenTelemetry;

namespace CodeDesignPlus.Net.Observability.Test.Processors;

public class TenantEnrichmentProcessorTest : IDisposable
{
    private const string SourceName = "CodeDesignPlus.Net.Observability.Test.Tenant";

    private readonly ActivitySource source = new(SourceName);
    private readonly ActivityListener listener;

    public TenantEnrichmentProcessorTest()
    {
        // Sin un listener que pida todos los datos, Activity.SetTag no guarda nada y las
        // aserciones pasarian en falso.
        this.listener = new ActivityListener
        {
            ShouldListenTo = candidate => candidate.Name == SourceName,
            Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllDataAndRecorded,
        };

        ActivitySource.AddActivityListener(this.listener);

        Baggage.Current = default;
    }

    public void Dispose()
    {
        this.listener.Dispose();
        this.source.Dispose();
        Baggage.Current = default;

        GC.SuppressFinalize(this);
    }

    [Fact]
    public void OnEnd_WhenTenantWasSeeded_WritesIdAndName()
    {
        var tenantId = Guid.NewGuid();

        TenantTelemetry.Seed(tenantId, "Conjunto Los Alamos");

        using var activity = this.source.StartActivity("operacion");

        new TenantEnrichmentProcessor().OnEnd(activity);

        Assert.Equal(tenantId.ToString(), activity.GetTagItem(TenantTelemetry.IdKey));
        Assert.Equal("Conjunto Los Alamos", activity.GetTagItem(TenantTelemetry.NameKey));
    }

    [Fact]
    public void OnEnd_WhenThereIsNoTenant_LeavesTheSpanUntouched()
    {
        using var activity = this.source.StartActivity("login");

        new TenantEnrichmentProcessor().OnEnd(activity);

        // El caso normal en el login, el refresh del token y los entrypoints gRPC internos.
        Assert.Null(activity.GetTagItem(TenantTelemetry.IdKey));
        Assert.Null(activity.GetTagItem(TenantTelemetry.NameKey));
    }

    [Fact]
    public void Seed_WhenTenantIsEmpty_PublishesNothing()
    {
        TenantTelemetry.Seed(Guid.Empty, "irrelevante");

        Assert.Null(Baggage.GetBaggage(TenantTelemetry.IdKey));
        Assert.Null(Baggage.GetBaggage(TenantTelemetry.NameKey));
    }

    [Fact]
    public void Seed_WhenNameIsMissing_StillPublishesTheId()
    {
        var tenantId = Guid.NewGuid();

        TenantTelemetry.Seed(tenantId, null);

        Assert.Equal(tenantId.ToString(), Baggage.GetBaggage(TenantTelemetry.IdKey));
        Assert.Null(Baggage.GetBaggage(TenantTelemetry.NameKey));
    }

    [Fact]
    public void OnEnd_WhenSpanIsNull_DoesNotThrow()
    {
        var exception = Record.Exception(() => new TenantEnrichmentProcessor().OnEnd(null));

        Assert.Null(exception);
    }
}
