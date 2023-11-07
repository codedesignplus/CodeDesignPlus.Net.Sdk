using System.Net;
using System.Net.Sockets;
using Ductus.FluentDocker.Model.Common;
using Ductus.FluentDocker.Model.Compose;
using Ductus.FluentDocker.Services;
using Ductus.FluentDocker.Services.Extensions;
using Ductus.FluentDocker.Services.Impl;

namespace CodeDesignPlus.Net.xUnit.Helpers.EventStoreContainer;

public class EventStoreContainer : DockerCompose
{

    protected override ICompositeService Build()
    {
        var file = Path.Combine(Directory.GetCurrentDirectory(), "Helpers", "EventStoreContainer", (TemplateString)"docker-compose.yml");

        var dockerCompose = new DockerComposeConfig
        {
            ComposeFilePath = new List<string> { file },
            ForceRecreate = true,
            RemoveOrphans = true,
            StopOnDispose = true,
            AlternativeServiceName = "eventstore_" + Guid.NewGuid().ToString("N"),
        };

        this.EnableGetPort = true;
        this.ContainerName = $"{dockerCompose.AlternativeServiceName}-eventstore.db";

        var compose = new DockerComposeCompositeService(base.DockerHost, dockerCompose);

        return compose;
    }
}
