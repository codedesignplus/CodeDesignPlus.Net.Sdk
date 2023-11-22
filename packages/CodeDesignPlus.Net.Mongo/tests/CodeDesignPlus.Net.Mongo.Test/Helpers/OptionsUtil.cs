using CodeDesignPlus.Net.Mongo.Abstractions.Options;

namespace CodeDesignPlus.Net.Mongo.Test.Helpers;

public static class OptionsUtil
{
    public static readonly MongoOptions MongoOptions = new()
    {
        Enable = true,
        Name = nameof(MongoOptions.Name),
        Email = $"{nameof(MongoOptions.Email)}@codedesignplus.com"
    };
}
