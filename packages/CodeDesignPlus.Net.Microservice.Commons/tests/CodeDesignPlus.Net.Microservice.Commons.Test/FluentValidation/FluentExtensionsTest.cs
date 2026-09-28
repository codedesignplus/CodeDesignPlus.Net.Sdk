using CodeDesignPlus.Net.Microservice.Commons.FluentValidation;
using CodeDesignPlus.Net.Microservice.Commons.Test.Helpers.Application.Commands;
using FluentValidation;

namespace CodeDesignPlus.Net.Microservice.Commons.Test.FluentValidation;

public class FluentExtensionsTest
{
    [Fact]
    public void AddFluentValidation_ShouldRegisterValidators()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
#pragma warning disable CS0618 // The obsolete overload is kept for compatibility and still has to work.
        services.AddFluentValidation();
#pragma warning restore CS0618
        var serviceProvider = services.BuildServiceProvider();
        var validators = serviceProvider.GetServices<IValidator<CreateOrderCommand>>();

        // Assert
        Assert.NotNull(validators);
        Assert.NotEmpty(validators);
        Assert.NotEmpty(services);
    }

    [Fact]
    public void AddFluentValidationOfT_AssemblyWithValidators_RegistersThem()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddFluentValidation<CreateOrderCommand>();
        var validators = services.BuildServiceProvider().GetServices<IValidator<CreateOrderCommand>>();

        // Assert
        Assert.NotEmpty(validators);
    }

    [Fact]
    public void AddFluentValidationOfT_AssemblyWithoutValidators_RegistersNoneAndDoesNotThrow()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddFluentValidation<ServiceCollection>();

        // Assert
        Assert.DoesNotContain(services, descriptor => descriptor.ServiceType.IsGenericType && descriptor.ServiceType.GetGenericTypeDefinition() == typeof(IValidator<>));
    }

    [Fact]
    public void AddFluentValidationOfT_Lifetime_IsApplied()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddFluentValidation<CreateOrderCommand>(ServiceLifetime.Singleton);

        // Assert
        Assert.Contains(services, descriptor => descriptor.ServiceType == typeof(IValidator<CreateOrderCommand>) && descriptor.Lifetime == ServiceLifetime.Singleton);
    }

    [Fact]
    public void AddFluentValidationOfT_NullServices_ThrowsArgumentNullException()
    {
        // Arrange
        IServiceCollection services = null!;

        // Act
        var exception = Record.Exception(() => services.AddFluentValidation<CreateOrderCommand>());

        // Assert
        Assert.IsType<ArgumentNullException>(exception);
    }
}
