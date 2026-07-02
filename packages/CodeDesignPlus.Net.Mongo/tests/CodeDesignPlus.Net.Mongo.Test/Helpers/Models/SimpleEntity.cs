using CodeDesignPlus.Net.Core.Abstractions;

namespace CodeDesignPlus.Net.Mongo.Test.Helpers.Models;

public class SimpleEntity : IEntityBase
{
    public Guid Id { get; set; }
    public string? Value { get; set; }
}
