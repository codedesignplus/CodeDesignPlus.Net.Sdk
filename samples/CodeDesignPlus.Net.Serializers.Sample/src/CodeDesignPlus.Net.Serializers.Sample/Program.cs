using CodeDesignPlus.Net.Serializers;
using CodeDesignPlus.Net.Serializers.Sample;

var userModel = new UserModel
{
    Id = Guid.NewGuid(),
    Name = "John Doe",
    Email = "john.doe@codedesignplus.com",
    Birthdate = DateTime.Now
};

var json = JsonSerializer.Serialize(userModel);

Console.WriteLine(json);

var userModelDeserialized = JsonSerializer.Deserialize<UserModel>(json);

Console.WriteLine(userModelDeserialized.Name);