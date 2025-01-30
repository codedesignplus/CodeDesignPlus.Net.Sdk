using System;
using CodeDesignPlus.Net.Core.Abstractions;

namespace CodeDesignPlus.Net.Core.Sample.Resources.DataTransferObject;

public class OrderDto : IDto
{
    public Guid Tenant { get; set; }
    public Guid Id { get; set; }

    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
}
