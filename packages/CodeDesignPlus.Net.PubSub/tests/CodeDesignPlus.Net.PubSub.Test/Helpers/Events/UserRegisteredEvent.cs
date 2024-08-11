using CodeDesignPlus.Net.Core.Abstractions;
using CodeDesignPlus.Net.Core.Abstractions.Attributes;

namespace CodeDesignPlus.Net.PubSub.Test.Helpers.Events
{
    /// <summary>
    /// Evento de integración usado cuando es creado un usuarios
    /// </summary>
    [EventKey<UserEntity>(1, "created")]
    public class UserRegisteredEvent : DomainEvent
    {
        public UserRegisteredEvent(Guid aggregateId, Guid? eventId = null, DateTime? occurredAt = null, Dictionary<string, object> metadata = null!)
            : base(aggregateId, eventId, occurredAt, metadata)
        {
        }

        /// <summary>
        /// Nombre del usuario creado
        /// </summary>
        public required string Name { get; set; }
        /// <summary>
        /// UserName
        /// </summary>
        public required string User { get; set; }
        /// <summary>
        /// Edad del usuario
        /// </summary>
        public ushort Age { get; set; }
    }
}
