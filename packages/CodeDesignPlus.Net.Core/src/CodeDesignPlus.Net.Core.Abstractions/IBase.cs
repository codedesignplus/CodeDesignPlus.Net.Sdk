namespace CodeDesignPlus.Net.Core.Abstractions
{
    /// <summary>
    /// Defines the base structure for entities, DTOs, and generic types.
    /// </summary>
    public interface IBase { }

    /// <summary>
    /// Defines the base structure for entities and DTOs.
    /// </summary>
    /// <typeparam name="TKey">Data type that will identify the record.</typeparam>
    /// <typeparam name="TUserKey">Data type that will identify the user.</typeparam>
    public interface IBase<TKey, TUserKey> : IBase, IAuditTrail<TUserKey>
    {
        /// <summary>
        /// Gets or sets the primary identifier of the record.
        /// </summary>
        TKey Id { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the record is active.
        /// </summary>
        bool IsActive { get; set; }
    }
}
