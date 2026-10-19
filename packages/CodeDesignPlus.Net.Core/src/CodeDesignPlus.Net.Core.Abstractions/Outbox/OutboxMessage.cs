namespace CodeDesignPlus.Net.Core.Abstractions.Outbox;

/// <summary>
/// Un evento ya serializado que espera en la bandeja de salida a que el despachador lo envíe (pendings/245).
/// </summary>
/// <remarks>
/// Se guarda el cuerpo tal como viajará, no el evento: volver a deserializarlo con su clase perdería lo que la clase no
/// sabe leer y cambiaría el mensaje entre un intento y otro.
/// </remarks>
/// <param name="Id">El identificador del evento (<see cref="IDomainEvent.EventId"/>), que es también el del mensaje.</param>
/// <param name="Topic">La clave del evento, que es el destino del mensaje.</param>
/// <param name="EventName">El nombre de la clase del evento.</param>
/// <param name="AggregateId">El agregado que originó el evento.</param>
/// <param name="Body">El evento serializado en JSON.</param>
/// <param name="Headers">Las cabeceras de trazas con las que se encoló, para que el envío cuelgue de la misma traza.</param>
/// <param name="CreatedAt">Cuándo se encoló.</param>
/// <param name="Attempts">Cuántos envíos fallidos lleva.</param>
public sealed record OutboxMessage(Guid Id, string Topic, string EventName, Guid AggregateId, string Body, Dictionary<string, string> Headers, Instant CreatedAt, int Attempts);
