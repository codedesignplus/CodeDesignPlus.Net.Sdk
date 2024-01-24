namespace CodeDesignPlus.Net.Core.Test.Helpers.Context
{
    /// <summary>
    /// Entidad fake que permitira validar el modelo de paginación
    /// </summary>
    public class FakeEntity : IEntity
    {
        /// <summary>
        /// Id del registro
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// Fake Name
        /// </summary>
        public string? Name { get; set; }
        /// <summary>
        /// Estado del registro
        /// </summary>
        public bool IsActive { get; set; }
        /// <summary>
        /// Id del usuario que creo el registro
        /// </summary>
        public Guid IdUserCreator { get; set; }
        /// <summary>
        /// Fecha de creación del registro
        /// </summary>
        public DateTime CreatedAt { get; set; }
        /// <summary>
        /// Id del usuario que actualizo el registro
        /// </summary>
        public Guid Tenant { get; set; }
    }
}
