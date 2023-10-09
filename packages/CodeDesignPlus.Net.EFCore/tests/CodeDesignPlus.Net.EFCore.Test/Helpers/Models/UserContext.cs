using CodeDesignPlus.Net.Security.Abstractions;

namespace CodeDesignPlus.Net.EFCore.Test;

public class UserContext<TKeyUser> : IUserContext<TKeyUser>
{
    public bool IsApplication { get; set; }
    public required TKeyUser IdUser { get; set; }
    public bool IsAuthenticated { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
}
