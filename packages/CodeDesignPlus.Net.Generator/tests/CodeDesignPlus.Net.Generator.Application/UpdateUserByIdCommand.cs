using CodeDesignPlus.Net.Generator.Attributes;

namespace CodeDesignPlus.Net.Generator.Application;

[DtoGenerator]
public class UpdateUserByIdCommand
{
    private readonly int id;

    public static string Code = "UpdateUserByIdCommand";

    public Guid UUI { get; set; }
}


[DtoGenerator]
public record AddAddressCommand(string Street, string City, string State, string Country, string ZipCode);