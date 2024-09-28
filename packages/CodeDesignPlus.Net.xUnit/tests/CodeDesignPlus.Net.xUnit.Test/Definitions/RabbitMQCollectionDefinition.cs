using CodeDesignPlus.Net.xUnit.Helpers.RabbitMQContainer;

namespace CodeDesignPlus.Net.xUnit.Test.Definitions;


[CollectionDefinition(RabbitMQCollectionFixture.Collection)]
public class RabbitMQCollectionDefinition : ICollectionFixture<RabbitMQCollectionFixture>
{
}