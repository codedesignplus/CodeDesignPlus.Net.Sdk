using CodeDesignPlus.Abstractions;
using CodeDesignPlus.Net.EFCore.Repository;

namespace CodeDesignPlus.InMemory.Repositories
{
    public class AppPermissionRepository : RepositoryBase<long, int>, IAppPermissionRepository
    {
        public AppPermissionRepository(CodeDesignPlusContextInMemory context) : base(context)
        {
        }
    }
}
