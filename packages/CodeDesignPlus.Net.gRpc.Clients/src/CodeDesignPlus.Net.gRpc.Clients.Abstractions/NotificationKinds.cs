namespace CodeDesignPlus.Net.gRpc.Clients.Abstractions;

/// <summary>
/// El catalogo de tipos de aviso de la plataforma.
/// </summary>
/// <remarks>
/// Reemplaza a <c>NotificationEventNames</c>, que estaba duplicado byte a byte en los dos SDKs y no
/// cubria ni la mitad de lo que los micros ya emitian: <c>pqrs.*</c>, <c>package.received</c>,
/// <c>infraction.sanctioned</c> y <c>council.meeting.completed</c> viajaban como literales sueltos, con
/// lo cual un typo no rompia nada y el aviso simplemente no llegaba.
/// <para>
/// La clave es una cadena con puntos y nunca un numero: un identificador que viaja entre microservicios
/// no puede ser la posicion de un enum, porque reordenar el enum en un repo cambia el significado de los
/// mensajes de los otros doce (regla 27).
/// </para>
/// <para>
/// La division entre esta clase y <see cref="Live"/> es la decision de diseño, no una agrupacion
/// estetica: lo de aqui se persiste y entra a la campana; lo de <see cref="Live"/> se pierde si nadie
/// esta mirando, y esta bien.
/// </para>
/// </remarks>
public static class NotificationKinds
{
    // ─── Cuentas de cobro ────────────────────────────────────────────────────────

    /// <summary>Cuenta de cobro emitida al residente. Payload: { documentId, amount, dueDate, concept }</summary>
    public const string InvoiceIssued = "invoice.issued";

    /// <summary>Cuenta de cobro pagada por completo. Payload: { documentId, amount, paidDate }</summary>
    public const string InvoicePaid = "invoice.paid";

    /// <summary>Cuenta de cobro vencida sin pagar. Payload: { documentId, amount, daysPastDue }</summary>
    public const string InvoiceOverdue = "invoice.overdue";

    /// <summary>Pago parcial aplicado a una cuenta de cobro. Payload: { documentId, amountPaid, amountDue }</summary>
    public const string InvoicePartiallyPaid = "invoice.partially_paid";

    // ─── Pagos ───────────────────────────────────────────────────────────────────

    /// <summary>Pago procesado con exito por la pasarela. Payload: { paymentId, amount, currency }</summary>
    public const string PaymentSucceeded = "payment.succeeded";

    /// <summary>Pago rechazado por la pasarela. Payload: { paymentId, reason }</summary>
    public const string PaymentFailed = "payment.failed";

    // ─── Reservas de areas comunes ───────────────────────────────────────────────

    /// <summary>Reserva confirmada tras el pago. Payload: { bookingId, commonAreaName, startTime, endTime }</summary>
    public const string BookingConfirmed = "booking.confirmed";

    /// <summary>Reserva cancelada. Payload: { bookingId, commonAreaName, reason }</summary>
    public const string BookingCancelled = "booking.cancelled";

    /// <summary>Recordatorio una hora antes de la reserva. Payload: { bookingId, commonAreaName, startTime, hoursUntil }</summary>
    public const string BookingReminder = "booking.reminder";

    /// <summary>Reserva cerrada con daños reportados. Payload: { bookingId, damageCost, damageNotes }</summary>
    public const string BookingCompletedWithDamages = "booking.completed_with_damages";

    // ─── Contratos de arrendamiento ──────────────────────────────────────────────

    /// <summary>Contrato listo para firma digital. Payload: { leaseId, unitId }</summary>
    public const string LeaseReadyForSignature = "lease.ready_for_signature";

    /// <summary>Contrato activado: todas las partes firmaron. Payload: { leaseId, unitId, startDate }</summary>
    public const string LeaseActivated = "lease.activated";

    /// <summary>Contrato en mora por cuentas de cobro vencidas. Payload: { leaseId, reason }</summary>
    public const string LeaseInDefault = "lease.in_default";

    /// <summary>Contrato terminado. Payload: { leaseId, endDate }</summary>
    public const string LeaseTerminated = "lease.terminated";

    // ─── Multas y sanciones ──────────────────────────────────────────────────────

    /// <summary>Multa emitida al residente. Payload: { penaltyId, reason, amount, dueDate }</summary>
    public const string PenaltyIssued = "penalty.issued";

    /// <summary>Apelacion de una multa resuelta. Payload: { penaltyId, upheld, resolution }</summary>
    public const string PenaltyAppealResolved = "penalty.appeal.resolved";

    /// <summary>Infraccion sancionada por el comite de convivencia. Payload: { infractionId, sanction }</summary>
    public const string InfractionSanctioned = "infraction.sanctioned";

    // ─── Cuotas ──────────────────────────────────────────────────────────────────

    /// <summary>Cobro de cuota generado. Payload: { feeType, amount, dueDate, concept }</summary>
    public const string FeeCharged = "fee.charged";

    /// <summary>Cuota extraordinaria creada, para toda la copropiedad. Payload: { assessmentId, title, totalAmount, yourShare }</summary>
    public const string ExtraordinaryAssessmentCreated = "assessment.created";

    // ─── Parqueadero ─────────────────────────────────────────────────────────────

    /// <summary>Reserva de parqueadero de visitante confirmada. Payload: { reservationId, parkingNumber, startTime, endTime }</summary>
    public const string ParkingReservationConfirmed = "parking.reservation.confirmed";

    /// <summary>Reserva de parqueadero cancelada. Payload: { reservationId, reason }</summary>
    public const string ParkingReservationCancelled = "parking.reservation.cancelled";

    // ─── PQRS ────────────────────────────────────────────────────────────────────

    /// <summary>PQRS radicada. Payload: { pqrsId, code, subject }</summary>
    public const string PqrsCreated = "pqrs.created";

    /// <summary>PQRS asignada a un responsable. Payload: { pqrsId, code, assignedTo }</summary>
    public const string PqrsAssigned = "pqrs.assigned";

    /// <summary>PQRS resuelta. Payload: { pqrsId, code, resolution }</summary>
    public const string PqrsResolved = "pqrs.resolved";

    /// <summary>PQRS cerrada. Payload: { pqrsId, code }</summary>
    public const string PqrsClosed = "pqrs.closed";

    /// <summary>PQRS escalada al comite de convivencia. Payload: { pqrsId, code, reason }</summary>
    public const string PqrsEscalatedToCommittee = "pqrs.escalated.committee";

    /// <summary>PQRS escalada al consejo. Payload: { pqrsId, code, reason }</summary>
    public const string PqrsEscalatedToCouncil = "pqrs.escalated.council";

    // ─── Porteria ────────────────────────────────────────────────────────────────

    /// <summary>Paquete recibido en porteria. Payload: { packageId, unitNumber, carrier }</summary>
    public const string PackageReceived = "package.received";

    // ─── Organos de administracion ───────────────────────────────────────────────

    /// <summary>Reunion de consejo cerrada con acta. Payload: { meetingId, date, minutesId }</summary>
    public const string CouncilMeetingCompleted = "council.meeting.completed";

    // ─── Mensajes del conjunto ───────────────────────────────────────────────────

    /// <summary>Mensaje de la administracion a toda la copropiedad. Payload: { title, message, priority }</summary>
    public const string AdminBroadcast = "admin.broadcast";

    /// <summary>
    /// Nombres de evento del canal efimero. <b>Nada de esto se persiste.</b>
    /// </summary>
    /// <remarks>
    /// Todos son progreso de un proceso largo, y la verdad de ese proceso no esta aqui: esta en el recurso
    /// con estado que el micro dueño expone por REST. Estos mensajes solo aceleran la pantalla de quien
    /// esta mirando en ese momento; quien llegue despues pregunta por el estado y se entera igual.
    /// <para>
    /// Persistirlos era lo que llenaba Mongo: un reparto de cuota extraordinaria de 200 unidades dejaba
    /// 200 documentos de progreso que nadie leyo jamas.
    /// </para>
    /// </remarks>
    public static class Live
    {
        /// <summary>Avance de la generacion de cargos. Payload: { processed, total }</summary>
        public const string ChargeGenerationProgress = "charge.generation.progress";

        /// <summary>Generacion de cargos terminada. Payload: { total, durationMs }</summary>
        public const string ChargeGenerationCompleted = "charge.generation.completed";

        /// <summary>Generacion de cargos abortada. Payload: { reason }</summary>
        public const string ChargeGenerationFailed = "charge.generation.failed";

        /// <summary>Avance del reparto de una cuota extraordinaria. Payload: { processed, total }</summary>
        public const string AssessmentDistributionProgress = "assessment.distribution.progress";

        /// <summary>Reparto de la cuota extraordinaria terminado. Payload: { total, distributed }</summary>
        public const string AssessmentDistributionCompleted = "assessment.distribution.completed";

        /// <summary>Reparto de la cuota extraordinaria abortado. Payload: { reason }</summary>
        public const string AssessmentDistributionFailed = "assessment.distribution.failed";
    }
}
