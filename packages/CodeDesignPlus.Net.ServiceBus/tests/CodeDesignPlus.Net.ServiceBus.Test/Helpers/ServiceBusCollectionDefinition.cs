using CodeDesignPlus.Net.xUnit.Containers.ServiceBusContainer;

namespace CodeDesignPlus.Net.ServiceBus.Test.Helpers;

[CollectionDefinition(ServiceBusCollectionFixture.Collection)]
public class ServiceBusCollectionDefinition : ICollectionFixture<ServiceBusCollectionFixture>
{
}
