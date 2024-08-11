using CodeDesignPlus.Abstractions;
using CodeDesignPlus.Net.EFCore.Repository;

namespace CodeDesignPlus.InMemory.Repositories
{
    public class RolePermissionRepository : RepositoryBase, IRolePermissionRepository
    {
        public RolePermissionRepository(CodeDesignPlusContextInMemory context) : base(context)
        {
        }
    }
}
