using System;

namespace CodeDesignPlus.Net.Generator.Attributes
{
    /// <summary>
    /// Attribute to indicate that a DTO (Data Transfer Object) should be generated for the class.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class DtoGeneratorAttribute : Attribute { }
}