using CodeDesignPlus.Net.Core.Abstractions;

namespace CodeDesignPlus.Net.Mongo.Test.Helpers.Models;

public class Product : IEntity<Guid, Guid>
{
    public Guid Id { get; set; }
    public bool IsActive  { get; set; }
    public Guid IdUserCreator  { get; set; }
    public DateTime CreatedAt  { get; set; }
}
