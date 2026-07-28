using System.Diagnostics;
using CodeDesignPlus.Net.Observability.Abstractions;
using OpenTelemetry;

namespace CodeDesignPlus.Net.Observability.Processors;

/// <summary>
/// Writes the tenant of the current operation onto every span, reading it from the Baggage that
/// <see cref="TenantTelemetry.Seed"/> published.
/// </summary>
/// <remarks>
/// It enriches on <see cref="OnEnd"/> and not on <c>OnStart</c>, which looks backwards but is the
/// only thing that works: the ASP.NET Core instrumentation opens the server span when the request
/// arrives, well before the middleware that resolves the tenant has run. On <c>OnStart</c> the
/// baggage is still empty and the most important span of the trace -the one carrying the route and
/// the status code- would end up without a tenant. By <c>OnEnd</c> every span has it, the server one
/// included.
/// </remarks>
public class TenantEnrichmentProcessor : BaseProcessor<Activity>
{
    /// <inheritdoc/>
    public override void OnEnd(Activity data)
    {
        if (data is null)
            return;

        var id = Baggage.GetBaggage(TenantTelemetry.IdKey);

        if (string.IsNullOrEmpty(id))
            return;

        data.SetTag(TenantTelemetry.IdKey, id);

        var name = Baggage.GetBaggage(TenantTelemetry.NameKey);

        if (!string.IsNullOrEmpty(name))
            data.SetTag(TenantTelemetry.NameKey, name);
    }
}
