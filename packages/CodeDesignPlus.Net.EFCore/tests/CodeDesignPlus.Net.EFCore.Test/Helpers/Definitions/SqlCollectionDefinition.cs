using System;
using CodeDesignPlus.Net.xUnit.Containers.SqlServer;

namespace CodeDesignPlus.Net.EFCore.Test.Helpers.Definitions;

[CollectionDefinition(SqlCollectionFixture.Collection)]
public class SqlCollectionDefinition : ICollectionFixture<SqlCollectionFixture>
{
}