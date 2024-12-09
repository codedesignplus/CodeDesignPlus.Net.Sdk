using CodeDesignPlus.Net.Generator.Attributes;

namespace CodeDesignPlus.Net.Application;


[DtoGenerator]
public record CustomCommand(string Name, string Description);
