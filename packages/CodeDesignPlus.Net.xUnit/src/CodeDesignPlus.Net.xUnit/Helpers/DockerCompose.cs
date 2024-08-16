﻿
namespace CodeDesignPlus.Net.xUnit.Helpers;

public abstract class DockerCompose 
{

    protected string ContainerName;
    protected ICompositeService CompositeService;
    protected IHostService DockerHost;
    protected bool EnableGetPort = false;
    protected int InternalPort { get; set; }
    public string Ip { get; private set; }
    public int Port { get; private set; }
    public bool IsRunning { get; }

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

                var endpoint = container.ToHostExposedEndpoint($"{this.InternalPort}/tcp");

                this.Ip = endpoint.Address.ToString();
                this.Port = endpoint.Port;
            }

            IsRunning = true;


            Thread.Sleep(2000);
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

    public void StopInstance()
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
