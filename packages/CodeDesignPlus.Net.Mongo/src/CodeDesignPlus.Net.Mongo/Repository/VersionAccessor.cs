namespace CodeDesignPlus.Net.Mongo.Repository;

/// <summary>
/// Reads and writes the <see cref="IVersionedAggregate.Version"/> of an aggregate through its private setter.
/// </summary>
/// <remarks>
/// The interface only exposes the getter so the domain cannot move the version; the repository is the one place that
/// does, after a write the store accepted. The setter is looked up once per type.
/// </remarks>
internal static class VersionAccessor
{
    /// <summary>
    /// The field name the version is stored under, which is the property name.
    /// </summary>
    public const string Field = nameof(IVersionedAggregate.Version);

    private static readonly ConcurrentDictionary<Type, Action<object, long>> setters = new();

    /// <summary>
    /// Sets the version of the aggregate.
    /// </summary>
    public static void Set(IVersionedAggregate aggregate, long version)
    {
        var setter = setters.GetOrAdd(aggregate.GetType(), BuildSetter);

        setter(aggregate, version);
    }

    private static Action<object, long> BuildSetter(Type type)
    {
        var property = type.GetProperty(Field, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        if (property is null || property.PropertyType != typeof(long) || property.GetSetMethod(true) is null)
        {
            throw new Exceptions.MongoException($"{type.Name} implements {nameof(IVersionedAggregate)} but has no 'long {Field} {{ get; private set; }}' property.");
        }

        var setMethod = property.GetSetMethod(true);

        return (target, value) => setMethod.Invoke(target, [value]);
    }
}
