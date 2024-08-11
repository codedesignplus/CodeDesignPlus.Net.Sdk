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
    public List<string> Emails { get; set; } = [];
}