namespace CodeDesignPlus.Net.xUnit.Helpers.ObservabilityContainer;

public class ObservabilityContainer : DockerCompose
{
    protected override ICompositeService Build()
    {
        var file = Path.Combine(Directory.GetCurrentDirectory(), "Helpers", "ObservabilityContainer", (TemplateString)"docker-compose.yml");

        var dockerCompose = new DockerComposeConfig
        {
            ComposeFilePath = [file],
            ForceRecreate = true,
            RemoveOrphans = true,
            StopOnDispose = true,
            AlternativeServiceName = "observability_" + Guid.NewGuid().ToString("N"),
        };

        var compose = new DockerComposeCompositeService(base.DockerHost, dockerCompose);

        return compose;
    }

}
