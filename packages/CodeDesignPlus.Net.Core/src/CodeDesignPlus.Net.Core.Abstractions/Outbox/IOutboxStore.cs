namespace CodeDesignPlus.Net.Core.Abstractions.Outbox;

/// <summary>
/// Dónde espera un evento publicado hasta que el despachador lo envía al bus (pendings/245).
/// </summary>
/// <remarks>
/// <c>CodeDesignPlus.Net.Mongo</c> trae una implementación (colección <c>Outbox</c> en la base del micro). Sin almacén
/// registrado, <c>IPubSub</c> publica directo y lo avisa en el log.
/// </remarks>
public interface IOutboxStore
{
    /// <summary>
    /// Crea los índices de la bandeja: el de los pendientes y el que borra lo enviado pasado el plazo de retención.
    /// </summary>
    /// <param name="sentRetention">Cuánto se guarda un mensaje después de enviado.</param>
    /// <param name="cancellationToken">El token de cancelación.</param>
    /// <returns>La tarea.</returns>
    Task EnsureIndexesAsync(Duration sentRetention, CancellationToken cancellationToken);

    /// <summary>
    /// Encola los mensajes. Un mensaje que ya estaba (mismo identificador) no se duplica.
    /// </summary>
    /// <param name="messages">Los mensajes a encolar.</param>
    /// <param name="cancellationToken">El token de cancelación.</param>
    /// <returns>La tarea.</returns>
    Task AddAsync(IReadOnlyList<OutboxMessage> messages, CancellationToken cancellationToken);

    /// <summary>
    /// Reclama el pendiente más antiguo que ya toca enviar y que nadie tiene reclamado, para que otra réplica no lo envíe
    /// a la vez.
    /// </summary>
    /// <param name="owner">Quién lo reclama.</param>
    /// <param name="lease">Cuánto dura el reclamo; pasado ese plazo, otra réplica puede reclamarlo.</param>
    /// <param name="cancellationToken">El token de cancelación.</param>
    /// <returns>El mensaje reclamado, o nulo si no hay ninguno.</returns>
    Task<OutboxMessage?> LeaseNextAsync(string owner, Duration lease, CancellationToken cancellationToken);

    /// <summary>
    /// Anota que el mensaje se envió.
    /// </summary>
    /// <param name="id">El identificador del mensaje.</param>
    /// <param name="cancellationToken">El token de cancelación.</param>
    /// <returns>La tarea.</returns>
    Task MarkSentAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>
    /// Anota un envío fallido: suma el intento y lo programa para más tarde o, si se agotaron, lo deja como fallido.
    /// </summary>
    /// <param name="id">El identificador del mensaje.</param>
    /// <param name="error">Por qué falló.</param>
    /// <param name="nextAttemptAt">Cuándo reintentarlo, o nulo si agotó sus intentos y queda como fallido.</param>
    /// <param name="cancellationToken">El token de cancelación.</param>
    /// <returns>La tarea.</returns>
    Task MarkFailedAsync(Guid id, string error, Instant? nextAttemptAt, CancellationToken cancellationToken);
}
