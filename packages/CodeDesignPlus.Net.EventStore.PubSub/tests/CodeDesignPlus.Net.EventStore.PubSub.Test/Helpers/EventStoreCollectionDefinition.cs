using CodeDesignPlus.Net.xUnit.Helpers.EventStoreContainer;

namespace CodeDesignPlus.Net.EventStore.PubSub.Test.Helpers;

[CollectionDefinition(EventStoreCollectionFixture.Collection)]
public class EventStoreCollectionDefinition : ICollectionFixture<EventStoreCollectionFixture>
{
}