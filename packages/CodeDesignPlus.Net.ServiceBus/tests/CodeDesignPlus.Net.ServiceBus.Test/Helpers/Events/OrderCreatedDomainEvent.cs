using CodeDesignPlus.Net.Core.Abstractions;
using CodeDesignPlus.Net.Core.Abstractions.Attributes;
using CodeDesignPlus.Net.ServiceBus.Test.Helpers.Entities;

namespace CodeDesignPlus.Net.ServiceBus.Test.Helpers.Events;

/// <summary>
/// Event used only by the provisioning test, so it owns its own topic and subscription.
/// </summary>
/// <remarks>
/// Compartir la suscripcion entre pruebas las acopla: la primera que la crea fija su MaxDeliveryCount y la
/// libreria, con buen criterio, no toca una suscripcion que ya existe.
/// </remarks>
[EventKey<OrderEntity>(1, "created")]
public class OrderCreatedDomainEvent(Guid aggregateId, Guid? eventId = null, Instant? occurredAt = null, Dictionary<string, object>? metadata = null)
    : DomainEvent(aggregateId, eventId, occurredAt, metadata)
{
}
