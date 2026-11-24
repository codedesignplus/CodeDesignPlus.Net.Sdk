namespace CodeDesignPlus.Net.Core.Test.Helpers;

/// <summary>
/// Options with a list, to detect duplicated binds.
/// </summary>
public class ListOptionsFake
{
    public static readonly string Section = "ListFake";

    public List<string> Items { get; set; } = [];
}
