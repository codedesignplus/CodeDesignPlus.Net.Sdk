namespace CodeDesignPlus.Net.xUnit.Helpers.OpenTelemetry;

public class OpenTelemetryContainer : DockerCompose
{
    protected override ICompositeService Build()
    {
        var file = Path.Combine(Directory.GetCurrentDirectory(), "Helpers", "OpenTelemetry", (TemplateString)"docker-compose.yml");

        var dockerCompose = new DockerComposeConfig
        {
            ComposeFilePath = [file],
            ForceRecreate = true,
            RemoveOrphans = true,
            StopOnDispose = true,
            AlternativeServiceName = "otel_" + Guid.NewGuid().ToString("N"),
        };

        this.EnableGetPort = true;
        this.InternalPort = 4317;
        this.ContainerName = $"{dockerCompose.AlternativeServiceName}-otel-collector";

        var compose = new DockerComposeCompositeService(DockerHost, dockerCompose);

        return compose;
    }
}
