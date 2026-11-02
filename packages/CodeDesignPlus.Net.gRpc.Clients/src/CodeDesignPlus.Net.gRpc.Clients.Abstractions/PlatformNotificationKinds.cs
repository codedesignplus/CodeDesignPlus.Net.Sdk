namespace CodeDesignPlus.Net.gRpc.Clients.Abstractions;

/// <summary>
/// El catalogo de tipos de aviso de la plataforma: los que nacen del sistema y no del negocio de un producto.
/// </summary>
/// <remarks>
/// Aqui van solo los avisos del sistema: licencias y facturacion de la plataforma. Los de negocio de Kappali
/// (cuentas de cobro, PQRS, porteria, asambleas…) siguen en <c>Kappali.Net.Common.Notifications.NotificationKinds</c>,
/// del SDK de Kappali; este SDK no conoce ninguno. Vive junto a <see cref="INotificationGrpc"/> y
/// <see cref="NotificationIds"/> porque es lo que ya consume cualquier micro que avisa, sin dependencias nuevas.
/// <para>
/// ms-notification no interpreta la clave: la guarda y la entrega como texto, y el frontend decide como se presenta.
/// Por eso el valor es contrato: si cambia, los avisos guardados dejan de casar con su ficha y los nuevos caen al
/// respaldo, sin un solo error. La clave es una cadena con puntos y nunca un numero, por la misma razon por la que
/// un identificador que viaja entre micros no puede ser la posicion de un enum.
/// </para>
/// </remarks>
public static class PlatformNotificationKinds
{
    // ─── Licencias ───────────────────────────────────────────────────────────────

    /// <summary>
    /// A la administración y al consejo: la licencia de la copropiedad vence en pocos días (Kappali pendings/736).
    /// Payload: { licenseId, licenseName, endDate, daysRemaining, graceEndsAt }
    /// </summary>
    public const string LicenseExpiring = "license.expiring";

    /// <summary>
    /// A la administración y al consejo: la licencia venció hoy y empieza el plazo de gracia para renovarla (Kappali pendings/736).
    /// Payload: { licenseId, licenseName, endDate, daysRemaining, graceEndsAt }
    /// </summary>
    public const string LicenseExpired = "license.expired";

    /// <summary>
    /// A la administración y al consejo: terminó el plazo de gracia sin renovar y la copropiedad queda en solo lectura
    /// (Kappali pendings/736, 737). Payload: { licenseId, licenseName, endDate, daysRemaining, graceEndsAt }
    /// </summary>
    public const string LicenseBlocked = "license.blocked";
}
