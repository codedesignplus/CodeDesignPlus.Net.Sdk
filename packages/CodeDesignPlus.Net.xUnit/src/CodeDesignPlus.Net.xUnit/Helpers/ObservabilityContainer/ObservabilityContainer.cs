namespace CodeDesignPlus.Net.xUnit.Helpers.ObservabilityContainer;

public class ObservabilityContainer : DockerCompose
{
    protected override ICompositeService Build()
    {
        var file = Path.Combine(Directory.GetCurrentDirectory(), "Helpers", "ObservabilityContainer", (TemplateString)"docker-compose.yml");

        var compose = new DockerComposeCompositeService(
            DockerHost,
            new DockerComposeConfig
            {
                ComposeFilePath = [file],
                ForceRecreate = true,
                RemoveOrphans = true,
                StopOnDispose = true
            });

        return compose;
    }

}
