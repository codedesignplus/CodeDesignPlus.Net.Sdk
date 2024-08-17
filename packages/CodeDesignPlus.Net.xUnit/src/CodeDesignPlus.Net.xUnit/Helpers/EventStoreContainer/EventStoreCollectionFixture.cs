using System;

namespace CodeDesignPlus.Net.xUnit.Helpers.EventStoreContainer;

public sealed class EventStoreCollectionFixture : IDisposable
{
    public const string Collection = "EventStore Collection";
    public  EventStoreContainer Container { get; }

    public EventStoreCollectionFixture()
    {
        this.Container = new EventStoreContainer();
    }

    public void Dispose()
    {
        this.Container.StopInstance();
    }
}
