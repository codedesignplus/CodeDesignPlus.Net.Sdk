using System.Reflection;
using CodeDesignPlus.Net.Core.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Sdk;

namespace CodeDesignPlus.Net.xUnit.Microservice.Validations.Startup;

/// <summary>
/// A custom attribute for providing data to test methods that validate startup services.
/// </summary>
public class StartupValidationAttribute : DataAttribute
{
    /// <summary>
    /// Gets the data for the test method.
    /// </summary>
    /// <param name="testMethod">The test method for which data is being provided.</param>
    /// <returns>An enumerable of object arrays representing the data for the test method.</returns>
    public override IEnumerable<object[]> GetData(MethodInfo testMethod)
    {
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder().Build();

        var startups = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(x => x.GetTypes())
            .Where(x => !x.FullName.StartsWith("Castle"))
            .Where(x => typeof(IStartupServices).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
            .Select(x => (IStartupServices)Activator.CreateInstance(x))
            .ToList();

        foreach (var startup in startups)
        {
            var exception = Record.Exception(() => startup.Initialize(services, configuration));

            yield return new object[] { startup, exception };
        }
    }
}