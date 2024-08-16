using System;
using CodeDesignPlus.Net.xUnit.Helpers.SqlServer;

namespace CodeDesignPlus.Net.xUnit.Test.Definitions;


[CollectionDefinition(SqlCollectionFixture.Collection)]
public class SqlCollectionDefinition : ICollectionFixture<SqlCollectionFixture>
{
}