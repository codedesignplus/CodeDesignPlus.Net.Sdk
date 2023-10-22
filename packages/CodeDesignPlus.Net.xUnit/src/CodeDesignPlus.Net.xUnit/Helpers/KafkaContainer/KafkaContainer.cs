using CodeDesignPlus.Net.xUnit.Helpers;
using Ductus.FluentDocker.Model.Common;
using Ductus.FluentDocker.Model.Compose;
using Ductus.FluentDocker.Services;
using Ductus.FluentDocker.Services.Impl;
using Moq;

namespace CodeDesignPlus.Net.xUnit;

public class KafkaContainer : DockerCompose
{

    protected override ICompositeService Build()
    {
        var file = Path.Combine(Directory.GetCurrentDirectory(), "Helpers", "KafkaContainer", (TemplateString)"docker-compose.yml");

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
