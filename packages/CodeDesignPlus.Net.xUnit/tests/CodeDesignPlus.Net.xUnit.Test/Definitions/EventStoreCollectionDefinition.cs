using System;
using CodeDesignPlus.Net.xUnit.Containers.EventStoreContainer;

namespace CodeDesignPlus.Net.xUnit.Test.Definitions;

[CollectionDefinition(EventStoreCollectionFixture.Collection)]
public class EventStoreCollectionDefinition : ICollectionFixture<EventStoreCollectionFixture>
{
}