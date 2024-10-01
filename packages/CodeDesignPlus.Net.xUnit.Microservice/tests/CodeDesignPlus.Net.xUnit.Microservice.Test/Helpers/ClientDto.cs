namespace CodeDesignPlus.Net.xUnit.Microservice.Test.Helpers;

public class ClientDto : IDtoBase
{
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public string? LastName { get; set; }

    public string? Email { get; set; }

    public string? Phone { get; set; }
}
