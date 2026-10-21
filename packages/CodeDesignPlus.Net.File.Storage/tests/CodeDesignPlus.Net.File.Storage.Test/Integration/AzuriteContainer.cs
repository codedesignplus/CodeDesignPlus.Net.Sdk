using System.Net;
using System.Net.Sockets;
using Azure.Storage.Blobs;
using CodeDesignPlus.Net.xUnit.Containers;
using Ductus.FluentDocker.Model.Compose;
using Ductus.FluentDocker.Services;
using Ductus.FluentDocker.Services.Impl;

namespace CodeDesignPlus.Net.File.Storage.Test.Integration;

/// <summary>
/// Azurite, the Azure Storage emulator, with only the Blob service.
/// </summary>
/// <remarks>
/// Azurite listens on the same port inside and outside the container: before copying a blob it reads the source URL it was
/// given, from inside the container, and that URL carries the port the test sees. With a mapped port (<c>0:10000</c>) the
/// copy fails with a 500 (<c>ECONNREFUSED</c>). So a free port is taken and the compose file is written with it.
/// </remarks>
public sealed class AzuriteContainer : DockerCompose
{
    /// <summary>
    /// The well-known development account of Azurite: it is public and only works against the emulator.
    /// </summary>
    private const string AccountName = "devstoreaccount1";

    /// <summary>
    /// The well-known key of <see cref="AccountName"/>, published in the Azurite documentation.
    /// </summary>
    private const string AccountKey = "Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==";

    /// <summary>
    /// Gets the connection string of the emulator, with the account key: the provider can sign the source of a copy with it.
    /// </summary>
    public string ConnectionString => $"DefaultEndpointsProtocol=http;AccountName={AccountName};AccountKey={AccountKey};BlobEndpoint=http://127.0.0.1:{this.Port}/{AccountName};";

    /// <summary>
    /// Creates a client of the emulator.
    /// </summary>
    /// <returns>The client.</returns>
    public BlobServiceClient CreateClient() => new(this.ConnectionString);

    /// <inheritdoc />
    protected override ICompositeService Build()
    {
        var port = FreePort();
        var file = Path.Combine(Path.GetTempPath(), $"azurite-{Guid.NewGuid():N}.yml");

        System.IO.File.WriteAllText(file, $"""
            services:
              azurite:
                image: mcr.microsoft.com/azure-storage/azurite:latest
                command: ["azurite-blob", "--blobHost", "0.0.0.0", "--blobPort", "{port}", "--skipApiVersionCheck"]
                ports:
                  - "{port}:{port}"
            """);

        var dockerCompose = new DockerComposeConfig
        {
            ComposeFilePath = [file],
            ForceRecreate = true,
            RemoveOrphans = true,
            StopOnDispose = true,
            AlternativeServiceName = "azurite_" + Guid.NewGuid().ToString("N"),
            ComposeVersion = ComposeVersion.V2,
        };

        this.EnableGetPort = true;
        this.InternalPort = port;
        this.ContainerName = "azurite";

        return new DockerComposeCompositeService(base.DockerHost, dockerCompose);
    }

    /// <summary>
    /// A port nobody listens on right now.
    /// </summary>
    /// <returns>The port.</returns>
    private static int FreePort()
    {
        using var listener = new TcpListener(IPAddress.Loopback, 0);

        listener.Start();

        return ((IPEndPoint)listener.LocalEndpoint).Port;
    }
}

/// <summary>
/// Shares one Azurite container among the tests of the collection.
/// </summary>
public sealed class AzuriteCollectionFixture : IDisposable
{
    /// <summary>
    /// The name of the collection.
    /// </summary>
    public const string Collection = "Azurite Collection";

    /// <summary>
    /// The largest amount of time the fixture waits for the emulator to answer.
    /// </summary>
    private static readonly TimeSpan Timeout = TimeSpan.FromMinutes(1);

    /// <summary>
    /// Gets the container.
    /// </summary>
    public AzuriteContainer Container { get; }

    /// <summary>
    /// Starts the container and waits until the Blob service answers.
    /// </summary>
    public AzuriteCollectionFixture()
    {
        this.Container = new AzuriteContainer();

        var client = this.Container.CreateClient();
        var deadline = DateTime.UtcNow.Add(Timeout);

        while (true)
        {
            try
            {
                client.GetProperties();

                return;
            }
            catch (Exception) when (DateTime.UtcNow < deadline)
            {
                // The emulator does not accept connections yet.
                Thread.Sleep(500);
            }
        }
    }

    /// <summary>
    /// Stops the container.
    /// </summary>
    public void Dispose()
    {
        this.Container.StopInstance();
    }
}

/// <summary>
/// The collection of the tests that run against Azurite.
/// </summary>
[CollectionDefinition(AzuriteCollectionFixture.Collection)]
public sealed class AzuriteCollection : ICollectionFixture<AzuriteCollectionFixture>
{
}
