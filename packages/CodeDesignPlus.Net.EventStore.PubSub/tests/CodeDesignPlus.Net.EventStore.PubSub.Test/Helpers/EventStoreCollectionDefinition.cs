using CodeDesignPlus.Net.xUnit.Containers.EventStoreContainer;

namespace CodeDesignPlus.Net.EventStore.PubSub.Test.Helpers;

[CollectionDefinition(EventStoreCollectionFixture.Collection)]
public class EventStoreCollectionDefinition : ICollectionFixture<EventStoreCollectionFixture>
{
}