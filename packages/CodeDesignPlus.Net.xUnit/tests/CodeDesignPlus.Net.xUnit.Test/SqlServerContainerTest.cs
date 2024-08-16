using System;
using CodeDesignPlus.Net.xUnit.Helpers.SqlServer;
using Microsoft.Data.SqlClient;

namespace CodeDesignPlus.Net.xUnit.Test;

[Collection(SqlCollectionFixture.Collection)]
public class SqlServerContainerTest
{
    private readonly SqlServerContainer container;

    public SqlServerContainerTest(SqlCollectionFixture sqlCollectionFixture)
    {
        this.container = sqlCollectionFixture.Container;
    }

    [Fact]
    public void Test()
    {
        // Arrange
        var sqlConnection = new SqlConnection($"Server=localhost,{this.container.Port};Database=master;User Id=sa;Password=Temporal1;Encrypt=false");

        // Act
        sqlConnection.Open();

        var transaction = sqlConnection.BeginTransaction();

        var command = sqlConnection.CreateCommand();

        command.Transaction = transaction;

        command.CommandText = "SELECT 1";

        command.ExecuteNonQuery();

        transaction.Commit();

        sqlConnection.Close();

        // Assert
        Assert.True(this.container.IsRunning);
    }
}
