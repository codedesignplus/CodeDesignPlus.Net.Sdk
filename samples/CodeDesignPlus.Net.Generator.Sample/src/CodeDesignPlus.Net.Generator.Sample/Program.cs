// See https://aka.ms/new-console-template for more information
using CodeDesignPlus.Microservice.Api.Dtos;

var createUserDto = new CreateUserDto()
{
    Name = "John",
    Email = "john.doe@codedesignplus.com",
    LastName = "Doe",
    Birthdate = new DateTime(1990, 10, 10),
    Password = ""
};

 var updateUserDto = new UpdateUserDto() {
    Id = Guid.NewGuid(),
    Name = "John",
    Email = "john.doe@codedesignplus.com",
    LastName = "Doe",
    Birthdate = new DateTime(1990, 10, 10),
    Password = ""
 };

Console.WriteLine(createUserDto.Name);
Console.WriteLine(updateUserDto.Id);

Console.ReadLine();
