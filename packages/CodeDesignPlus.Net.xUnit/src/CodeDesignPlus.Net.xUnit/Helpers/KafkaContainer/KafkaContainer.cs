namespace CodeDesignPlus.Net.xUnit.Helpers.KafkaContainer;

public class KafkaContainer : DockerCompose
{
    public string BrokerList { get; private set; }
    protected override ICompositeService Build()
    {
        var file = Path.Combine(Directory.GetCurrentDirectory(), "Helpers", "KafkaContainer", (TemplateString)"docker-compose.yml");

        var random = new Random();

        var hostPort = random.Next(29000, 29999) ;

        this.BrokerList = $"localhost:{hostPort}";

        var compose = new DockerComposeConfig
        {
            ComposeFilePath = [file],
            ForceRecreate = true,
            RemoveOrphans = true,
            StopOnDispose = true,
            AlternativeServiceName = "kafka_" + Guid.NewGuid().ToString("N"),
            EnvironmentNameValue = new Dictionary<string, string>
            {
                { "KAFKA_PORT" , random.Next(9000, 9999).ToString() },
                { "KAFKA_HOST_PORT", hostPort.ToString() }
            }
        };


        this.EnableGetPort = false;
        this.ContainerName = $"{compose.AlternativeServiceName}-kafka";

        return new DockerComposeCompositeService(DockerHost, compose);
    }

}
