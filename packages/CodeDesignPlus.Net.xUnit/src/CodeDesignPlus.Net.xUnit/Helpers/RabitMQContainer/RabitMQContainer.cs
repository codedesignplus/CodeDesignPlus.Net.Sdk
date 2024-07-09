using Ductus.FluentDocker.Model.Common;
using Ductus.FluentDocker.Model.Compose;
using Ductus.FluentDocker.Services;
using Ductus.FluentDocker.Services.Impl;

namespace CodeDesignPlus.Net.xUnit.Helpers.RabitMQContainer;

public class RabitMQContainer : DockerCompose
{
    protected override ICompositeService Build()
    {
        var file = Path.Combine(Directory.GetCurrentDirectory(), "Helpers", "RabitMQContainer", (TemplateString)"docker-compose.yml");

        var dockerCompose = new DockerComposeConfig
        {
            ComposeFilePath = [file],
            ForceRecreate = true,
            RemoveOrphans = true,
            StopOnDispose = true,
            AlternativeServiceName = "rabbitmq_" + Guid.NewGuid().ToString("N"),
        };

        this.EnableGetPort = true;
        this.InternalPort = 5672;
        this.ContainerName = $"{dockerCompose.AlternativeServiceName}-rabbitmq";

        var compose = new DockerComposeCompositeService(base.DockerHost, dockerCompose);

        return compose;
    }
}