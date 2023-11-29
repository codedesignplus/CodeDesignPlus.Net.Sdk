using CodeDesignPlus.Net.Mongo.Abstractions.Options;

namespace CodeDesignPlus.Net.Mongo.Test.Helpers;

public static class OptionsUtil
{
    public static MongoOptions MongoOptions => new()
    {
        Enable = true,
        ConnectionString = "mongodb://localhost:27017",
        Database = "dbtestmongo"
    };

    public static MongoOptions GetOptions(int port) => new()
    {
        Enable = true,
        ConnectionString = $"mongodb://localhost:{port}",
        Database = "dbtestmongo"
    };
}
