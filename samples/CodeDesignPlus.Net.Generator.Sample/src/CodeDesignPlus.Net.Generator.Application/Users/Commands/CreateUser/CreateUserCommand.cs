using CodeDesignPlus.Net.Generator.Attributes;

namespace CodeDesignPlus.Net.Generator.Application.Users.Commands.CreateUser;

[DtoGenerator]
public class CreateUserCommand
{
    public required string Name { get; set; }

    public required string LastName { get; set; }

    public required string Email { get; set; }

    public DateTime Birthdate { get; set; }

    public required string Password { get; set; }

    public required string Other { get; set; }
}
