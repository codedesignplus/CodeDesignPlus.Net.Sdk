using CodeDesignPlus.Entities;
using CodeDesignPlus.Net.EFCore.Abstractions.Operations;

namespace CodeDesignPlus.Abstractions
{
    public interface IPermissionRepository : IOperationBase<long, int, Permission>
    {
    }
}
