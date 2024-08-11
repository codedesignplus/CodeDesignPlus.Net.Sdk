using CodeDesignPlus.Net.Generator.Attributes;

namespace CodeDesignPlus.Net.Generator.Application;

[DtoGenerator]
public class CreateUserCommand
{
    public string? Name { get; set; }
    public int Age { get; set; }
}
