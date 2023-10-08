namespace CodeDesignPlus.Net.Core.Abstractions
{
    /// <summary>
    /// Definition for entities of the repository pattern.
    /// </summary>
    /// <typeparam name="TUserKey">Type that will identify the user.</typeparam>
    public interface IEntityLong<TUserKey> : IEntityBase<long, TUserKey>
    {

    }
}
