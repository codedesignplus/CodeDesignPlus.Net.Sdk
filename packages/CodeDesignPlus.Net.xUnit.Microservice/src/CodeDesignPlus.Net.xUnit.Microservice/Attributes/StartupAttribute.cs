namespace CodeDesignPlus.Net.xUnit.Microservice.Attributes;

/// <summary>
/// A custom attribute for providing data to test methods that validate startup services.
/// </summary>
/// <typeparam name="TAssemblyScan">The type of the assembly to scan for startup services.</typeparam>
[AttributeUsage(AttributeTargets.Method)]
public class StartupAttribute<TAssemblyScan> : DataAttribute
{
    /// <summary>
    /// A callback to configure the services and configuration.
    /// </summary>
    private readonly Action<IServiceCollection, IConfigurationBuilder> Configure;

    /// <summary>
    /// Initializes a new instance of the <see cref="StartupAttribute{TAssemblyScan}"/> class with no configuration callback.
    /// </summary>
    public StartupAttribute()
    {
        this.Configure = null!;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="StartupAttribute{TAssemblyScan}"/> class with a callback to configure services and configuration.
    /// </summary>
    /// <param name="configure">The callback to configure services and configuration.</param>
    public StartupAttribute(Action<IServiceCollection, IConfigurationBuilder> configure)
    {
        this.Configure = configure;
    }


    /// <summary>
    /// Gets the data for the test method.
    /// </summary>
    /// <param name="testMethod">The test method for which data is being provided.</param>
    /// <returns>An enumerable of object arrays representing the data for the test method.</returns>
    public override IEnumerable<object[]> GetData(MethodInfo testMethod)
    {
        var services = new ServiceCollection();
        var configurationBuilder = new ConfigurationBuilder();

        this.Configure?.Invoke(services, configurationBuilder);

        var configuration = configurationBuilder.Build();

        var startups = typeof(TAssemblyScan).Assembly
            .GetTypes()
            .Where(x => !x.FullName.StartsWith("Castle") || !x.FullName.Contains("DynamicProxyGenAssembly"))
            .Where(x => typeof(IStartup).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
            .Select(x => (IStartup)Activator.CreateInstance(x))
            .ToList();

        foreach (var startup in startups)
        {
            var exception = Record.Exception(() => startup.Initialize(services, configuration));

            yield return new object[] { startup, exception };
        }
    }
}