namespace CodeDesignPlus.Net.xUnit.Helpers.RabbitMQContainer;

public class RabbitMQContainer : DockerCompose
{
    protected override ICompositeService Build()
    {
        var file = Path.Combine(Directory.GetCurrentDirectory(), "Helpers", "RabbitMQContainer", (TemplateString)"docker-compose.yml");

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