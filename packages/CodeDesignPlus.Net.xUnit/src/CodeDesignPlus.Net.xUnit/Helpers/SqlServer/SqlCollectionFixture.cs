using System.Net;

namespace CodeDesignPlus.Net.xUnit.Helpers.SqlServer;

/// <summary>
/// Provides a fixture for managing a SQL Server container during xUnit tests.
/// </summary>
public sealed class SqlCollectionFixture : IDisposable
{
    /// <summary>
    /// The name of the collection for the SQL Server tests.
    /// </summary>
    public const string Collection = "SqlServer Collection";

    /// <summary>
    /// Gets the SQL Server container instance.
    /// </summary>
    public SqlServerContainer Container { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SqlCollectionFixture"/> class.
    /// </summary>
    public SqlCollectionFixture()
    {
        this.Container = new SqlServerContainer();

        // Set the security protocol to TLS 1.2.
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

        // Wait for the SQL Server container to be fully initialized.
        Thread.Sleep(10000);
    }

    /// <summary>
    /// Disposes the SQL Server container instance.
    /// </summary>
    public void Dispose()
    {
        this.Container.StopInstance();
    }
}