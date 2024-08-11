namespace CodeDesignPlus.Net.xUnit.Helpers.KafkaContainer;

public class KafkaContainer : DockerCompose
{

    protected override ICompositeService Build()
    {
        var file = Path.Combine(Directory.GetCurrentDirectory(), "Helpers", "KafkaContainer", (TemplateString)"docker-compose.yml");

        var compose = new DockerComposeCompositeService(
            DockerHost,
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
