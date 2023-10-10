namespace CodeDesignPlus.Net.Event.Bus.Test.Helpers.Events
{
    /// <summary>
    /// Evento de integración usado cuando es creado un usuarios
    /// </summary>
    public class UserRegisteredEvent : EventBase
    {
        /// <summary>
        /// Crea una nueva instancia de <see cref="UserRegisteredEvent"/>
        /// </summary>
        public UserRegisteredEvent()
        {

        }

        /// <summary>
        /// Crea una nueva instancia de <see cref="UserRegisteredEvent"/>
        /// </summary>
        /// <param name="idEvent">Id Event</param>
        /// <param name="eventDate">Date the event was generated</param>
        /// <exception cref="ArgumentOutOfRangeException">The assigned date is invalid</exception>
        public UserRegisteredEvent(Guid idEvent, DateTime eventDate) : base(idEvent, eventDate)
        {
        }

        /// <summary>
        /// Id del usaurio
        /// </summary>
        public long Id { get; set; }
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
