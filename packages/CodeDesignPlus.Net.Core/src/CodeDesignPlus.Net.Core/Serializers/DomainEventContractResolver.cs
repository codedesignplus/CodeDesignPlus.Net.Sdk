using System.Reflection;

namespace CodeDesignPlus.Net.Core;

/// <summary>
/// Represents a contract resolver for the domain event.
/// </summary>
public class DomainEventContractResolver : DefaultContractResolver
{
    /// <summary>
    /// The property names to ignore.
    /// </summary>
    private readonly string[] propertyNamesToIgnore;

    /// <summary>
    /// 
    /// </summary>
    public DomainEventContractResolver()
    {
        this.propertyNamesToIgnore = [
            nameof(IDomainEvent.EventId),
            nameof(IDomainEvent.EventType),
            nameof(IDomainEvent.OccurredAt),
            nameof(IDomainEvent.Metadata)
        ];
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="propertyIgnores"></param>
    public DomainEventContractResolver(params string[] propertyIgnores)
    {
        this.propertyNamesToIgnore = propertyIgnores;
    }

    /// <summary>
    /// Creates a <see cref="JsonProperty"/> for the given <see cref="MemberInfo"/>.
    /// </summary>
    /// <param name="member">The member to create a <see cref="JsonProperty"/> for.</param>
    /// <param name="memberSerialization">The member's parent <see cref="MemberSerialization"/>.</param>
    /// <returns>A created <see cref="JsonProperty"/> for the given <see cref="MemberInfo"/>.</returns>
    protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
    {
        if (!typeof(DomainEvent).IsAssignableFrom(member.DeclaringType))
            return base.CreateProperty(member, memberSerialization);

        JsonProperty property = base.CreateProperty(member, memberSerialization);

        if (propertyNamesToIgnore.Contains(property.PropertyName, StringComparer.CurrentCultureIgnoreCase))
        {
            property.ShouldSerialize = _ => false;
        }

        return property;
    }

    /// <summary>
    /// Resolves the name of the property.
    /// </summary>
    /// <param name="propertyName">The name of the property.</param>
    /// <returns>The resolved name of the property.</returns>
    protected override string ResolvePropertyName(string propertyName)
    {
        if (string.IsNullOrEmpty(propertyName))
            return propertyName;

        return char.ToLowerInvariant(propertyName[0]) + propertyName[1..];
    }

    /// <summary>
    /// Resolves the dictionary key.
    /// </summary>
    /// <param name="dictionaryKey">The key of the dictionary.</param>
    /// <returns>The resolved key of the dictionary.</returns>
    protected override string ResolveDictionaryKey(string dictionaryKey)
    {
        return dictionaryKey;
    }
}
