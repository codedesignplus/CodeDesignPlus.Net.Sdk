using System;
using CodeDesignPlus.Net.xUnit.Helpers.RabbitMQContainer;

namespace CodeDesignPlus.Net.RabbitMQ.Test.Helpers;

[CollectionDefinition(RabbitMQCollectionFixture.Collection)]
public class RabbitMQCollectionDefinition : ICollectionFixture<RabbitMQCollectionFixture>
{
}