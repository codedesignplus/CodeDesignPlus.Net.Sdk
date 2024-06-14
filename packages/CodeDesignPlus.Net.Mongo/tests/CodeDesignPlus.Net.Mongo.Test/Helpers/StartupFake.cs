using CodeDesignPlus.Net.Core.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeDesignPlus.Net.Mongo.Test.Helpers
{
    public class StartupFake : IStartupServices
    {
        public void Initialize(IServiceCollection services, IConfiguration configuration)
        {

        }
    }
}
