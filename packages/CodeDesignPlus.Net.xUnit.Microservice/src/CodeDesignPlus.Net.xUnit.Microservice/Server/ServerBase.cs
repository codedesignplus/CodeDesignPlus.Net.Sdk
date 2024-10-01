using CodeDesignPlus.Net.xUnit.Microservice.Server.Logger;
using Grpc.Net.Client;

namespace CodeDesignPlus.Net.xUnit.Microservice.Server;
/// <summary>
/// A base server class for configuring and managing a web application for testing purposes.
/// </summary>
/// <typeparam name="TProgram">The program class of the web application.</typeparam>
/// <param name="server">The server instance.</param>
public class ServerBase<TProgram>(Server<TProgram> server) : IAsyncLifetime where TProgram : class
{
    private AsyncServiceScope scope;

    /// <summary>
    /// Gets the service provider for the application.
    /// </summary>
    protected IServiceProvider Services;

    /// <summary>
    /// Gets the HTTP client for making requests to the server.
    /// </summary>
    protected HttpClient Client;

    /// <summary>
    /// Gets the in-memory logger provider for capturing log messages.
    /// </summary>
    protected InMemoryLoggerProvider LoggerProvider => server.LoggerProvider;

    /// <summary>
    /// Gets the gRPC channel for making gRPC calls to the server.
    /// </summary>
    protected GrpcChannel Channel;

    /// <summary>
    /// Initializes the server and its resources asynchronously.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task InitializeAsync()
    {
        Client = server.CreateClient();
        scope = server.Services.CreateAsyncScope();
        Services = scope.ServiceProvider;

        var options = new GrpcChannelOptions
        {
            HttpHandler = server.Server.CreateHandler()
        };

        Channel = GrpcChannel.ForAddress(server.Server.BaseAddress, options);

        return Task.CompletedTask;
    }

    /// <summary>
    /// Disposes the server and its resources asynchronously.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
}