using System.ComponentModel.DataAnnotations;

namespace CodeDesignPlus.Net.PubSub.Abstractions.Options;

/// <summary>
/// La bandeja de salida de los eventos (pendings/245).
/// </summary>
/// <remarks>
/// Con la bandeja, publicar es escribir el evento en la base del micro, sin depender del cliente ni del bus; un despachador
/// en segundo plano lo envía, lo reintenta si falla y deja a la vista lo que agota sus intentos. Necesita un
/// <c>IOutboxStore</c> (lo trae <c>CodeDesignPlus.Net.Mongo</c>) y un transporte que sepa enviar un mensaje ya
/// serializado; sin alguno de los dos, se publica directo.
/// </remarks>
public class OutboxOptions
{
    /// <summary>
    /// Si los eventos pasan por la bandeja. Activa por defecto.
    /// </summary>
    public bool Enable { get; set; } = true;
    /// <summary>
    /// Cada cuántos segundos mira el despachador si hay pendientes cuando nadie le avisa.
    /// </summary>
    /// <remarks>
    /// Al encolar en este proceso se le avisa y despacha enseguida; el sondeo recoge lo que encolaron otras réplicas y lo
    /// que toca reintentar.
    /// </remarks>
    [Range(1, 300)]
    public uint PollingIntervalSeconds { get; set; } = 5;
    /// <summary>
    /// Cuántos segundos reclama una réplica un mensaje mientras lo envía; pasado ese plazo, otra puede enviarlo.
    /// </summary>
    [Range(10, 3600)]
    public uint LeaseSeconds { get; set; } = 120;
    /// <summary>
    /// Cuántos envíos fallidos se admiten antes de dejar el mensaje como fallido.
    /// </summary>
    [Range(1, 100)]
    public uint MaxAttempts { get; set; } = 20;
    /// <summary>
    /// La espera en segundos antes del primer reintento; se duplica en cada fallo hasta <see cref="MaxRetryDelaySeconds"/>.
    /// </summary>
    [Range(1, 3600)]
    public uint InitialRetryDelaySeconds { get; set; } = 5;
    /// <summary>
    /// La espera máxima en segundos entre dos reintentos.
    /// </summary>
    [Range(1, 86400)]
    public uint MaxRetryDelaySeconds { get; set; } = 600;
    /// <summary>
    /// Cuántos días se guarda un mensaje después de enviado. Los fallidos no se borran nunca.
    /// </summary>
    [Range(1, 365)]
    public uint SentRetentionDays { get; set; } = 7;
}
