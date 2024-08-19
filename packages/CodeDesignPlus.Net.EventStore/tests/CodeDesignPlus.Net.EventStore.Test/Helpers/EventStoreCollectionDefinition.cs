using System;
using CodeDesignPlus.Net.xUnit.Helpers.EventStoreContainer;

namespace CodeDesignPlus.Net.EventStore.Test.Helpers;

[CollectionDefinition(EventStoreCollectionFixture.Collection)]
public class EventStoreCollectionDefinition : ICollectionFixture<EventStoreCollectionFixture>
{
}