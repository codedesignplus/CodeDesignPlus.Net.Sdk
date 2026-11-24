namespace CodeDesignPlus.Net.Core.Test.Helpers;

/// <summary>
/// Startup that counts how many times it runs on each <see cref="IServiceCollection"/> and binds a list option.
/// </summary>
public class CountingStartupFake : IStartup
{
    private static readonly ConditionalWeakTable<IServiceCollection, StrongBox<int>> executions = [];

    /// <summary>
    /// Gets how many times the startup ran on the given collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The number of executions.</returns>
    public static int GetExecutions(IServiceCollection services)
    {
        return executions.TryGetValue(services, out var box) ? box.Value : 0;
    }

    public void Initialize(IServiceCollection services, IConfiguration configuration)
    {
        var box = executions.GetOrCreateValue(services);

        Interlocked.Increment(ref box.Value);

        services.AddOptions<ListOptionsFake>().Bind(configuration.GetSection(ListOptionsFake.Section));
    }
}
