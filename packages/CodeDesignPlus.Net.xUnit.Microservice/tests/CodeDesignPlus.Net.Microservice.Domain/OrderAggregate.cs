using CodeDesignPlus.Net.Core.Abstractions;
using CodeDesignPlus.Net.Microservice.Domain.Entities;
using NodaTime;

namespace CodeDesignPlus.Net.Microservice.Domain;

public class OrderAggregate(Guid id) : AggregateRoot(id)
{
    public ClientEntity? Client { get; private set; }

    public string? OrderStatus { get; private set; }

    public static OrderAggregate Create(Guid id, ClientEntity client, string orderStatus, Guid createdBy)
    {
        return new OrderAggregate(id){
            Client = client, 
            OrderStatus = orderStatus,
            CreatedBy = createdBy,
            CreatedAt = SystemClock.Instance.GetCurrentInstant()
        };
    }

}
