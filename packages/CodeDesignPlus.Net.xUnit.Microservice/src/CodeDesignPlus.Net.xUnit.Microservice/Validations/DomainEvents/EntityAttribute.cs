using System.Reflection;
using CodeDesignPlus.Net.Core.Abstractions;
using CodeDesignPlus.Net.Core.Exceptions;
using CodeDesignPlus.Net.xUnit.Microservice.Utils.Reflection;
using Xunit.Sdk;

namespace CodeDesignPlus.Net.xUnit.Microservice.Validations.DomainEvents;

/// <summary>
/// A custom attribute for providing data to test methods that validate domain events.
/// </summary>
/// <param name="useCreateMethod">Indicates whether to use the static Create method or the constructor to create instances of domain events.</param>
public class DomainEventAttribute(bool useCreateMethod) : DataAttribute
{
    /// <summary>
    /// Gets the data for the test method.
    /// </summary>
    /// <param name="testMethod">The test method for which data is being provided.</param>
    /// <returns>An enumerable of object arrays representing the data for the test method.</returns>
    public override IEnumerable<object[]> GetData(MethodInfo testMethod)
    {
        var domainEvents = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(x => x.GetTypes())
            .Where(x => !x.FullName.StartsWith("Castle"))
            .Where(x => typeof(IDomainEvent).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
            .ToList();

        foreach (var domainEvent in domainEvents)
        {
            object instance;
            Dictionary<ParameterInfo, object> values;

            if (useCreateMethod)
            {
                var nameConstructor = domainEvent.GetMethod("Create", BindingFlags.Static | BindingFlags.Public);

                if (nameConstructor is null)
                    throw new CoreException($"The {domainEvent.Name} class does not have a static Create method.");
                    
                values = nameConstructor.GetParameters().GetParameterValues();
                instance = nameConstructor.Invoke(null, [.. values.Values]);
            }
            else
            {
                var constructor = domainEvent.GetConstructors().FirstOrDefault()!;
                values = constructor.GetParameters().GetParameterValues();
                instance = constructor.Invoke(values.Values.ToArray());
            }

            yield return new object[] { domainEvent, instance, values };
        }
    }
}