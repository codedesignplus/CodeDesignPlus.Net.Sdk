using System;

namespace CodeDesignPlus.Net.Serializers.Sample;

public class UserModel
{
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public string? LastName { get; set; }

    public string? Email { get; set; }

    public Instant Birthdate { get; set; }

}
