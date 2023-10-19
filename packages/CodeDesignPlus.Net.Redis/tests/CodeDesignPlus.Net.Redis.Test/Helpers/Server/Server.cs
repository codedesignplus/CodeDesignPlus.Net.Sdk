using Ductus.FluentDocker.Model.Common;
using Ductus.FluentDocker.Model.Compose;
using Ductus.FluentDocker.Services;
using Ductus.FluentDocker.Services.Impl;

namespace CodeDesignPlus.Net.Redis.Test.Helpers.Server;

public class Server : DockerCompose
{
    public Server()
    {

    }

    protected override ICompositeService Build()
    {
        var file = Path.Combine(Directory.GetCurrentDirectory(), "Helpers", "Server", (TemplateString)"docker-compose.standalone.yml");

        var compose = new DockerComposeCompositeService(
            base.DockerHost,
            new DockerComposeConfig
            {
                ComposeFilePath = new List<string> { file },
                ForceRecreate = true,
                RemoveOrphans = true,
                StopOnDispose = true
            });

        return compose;
    }
}
