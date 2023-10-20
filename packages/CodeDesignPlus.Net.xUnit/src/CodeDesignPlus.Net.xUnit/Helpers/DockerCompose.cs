using Ductus.FluentDocker.Services;

namespace CodeDesignPlus.Net.xUnit.Helpers;

public abstract class DockerCompose : IDisposable
{
    protected ICompositeService CompositeService;
    protected IHostService DockerHost;

    public DockerCompose()
    {
        this.EnsureDockerHost();

        this.CompositeService = this.Build();

        try
        {
            this.CompositeService.Start();
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
