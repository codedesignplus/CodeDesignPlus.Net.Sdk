namespace CodeDesignPlus.Net.Security.Test.Helpers.Models;

public class UserInfo
{
    public User? User { get; set; }
}

public class User
{
    public string? IdUser { get; set; }
    public string? Name { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public string? PostalCode { get; set; }
    public string? StreetAddress { get; set; }
    public string? State { get; set; }
    public string? JobTitle { get; set; }

    // `Email`, en singular: `IUserContext` dejo de exponer una lista y el endpoint de pruebas serializa
    // `userContext.Email`. Con `Emails` aqui, la deserializacion no encontraba esa propiedad y la lista
    // llegaba siempre vacia, asi que la comprobacion del correo no comprobaba nada.
    public string? Email { get; set; }
}