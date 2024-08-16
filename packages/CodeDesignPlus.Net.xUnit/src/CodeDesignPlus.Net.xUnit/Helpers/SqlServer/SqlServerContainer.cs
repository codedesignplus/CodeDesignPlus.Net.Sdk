using System;

namespace CodeDesignPlus.Net.xUnit.Helpers.SqlServer;

public class SqlServerContainer : DockerCompose
{
    protected override ICompositeService Build()
    {

        var file = Path.Combine(Directory.GetCurrentDirectory(), "Helpers", "SqlServer", (TemplateString)"docker-compose.yml");

        var dockerCompose = new DockerComposeConfig
        {
            ComposeFilePath = [file],
            ForceRecreate = true,
            RemoveOrphans = true,
            StopOnDispose = true,
            AlternativeServiceName = "sql_" + Guid.NewGuid().ToString("N"),
        };

        this.EnableGetPort = true;
        this.InternalPort = 1433;
        this.ContainerName = $"{dockerCompose.AlternativeServiceName}-sqlserver";

        var compose = new DockerComposeCompositeService(base.DockerHost, dockerCompose);

        return compose;
    }
}
