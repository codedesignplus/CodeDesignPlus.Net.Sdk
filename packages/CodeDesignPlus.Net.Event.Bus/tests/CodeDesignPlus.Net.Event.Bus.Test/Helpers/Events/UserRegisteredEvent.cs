namespace CodeDesignPlus.Net.Event.Bus.Test.Helpers.Events
{
    /// <summary>
    /// Evento de integración usado cuando es creado un usuarios
    /// </summary>
    public class UserRegisteredEvent : EventBase
    {
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
