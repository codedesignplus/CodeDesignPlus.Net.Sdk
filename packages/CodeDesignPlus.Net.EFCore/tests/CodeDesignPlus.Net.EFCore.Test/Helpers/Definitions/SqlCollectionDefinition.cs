using System;
using CodeDesignPlus.Net.xUnit.Helpers.SqlServer;

namespace CodeDesignPlus.Net.EFCore.Test.Helpers.Definitions;

[CollectionDefinition(SqlCollectionFixture.Collection)]
public class SqlCollectionDefinition : ICollectionFixture<SqlCollectionFixture>
{
}