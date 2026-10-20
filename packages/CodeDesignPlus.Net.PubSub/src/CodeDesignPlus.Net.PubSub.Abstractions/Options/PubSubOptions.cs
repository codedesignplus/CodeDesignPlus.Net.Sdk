using System.ComponentModel.DataAnnotations;

namespace CodeDesignPlus.Net.PubSub.Abstractions.Options;

/// <summary>
/// Options for configuring the PubSub service.
/// </summary>
public class PubSubOptions
{
    /// <summary>
    /// The name of the section used in the appsettings.
    /// </summary>
    public static readonly string Section = "PubSub";
    /// <summary>
    /// Gets or sets a value indicating whether diagnostics are enabled.
    /// </summary>
    public bool EnableDiagnostic { get; set; } = false;
    /// <summary>
    /// Gets or sets a value indicating whether to register automatic handlers.
    /// </summary>
    public bool RegisterAutomaticHandlers { get; set; } = true;
    /// <summary>
    /// Cuántos agregados lee y publica el dueño por página al contestar una petición de foto.
    /// </summary>
    [Range(1, 1000)]
    public uint SnapshotPageSize { get; set; } = 100;
    /// <summary>
    /// Cada cuánto vuelve a mirar la copia las fotos que no quedaron sincronizadas, y cuánto espera desde la última
    /// petición antes de repetirla. <c>00:00:00</c> desactiva la repetición: solo se vuelve a pedir al arrancar.
    /// </summary>
    /// <remarks>
    /// Una foto queda sin sincronizar si la copia no aplicó todo lo que anunció el dueño (una foto acabó en la cola de
    /// descartados) o si el dueño no contestó. Al repetir, la copia lo avisa en el log (pendings/485).
    /// </remarks>
    [Range(typeof(TimeSpan), "00:00:00", "7.00:00:00")]
    public TimeSpan SnapshotRetryInterval { get; set; } = TimeSpan.FromHours(1);
    /// <summary>
    /// Plazo en segundos para encolar un evento en la bandeja de salida, o para enviarlo si no la hay.
    /// </summary>
    /// <remarks>
    /// Corre por su cuenta y no con el token de quien publica: si el cliente de una petición se va después de guardar, el
    /// evento sale igual (pendings/245).
    /// </remarks>
    [Range(1, 300)]
    public uint PublishTimeoutSeconds { get; set; } = 30;
    /// <summary>
    /// La bandeja de salida: dónde esperan los eventos publicados hasta que el despachador los envía.
    /// </summary>
    public OutboxOptions Outbox { get; set; } = new();
}
