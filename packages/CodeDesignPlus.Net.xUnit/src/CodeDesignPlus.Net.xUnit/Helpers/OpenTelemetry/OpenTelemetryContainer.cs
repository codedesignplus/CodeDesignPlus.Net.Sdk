namespace CodeDesignPlus.Net.xUnit.Helpers.OpenTelemetry;

public class OpenTelemetryContainer : DockerCompose
{
    protected override ICompositeService Build()
    {
        var file = Path.Combine(Directory.GetCurrentDirectory(), "Helpers", "OpenTelemetry", (TemplateString)"docker-compose.yml");

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
