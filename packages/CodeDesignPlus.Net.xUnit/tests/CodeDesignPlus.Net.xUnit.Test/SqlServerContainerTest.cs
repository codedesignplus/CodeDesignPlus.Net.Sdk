using System;
using System.Net;
using CodeDesignPlus.Net.xUnit.Helpers.SqlServer;
using Microsoft.Data.SqlClient;

namespace CodeDesignPlus.Net.xUnit.Test;

[Collection(SqlCollectionFixture.Collection)]
public class SqlServerContainerTest(SqlCollectionFixture sqlCollectionFixture)
{
    private readonly SqlServerContainer container = sqlCollectionFixture.Container;

    [Fact]
    public void CheckConnectionService()
    {
        // Arrange
        var sqlConnection = new SqlConnection($"Server=localhost,{this.container.Port};Database=master;User Id=sa;Password=Temporal1;Encrypt=True;TrustServerCertificate=True");

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
