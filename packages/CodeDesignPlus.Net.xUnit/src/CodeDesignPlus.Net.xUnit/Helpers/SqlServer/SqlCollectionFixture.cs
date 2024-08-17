using System;
using System.Net;
using Xunit;

namespace CodeDesignPlus.Net.xUnit.Helpers.SqlServer;

public sealed class SqlCollectionFixture : IDisposable
{
    public const string Collection = "SqlServer Collection";
    public  SqlServerContainer Container { get; }

    public SqlCollectionFixture()
    {
        this.Container = new SqlServerContainer();

        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

        Thread.Sleep(5000);
    }

    public void Dispose()
    {
        this.Container.StopInstance();
    }
}
