using CodeDesignPlus.Net.Core.Abstractions;
using CodeDesignPlus.Net.Microservice.Domain.Entities;

namespace CodeDesignPlus.Net.Microservice.Domain;

public class OrderAggregate(Guid id) : AggregateRoot(id)
{
    public ClientEntity? Client { get; private set; }

    public string? OrderStatus { get; private set; }

    public static OrderAggregate Create(Guid id)
    {
        return new OrderAggregate(id);
    }

}
