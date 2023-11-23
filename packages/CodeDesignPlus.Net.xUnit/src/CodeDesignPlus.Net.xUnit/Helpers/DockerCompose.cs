using Ductus.FluentDocker.Services;
using Ductus.FluentDocker.Services.Extensions;

namespace CodeDesignPlus.Net.xUnit.Helpers;

public abstract class DockerCompose : IDisposable
{

    protected string ContainerName;
    protected ICompositeService CompositeService;
    protected IHostService DockerHost;
    protected bool EnableGetPort = false;

    public string Ip { get; private set; }
    public int Port { get; private set; }

    public DockerCompose()
    {
        this.EnsureDockerHost();

        this.CompositeService = this.Build();

        try
        {
            this.CompositeService.Start();

            if (EnableGetPort && !string.IsNullOrEmpty(this.ContainerName))
            {
                var container = this.CompositeService.Containers.FirstOrDefault(x => x.Name.StartsWith(this.ContainerName));

                var endpoint = container.ToHostExposedEndpoint("27017/tcp");

                this.Ip = endpoint.Address.ToString();
                this.Port = endpoint.Port;
            }
        }
        catch
        {
            this.CompositeService.Dispose();
            throw;
        }

        this.OnContainerInitialized();
    }

    protected abstract ICompositeService Build();

    protected virtual void OnContainerTearDown()
    {
    }

    protected virtual void OnContainerInitialized()
    {
    }

    private void EnsureDockerHost()
    {
        if (this.DockerHost?.State == ServiceRunningState.Running)
            return;

        var hosts = new Hosts().Discover();

        this.DockerHost = hosts.FirstOrDefault(x => x.IsNative) ?? hosts.FirstOrDefault(x => x.Name == "default");

        if (null != this.DockerHost)
        {
            if (this.DockerHost.State != ServiceRunningState.Running)
                this.DockerHost.Start();

            return;
        }

        if (hosts.Count > 0) this.DockerHost = hosts.First();

        if (this.DockerHost != null) return;

        this.EnsureDockerHost();
    }

    public void Dispose()
    {
        this.OnContainerTearDown();
        var compositeService = this.CompositeService;
        this.CompositeService = null!;
        try
        {
            compositeService?.Dispose();
        }
        catch { }
    }

}
