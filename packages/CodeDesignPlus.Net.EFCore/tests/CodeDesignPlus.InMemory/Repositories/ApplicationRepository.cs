﻿using CodeDesignPlus.Abstractions;
using CodeDesignPlus.Net.EFCore.Repository;

namespace CodeDesignPlus.InMemory.Repositories
{
    public class ApplicationRepository : RepositoryBase<long, int>, IApplicationRepository
    {
        public ApplicationRepository(CodeDesignPlusContextInMemory context) : base(context)
        {
        }
    }
}
