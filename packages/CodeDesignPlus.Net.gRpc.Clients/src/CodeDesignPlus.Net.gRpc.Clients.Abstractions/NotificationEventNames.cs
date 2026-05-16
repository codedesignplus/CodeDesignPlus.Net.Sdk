namespace CodeDesignPlus.Net.gRpc.Clients.Abstractions;

/// <summary>
/// Nombres estándar de eventos de notificación en tiempo real para la plataforma.
/// Estos valores se usan como <c>eventName</c> en las notificaciones SignalR,
/// permitiendo al frontend suscribirse a eventos específicos.
/// </summary>
public static class NotificationEventNames
{
    // ─── Facturación ────────────────────────────────────────────────────────────

    /// <summary>
    /// Se emite cuando una factura es emitida oficialmente al residente.
    /// Payload: { documentId, amount, dueDate, concept }
    /// </summary>
    public const string InvoiceIssued = "invoice.issued";

    /// <summary>
    /// Se emite cuando una factura es pagada completamente.
    /// Payload: { documentId, amount, paidDate }
    /// </summary>
    public const string InvoicePaid = "invoice.paid";

    /// <summary>
    /// Se emite cuando una factura vence sin haber sido pagada.
    /// Payload: { documentId, amount, daysPastDue }
    /// </summary>
    public const string InvoiceOverdue = "invoice.overdue";

    /// <summary>
    /// Se emite cuando se aplica un pago parcial a una factura.
    /// Payload: { documentId, amountPaid, amountDue }
    /// </summary>
    public const string InvoicePartiallyPaid = "invoice.partially_paid";

    // ─── Pagos ───────────────────────────────────────────────────────────────────

    /// <summary>
    /// Se emite cuando un pago es procesado exitosamente por el gateway.
    /// Payload: { paymentId, amount, currency }
    /// </summary>
    public const string PaymentSucceeded = "payment.succeeded";

    /// <summary>
    /// Se emite cuando un pago falla en el gateway.
    /// Payload: { paymentId, reason }
    /// </summary>
    public const string PaymentFailed = "payment.failed";

    // ─── Reservas de Áreas Comunes ───────────────────────────────────────────────

    /// <summary>
    /// Se emite cuando una reserva de área común es confirmada tras el pago exitoso.
    /// Payload: { bookingId, commonAreaName, startTime, endTime }
    /// </summary>
    public const string BookingConfirmed = "booking.confirmed";

    /// <summary>
    /// Se emite cuando una reserva es cancelada.
    /// Payload: { bookingId, commonAreaName, reason }
    /// </summary>
    public const string BookingCancelled = "booking.cancelled";

    /// <summary>
    /// Se emite como recordatorio 1 hora antes de una reserva confirmada.
    /// Payload: { bookingId, commonAreaName, startTime, hoursUntil }
    /// </summary>
    public const string BookingReminder = "booking.reminder";

    /// <summary>
    /// Se emite cuando una reserva es completada con daños reportados.
    /// Payload: { bookingId, damageCost, damageNotes }
    /// </summary>
    public const string BookingCompletedWithDamages = "booking.completed_with_damages";

    // ─── Contratos de Arrendamiento ──────────────────────────────────────────────

    /// <summary>
    /// Se emite cuando un contrato de arrendamiento está listo para firma digital.
    /// Payload: { leaseId, unitId }
    /// </summary>
    public const string LeaseReadyForSignature = "lease.ready_for_signature";

    /// <summary>
    /// Se emite cuando un contrato de arrendamiento es activado (todas las partes firmaron).
    /// Payload: { leaseId, unitId, startDate }
    /// </summary>
    public const string LeaseActivated = "lease.activated";

    /// <summary>
    /// Se emite cuando un contrato es marcado en mora por facturas vencidas.
    /// Payload: { leaseId, reason }
    /// </summary>
    public const string LeaseInDefault = "lease.in_default";

    /// <summary>
    /// Se emite cuando un contrato es terminado.
    /// Payload: { leaseId, endDate }
    /// </summary>
    public const string LeaseTerminated = "lease.terminated";

    // ─── Multas y Penalidades ────────────────────────────────────────────────────

    /// <summary>
    /// Se emite cuando se emite una multa al residente.
    /// Payload: { penaltyId, reason, amount, dueDate }
    /// </summary>
    public const string PenaltyIssued = "penalty.issued";

    /// <summary>
    /// Se emite cuando se resuelve la apelación de una multa.
    /// Payload: { penaltyId, upheld, resolution }
    /// </summary>
    public const string PenaltyAppealResolved = "penalty.appeal.resolved";

    // ─── Cuotas de Administración ────────────────────────────────────────────────

    /// <summary>
    /// Se emite cuando se genera un cobro de cuota (administración, fondo de reserva, etc.).
    /// Payload: { feeType, amount, dueDate, concept }
    /// </summary>
    public const string FeeCharged = "fee.charged";

    /// <summary>
    /// Se emite cuando se crea una cuota extraordinaria que aplica a todos los residentes.
    /// Broadcast a todo el tenant.
    /// Payload: { assessmentId, title, totalAmount, yourShare }
    /// </summary>
    public const string ExtraordinaryAssessmentCreated = "assessment.created";

    // ─── Parqueadero ─────────────────────────────────────────────────────────────

    /// <summary>
    /// Se emite cuando una reserva de parqueadero para visitante es confirmada.
    /// Payload: { reservationId, parkingNumber, startTime, endTime }
    /// </summary>
    public const string ParkingReservationConfirmed = "parking.reservation.confirmed";

    /// <summary>
    /// Se emite cuando una reserva de parqueadero es cancelada.
    /// Payload: { reservationId, reason }
    /// </summary>
    public const string ParkingReservationCancelled = "parking.reservation.cancelled";

    // ─── Mensajes del Conjunto ───────────────────────────────────────────────────

    /// <summary>
    /// Se emite para enviar un mensaje broadcast a todos los residentes del conjunto.
    /// Payload: { title, message, priority }
    /// </summary>
    public const string AdminBroadcast = "admin.broadcast";
}
