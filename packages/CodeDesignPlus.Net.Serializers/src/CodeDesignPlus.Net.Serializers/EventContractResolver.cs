namespace CodeDesignPlus.Net.Serializers;

/// <summary>
/// Contract resolver for the serialization of domain events.
/// </summary>
public class EventContractResolver : DefaultContractResolver
{
    /// <summary>
    /// The property names to ignore during serialization.
    /// </summary>
    private readonly string[] propertyNamesToIgnore;

    /// <summary>
    /// Initializes a new instance of the <see cref="EventContractResolver"/> class with default properties to ignore.
    /// </summary>
    public EventContractResolver()
    {
        this.propertyNamesToIgnore = new[]
        {
            nameof(IDomainEvent.EventId),
            nameof(IDomainEvent.OccurredAt),
            nameof(IDomainEvent.Metadata)
        };
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EventContractResolver"/> class with specified properties to ignore.
    /// </summary>
    /// <param name="propertyNamesToIgnore">The property names to ignore during serialization.</param>
    public EventContractResolver(string[] propertyNamesToIgnore)
    {
        this.propertyNamesToIgnore = propertyNamesToIgnore;
    }

    /// <summary>
    /// Creates a <see cref="JsonProperty"/> for the given <see cref="MemberInfo"/>.
    /// </summary>
    /// <param name="member">The member to create a <see cref="JsonProperty"/> for.</param>
    /// <param name="memberSerialization">The member's parent <see cref="MemberSerialization"/>.</param>
    /// <returns>A created <see cref="JsonProperty"/> for the given <see cref="MemberInfo"/>.</returns>
    protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
    {
        var property = base.CreateProperty(member, memberSerialization);

        if (!property.Writable)
        {
            property.Writable = (member as PropertyInfo).GetSetMethod(true) != null;
        }

        if (!typeof(DomainEvent).IsAssignableFrom(member.DeclaringType))
            return property;

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