namespace CodeDesignPlus.Net.xUnit.Microservice.Test.Helpers;

public class OrderDto : IDtoBase
{
    public Guid Id { get; set; }

    public string? Status { get; set; }

    public ClientDto? Client { get; set; }
}
