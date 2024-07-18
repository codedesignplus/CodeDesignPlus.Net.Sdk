namespace CodeDesignPlus.Net.xUnit.Helpers.MongoContainer;

public class MongoContainer : DockerCompose
{
    protected override ICompositeService Build()
    {

        var file = Path.Combine(Directory.GetCurrentDirectory(), "Helpers", "MongoContainer", (TemplateString)"docker-compose.yml");

        var dockerCompose = new DockerComposeConfig
        {
            ComposeFilePath = new List<string> { file },
            ForceRecreate = true,
            RemoveOrphans = true,
            StopOnDispose = true,
            AlternativeServiceName = "mongo_" + Guid.NewGuid().ToString("N"),
        };

        this.EnableGetPort = true;
        this.InternalPort = 27017;
        this.ContainerName = $"{dockerCompose.AlternativeServiceName}-mongo";

        var compose = new DockerComposeCompositeService(base.DockerHost, dockerCompose);

        return compose;
    }
}
