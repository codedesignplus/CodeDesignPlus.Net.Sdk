namespace CodeDesignPlus.Net.Core.Abstractions
{
    /// <summary>
    /// Definition for the base entities of the repository pattern and constraints for generic types.
    /// </summary>
    public interface IEntityBase : IBase { }

    /// <summary>
    /// Definition for the base entities of the repository pattern.
    /// </summary>
    /// <typeparam name="TKey">Data type that will identify the record.</typeparam>
    /// <typeparam name="TUserKey">Data type that will identify the user.</typeparam>
    public interface IEntityBase<TKey, TUserKey> : IBase<TKey, TUserKey>, IEntityBase
    {

    }
}
