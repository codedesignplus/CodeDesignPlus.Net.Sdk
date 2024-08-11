using CodeDesignPlus.Net.Generator.Attributes;

namespace CodeDesignPlus.Net.Generator.Application;

public class DeleteUserUseCase
{
    [DtoGenerator]
    public class DeleteUserCommand
    {
        public int Id { get; set; }
    }

    public class DeleteUserHandler
    {

    }
}
