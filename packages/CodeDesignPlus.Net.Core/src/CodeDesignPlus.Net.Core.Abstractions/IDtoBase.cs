namespace CodeDesignPlus.Net.Core.Abstractions
{
    /// <summary>
    /// Definition for base entities and constraint for generic types.
    /// </summary>
    public interface IDtoBase : IBase { }

    /// <summary>
    /// Definition for base entities.
    /// </summary>
    /// <typeparam name="TKey">Data type that will identify the record.</typeparam>
    /// <typeparam name="TUserKey">Data type that will identify the user.</typeparam>
    public interface IDtoBase<TKey, TUserKey> : IBase<TKey, TUserKey>, IDtoBase
    {

    }
}
