using CodeDesignPlus.Abstractions;
using CodeDesignPlus.Net.EFCore.Operations;
using CodeDesignPlus.Entities;
using CodeDesignPlus.Net.Security.Abstractions;

namespace CodeDesignPlus.InMemory.Repositories
{
    public class PermissionRepository : OperationBase<long, int, Permission>, IPermissionRepository
    {
        public PermissionRepository(IUserContext<int> user, CodeDesignPlusContextInMemory context) : base(user, context)
        {
        }
    }
}
