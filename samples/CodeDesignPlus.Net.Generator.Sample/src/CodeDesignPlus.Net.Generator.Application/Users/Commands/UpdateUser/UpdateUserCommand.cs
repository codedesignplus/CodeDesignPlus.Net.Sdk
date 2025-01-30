using CodeDesignPlus.Net.Generator.Attributes;

namespace CodeDesignPlus.Net.Generator.Application.Users.Commands.UpdateUser;

[DtoGenerator]
public record UpdateUserCommand(Guid Id, string Name, string LastName, string Email, DateTime Birthdate, string Password, string Other);