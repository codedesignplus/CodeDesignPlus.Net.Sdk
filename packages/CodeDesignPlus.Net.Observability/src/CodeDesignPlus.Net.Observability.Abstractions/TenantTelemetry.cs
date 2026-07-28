using OpenTelemetry;

namespace CodeDesignPlus.Net.Observability.Abstractions;

/// <summary>
/// Carries the tenant of the current operation into the telemetry pipeline.
/// </summary>
/// <remarks>
/// <para>
/// Uses Baggage on purpose. Baggage lives in an <c>AsyncLocal</c>, not in the DI container, so a
/// singleton span processor can read it the moment any span starts. That is what makes the tenant
/// land on child spans too -Mongo, Redis, outgoing gRPC- and not only on the server span, which is
/// all an ASP.NET Core enricher would reach.
/// </para>
/// <para>
/// Baggage is injected into the <c>baggage</c> header of whatever the service calls next. Inside the
/// mesh that is exactly the point: a request that crosses three microservices keeps its tenant all
/// the way. Bear it in mind before adding anything here that should not leave the cluster.
/// </para>
/// </remarks>
public static class TenantTelemetry
{
    /// <summary>
    /// Attribute holding the tenant identifier.
    /// </summary>
    public const string IdKey = "tenant.id";

    /// <summary>
    /// Attribute holding the tenant display name.
    /// </summary>
    public const string NameKey = "tenant.name";

    /// <summary>
    /// Publishes the tenant so every span opened from here on carries it.
    /// </summary>
    /// <param name="id">The tenant identifier. An empty value is ignored.</param>
    /// <param name="name">The tenant display name. Optional.</param>
    public static void Seed(Guid id, string name)
    {
        // Sin tenant no hay nada que etiquetar: pasa en el login, en el refresh del token y en los
        // entrypoints gRPC internos. No es un error, es el caso normal.
        if (id == Guid.Empty)
            return;

        Baggage.SetBaggage(IdKey, id.ToString());

        if (!string.IsNullOrWhiteSpace(name))
            Baggage.SetBaggage(NameKey, name);
    }
}
