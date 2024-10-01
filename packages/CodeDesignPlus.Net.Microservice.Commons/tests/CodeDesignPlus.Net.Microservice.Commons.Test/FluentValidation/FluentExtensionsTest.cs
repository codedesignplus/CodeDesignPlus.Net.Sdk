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
        services.AddFluentValidation();
        var serviceProvider = services.BuildServiceProvider();
        var validators = serviceProvider.GetServices<IValidator<CreateOrderCommand>>();

        // Assert
        Assert.NotNull(validators);
        Assert.NotEmpty(validators);
        Assert.NotEmpty(services);
    }
}
