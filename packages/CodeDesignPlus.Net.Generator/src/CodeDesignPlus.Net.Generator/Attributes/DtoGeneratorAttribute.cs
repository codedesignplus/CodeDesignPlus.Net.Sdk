using System;

namespace CodeDesignPlus.Net.Generator
{

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false)]
    public class DtoGeneratorAttribute : Attribute { }
}