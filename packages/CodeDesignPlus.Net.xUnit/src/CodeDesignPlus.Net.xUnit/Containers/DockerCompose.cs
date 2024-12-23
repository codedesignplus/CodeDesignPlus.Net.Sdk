namespace CodeDesignPlus.Net.xUnit.Containers;

/// <summary>
/// Abstract base class for managing Docker containers using Docker Compose.
/// </summary>
public abstract class DockerCompose
{
    /// <summary>
    /// Gets or sets the name of the Docker container.
    /// </summary>
    protected string ContainerName;

    /// <summary>
    /// Gets or sets the composite service for Docker Compose.
    /// </summary>
    protected ICompositeService CompositeService;

    /// <summary>
    /// Gets or sets the Docker host service.
    /// </summary>
    protected IHostService DockerHost;

    /// <summary>
    /// Gets or sets a value indicating whether to enable port retrieval.
    /// </summary>
    protected bool EnableGetPort = false;

    /// <summary>
    /// Gets or sets the internal port of the Docker container.
    /// </summary>
    protected int InternalPort { get; set; }

    /// <summary>
    /// Gets the IP address of the Docker container.
    /// </summary>
    public string Ip { get; private set; }

    /// <summary>
    /// Gets the exposed port of the Docker container.
    /// </summary>
    public int Port { get; private set; }

    /// <summary>
    /// Gets a value indicating whether the Docker container is running.
    /// </summary>
    public bool IsRunning { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DockerCompose"/> class.
    /// </summary>
    protected DockerCompose()
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
        }
        catch
        {
            this.CompositeService.Dispose();
            throw;
        }

        this.OnContainerInitialized();
    }

    /// <summary>
    /// Builds the Docker Compose service configuration.
    /// </summary>
    /// <returns>An <see cref="ICompositeService"/> representing the Docker Compose service.</returns>
    protected abstract ICompositeService Build();

    /// <summary>
    /// Called when the Docker container is being torn down.
    /// </summary>
    protected virtual void OnContainerTearDown()
    {
    }

    /// <summary>
    /// Called when the Docker container is initialized.
    /// </summary>
    protected virtual void OnContainerInitialized()
    {
    }

    /// <summary>
    /// Ensures that the Docker host is running.
    /// </summary>
    private void EnsureDockerHost()
    {
        if (this.DockerHost?.State == ServiceRunningState.Running)
            return;

        var hosts = new Hosts().Discover();
        this.DockerHost = hosts.FirstOrDefault(x => x.IsNative) ?? hosts.FirstOrDefault(x => x.Name == "default");

        if (this.DockerHost != null)
        {
            if (this.DockerHost.State != ServiceRunningState.Running)
                this.DockerHost.Start();

            return;
        }

        if (hosts.Count > 0)
            this.DockerHost = hosts[0];

        if (this.DockerHost == null)
            this.EnsureDockerHost();
    }

    /// <summary>
    /// Stops the Docker container instance.
    /// </summary>
    public void StopInstance()
    {
        this.OnContainerTearDown();
        var compositeService = this.CompositeService;
        this.CompositeService = null!;
        try
        {
            compositeService?.Dispose();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}