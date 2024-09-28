using CodeDesignPlus.Net.Core.Abstractions;

namespace CodeDesignPlus.Net.xUnit.Microservice.Test.Helpers;

public class OrderAggregate(Guid id) : AggregateRoot(id)
{
    public ClientEntity? Client { get; private set; }

    public string? OrderStatus { get; private set; }

    public static OrderAggregate Create(Guid id)
    {
        return new OrderAggregate(id);
    }

}
