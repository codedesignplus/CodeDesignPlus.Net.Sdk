using CodeDesignPlus.Net.gRpc.Clients.Services.Notification;
using Google.Protobuf.WellKnownTypes;

namespace CodeDesignPlus.Net.gRpc.Clients.Abstractions;

/// <summary>
/// La puerta publica de los avisos durables: arma la audiencia, el identificador de idempotencia y la
/// marca de tiempo, y serializa el payload en camelCase.
/// </summary>
/// <remarks>
/// Hay un metodo por tipo de audiencia en vez de uno con un parametro <c>AudienceKind</c>: asi no existe
/// la llamada que compila con <c>AUDIENCE_KIND_UNSPECIFIED</c> y manda un aviso privado a toda la
/// copropiedad.
/// </remarks>
public static class InboxExtensions
{
    /// <summary>
    /// Manda un aviso durable a usuarios concretos.
    /// </summary>
    /// <param name="grpc">El cliente de la bandeja.</param>
    /// <param name="userIds">Los usuarios destinatarios.</param>
    /// <param name="kind">La clave estable del tipo de aviso (use <see cref="NotificationKinds"/>).</param>
    /// <param name="title">Respaldo para cuando el frontend no conoce el <paramref name="kind"/>.</param>
    /// <param name="body">El texto del aviso.</param>
    /// <param name="module">El modulo dueño del dato al que lleva el clic, o <c>null</c> si no lleva a ninguna parte.</param>
    /// <param name="aggregateId">El identificador del agregado dentro de ese modulo.</param>
    /// <param name="payload">El objeto que se serializa como payload JSON.</param>
    /// <param name="tenant">La copropiedad a la que pertenece el aviso.</param>
    /// <param name="sentBy">Quien o que origina el aviso.</param>
    /// <param name="occurredAt">Cuando ocurrio el hecho. Por omision, ahora.</param>
    /// <param name="cancellationToken">Token para cancelar la operacion.</param>
    /// <returns>Una tarea que representa la operacion asincronica.</returns>
    public static Task NotifyUsersAsync(
        this IInboxGrpc grpc,
        IEnumerable<Guid> userIds,
        string kind,
        string title,
        string body,
        string? module,
        string? aggregateId,
        object payload,
        Guid tenant,
        Guid sentBy,
        DateTimeOffset? occurredAt = null,
        CancellationToken cancellationToken = default)
    {
        var audience = new Audience { Kind = AudienceKind.User };
        audience.Values.AddRange(userIds.Select(x => x.ToString()));

        return Send(grpc, audience, kind, title, body, module, aggregateId, payload, tenant, sentBy, occurredAt, cancellationToken);
    }

    /// <summary>
    /// Manda un aviso durable a quien tenga alguno de esos roles.
    /// </summary>
    /// <remarks>
    /// <b>Lo que viaja son los identificadores de grupo del proveedor de identidad, no los nombres de los
    /// roles.</b> El lector trae esos identificadores en el claim <c>groups</c> de su token, y la bandeja
    /// compara cadenas sin traducir nada por el camino: un aviso dirigido a la cadena "Administrador" se
    /// guarda bien y no le llega a nadie.
    /// <para>
    /// El rol si viaja sin resolver a las personas que lo tienen —quien emite conoce la unidad, no al
    /// residente, y buscarlas aqui seria una llamada cruzada en el path de escritura—. Lo que se resuelve
    /// al leer es la pertenencia al grupo, y eso sale gratis porque viene en el token.
    /// </para>
    /// <para>
    /// Los identificadores <b>cambian por entorno</b>, porque cada entorno es un directorio distinto. Quien
    /// llama los toma de su configuracion, nunca de una constante en el codigo.
    /// </para>
    /// </remarks>
    /// <param name="grpc">El cliente de la bandeja.</param>
    /// <param name="roles">Los identificadores de los grupos destinatarios, no los nombres de los roles.</param>
    /// <param name="kind">La clave estable del tipo de aviso (use <see cref="NotificationKinds"/>).</param>
    /// <param name="title">Respaldo para cuando el frontend no conoce el <paramref name="kind"/>.</param>
    /// <param name="body">El texto del aviso.</param>
    /// <param name="module">El modulo dueño del dato al que lleva el clic, o <c>null</c>.</param>
    /// <param name="aggregateId">El identificador del agregado dentro de ese modulo.</param>
    /// <param name="payload">El objeto que se serializa como payload JSON.</param>
    /// <param name="tenant">La copropiedad a la que pertenece el aviso.</param>
    /// <param name="sentBy">Quien o que origina el aviso.</param>
    /// <param name="occurredAt">Cuando ocurrio el hecho. Por omision, ahora.</param>
    /// <param name="cancellationToken">Token para cancelar la operacion.</param>
    /// <returns>Una tarea que representa la operacion asincronica.</returns>
    public static Task NotifyRolesAsync(
        this IInboxGrpc grpc,
        IEnumerable<string> roles,
        string kind,
        string title,
        string body,
        string? module,
        string? aggregateId,
        object payload,
        Guid tenant,
        Guid sentBy,
        DateTimeOffset? occurredAt = null,
        CancellationToken cancellationToken = default)
    {
        var audience = new Audience { Kind = AudienceKind.Role };
        audience.Values.AddRange(roles);

        return Send(grpc, audience, kind, title, body, module, aggregateId, payload, tenant, sentBy, occurredAt, cancellationToken);
    }

    /// <summary>
    /// Manda un aviso durable a toda la copropiedad.
    /// </summary>
    /// <param name="grpc">El cliente de la bandeja.</param>
    /// <param name="kind">La clave estable del tipo de aviso (use <see cref="NotificationKinds"/>).</param>
    /// <param name="title">Respaldo para cuando el frontend no conoce el <paramref name="kind"/>.</param>
    /// <param name="body">El texto del aviso.</param>
    /// <param name="module">El modulo dueño del dato al que lleva el clic, o <c>null</c>.</param>
    /// <param name="aggregateId">El identificador del agregado dentro de ese modulo.</param>
    /// <param name="payload">El objeto que se serializa como payload JSON.</param>
    /// <param name="tenant">La copropiedad destinataria.</param>
    /// <param name="sentBy">Quien o que origina el aviso.</param>
    /// <param name="occurredAt">Cuando ocurrio el hecho. Por omision, ahora.</param>
    /// <param name="cancellationToken">Token para cancelar la operacion.</param>
    /// <returns>Una tarea que representa la operacion asincronica.</returns>
    public static Task NotifyTenantAsync(
        this IInboxGrpc grpc,
        string kind,
        string title,
        string body,
        string? module,
        string? aggregateId,
        object payload,
        Guid tenant,
        Guid sentBy,
        DateTimeOffset? occurredAt = null,
        CancellationToken cancellationToken = default)
        => Send(grpc, new Audience { Kind = AudienceKind.Tenant }, kind, title, body, module, aggregateId, payload, tenant, sentBy, occurredAt, cancellationToken);

    private static Task Send(
        IInboxGrpc grpc, Audience audience, string kind, string title, string body,
        string? module, string? aggregateId, object payload, Guid tenant, Guid sentBy,
        DateTimeOffset? occurredAt, CancellationToken cancellationToken)
    {
        var request = new NotificationRequest
        {
            // Lo genera el emisor: es la clave de idempotencia frente a las reentregas del bus. Si lo
            // generara el servidor, cada reentrega crearia un aviso nuevo y la campana repetiria el hecho.
            Id = Guid.NewGuid().ToString(),
            Tenant = tenant.ToString(),
            Audience = audience,
            Kind = kind,
            Title = title,
            Body = body,
            JsonPayload = NotificationSerialization.Serialize(payload),
            SentBy = sentBy.ToString(),
            OccurredAt = Timestamp.FromDateTimeOffset(occurredAt ?? DateTimeOffset.UtcNow)
        };

        // Sin modulo no hay a donde llevar, y un Resource vacio produciria un enlace roto en la campana.
        if (!string.IsNullOrWhiteSpace(module))
            request.Resource = new Resource { Module = module, AggregateId = aggregateId ?? string.Empty };

        return grpc.NotifyAsync(request, cancellationToken);
    }
}
