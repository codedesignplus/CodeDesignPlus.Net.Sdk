
namespace CodeDesignPlus.Net.xUnit.Helpers.EventStoreContainer;

public class EventStoreContainer : DockerCompose
{
    protected override ICompositeService Build()
    {
        this.InternalPort = 1113;

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
