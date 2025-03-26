using System;
using CodeDesignPlus.Net.Core.Abstractions;
using CodeDesignPlus.Net.Microservice.Domain.ValueObjects;
using NodaTime;

namespace CodeDesignPlus.Net.Microservice.Domain.Aggregate;

public class OrderBase(Guid id) : AggregateRootBase(id)
{
    public OrderBase(Guid id, string name, string description, decimal price, AddressValueObject address, Instant createdAt) : this(id)
    {
        this.Name = name;
        this.Description = description;
        this.Price = price;
        this.CreatedAt = createdAt;
        this.Address = address;
    }

    public string Name { get; } = null!;
    public string Description { get; } = null!;
    public decimal Price { get; }

    public AddressValueObject Address { get; } = null!;

    public static OrderBase Create(Guid id, string name, string description, decimal price, AddressValueObject address, Instant createdAt)
    {
        return new OrderBase(id, name, description, price, address, createdAt);
    }
}
