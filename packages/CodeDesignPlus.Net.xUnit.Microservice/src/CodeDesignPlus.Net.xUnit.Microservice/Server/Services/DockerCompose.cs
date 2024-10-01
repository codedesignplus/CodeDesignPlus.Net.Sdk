using Ductus.FluentDocker.Services;

namespace CodeDesignPlus.Net.xUnit.Microservice.Server.Services;

/// <summary>
/// Represents a Docker Compose service.
/// </summary>
public abstract class DockerCompose
{

    /// <summary>
    /// Gets or sets the composite service for Docker Compose.
    /// </summary>
    protected ICompositeService CompositeService;

    /// <summary>
    /// Gets or sets the Docker host service.
    /// </summary>
    protected IHostService DockerHost;

    /// <summary>
    /// Initializes a new instance of the <see cref="DockerCompose"/> class.
    /// </summary>
    protected DockerCompose()
    {
        EnsureDockerHost();

        CompositeService = Build();

        try
        {
            CompositeService.Start();
        }
        catch
        {
            CompositeService.Dispose();

            throw;
        }

        OnContainerInitialized();
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
        if (DockerHost?.State == ServiceRunningState.Running)
            return;

        var hosts = new Hosts().Discover();
        DockerHost = hosts.FirstOrDefault(x => x.IsNative) ?? hosts.FirstOrDefault(x => x.Name == "default");

        if (DockerHost != null)
        {
            if (DockerHost.State != ServiceRunningState.Running)
                DockerHost.Start();

            return;
        }

        if (hosts.Count > 0)
            DockerHost = hosts[0];

        if (DockerHost == null)
            EnsureDockerHost();
    }

    /// <summary>
    /// Stops the Docker container instance.
    /// </summary>
    public void StopInstance()
    {
        OnContainerTearDown();
        var compositeService = CompositeService;
        CompositeService = null!;
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

