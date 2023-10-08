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
    public interface IBase<TKey, TUserKey>: IBase
    {
        /// <summary>
        /// Record's ID.
        /// </summary>
        public TKey Id { get; set; }
        /// <summary>
        /// Record's state.
        /// </summary>
        public bool State { get; set; }
        /// <summary>
        /// ID of the user who created the record.
        /// </summary>
        public TUserKey IdUserCreator { get; set; }
        /// <summary>
        /// Record's creation date.
        /// </summary>
        public DateTime DateCreated { get; set; }
    }
}
