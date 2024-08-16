using System;

namespace CodeDesignPlus.Net.xUnit.Helpers.MongoContainer;

public sealed class MongoCollectionFixture : IDisposable
{
    public const string Collection = "Mongo Collection";
    public  MongoContainer Container { get; }

    public MongoCollectionFixture()
    {
        this.Container = new MongoContainer();
    }

    public void Dispose()
    {
        this.Container.StopInstance();
    }
}
