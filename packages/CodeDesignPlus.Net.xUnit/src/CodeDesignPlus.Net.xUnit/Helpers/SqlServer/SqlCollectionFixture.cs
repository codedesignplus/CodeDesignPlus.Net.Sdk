using System;
using Xunit;

namespace CodeDesignPlus.Net.xUnit.Helpers.SqlServer;

public sealed class SqlCollectionFixture : IDisposable
{
    public const string Collection = "SqlServer Collection";
    public  SqlServerContainer Container { get; }

    public SqlCollectionFixture()
    {
        this.Container = new SqlServerContainer();
    }

    public void Dispose()
    {
        this.Container.StopInstance();
    }
}
