using System;
using CodeDesignPlus.Net.Microservice.Commons.MediatR;
using CodeDesignPlus.Net.Microservice.Commons.Test.Helpers.Application.Queries;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Moq;

namespace CodeDesignPlus.Net.Microservice.Commons.Test.MediatR;


public class ValidationPipelineTest
{
    private readonly Mock<IValidator<OrderRequest>> _validatorMock;
    private readonly ValidationPipeline<OrderRequest, OrderResponse> _pipeline;
    private readonly Mock<RequestHandlerDelegate<OrderResponse>> _nextMock;

    public ValidationPipelineTest()
    {
        _validatorMock = new Mock<IValidator<OrderRequest>>();
        _pipeline = new ValidationPipeline<OrderRequest, OrderResponse>([_validatorMock.Object]);
        _nextMock = new Mock<RequestHandlerDelegate<OrderResponse>>();
    }

    [Fact]
    public async Task Handle_ShouldCallNext_WhenValidationPasses()
    {
        // Arrange
        var request = new OrderRequest();
        _validatorMock.Setup(v => v.Validate(It.IsAny<ValidationContext<OrderRequest>>()))
                      .Returns(new ValidationResult());

        // Act
        _ = await _pipeline.Handle(request, _nextMock.Object, CancellationToken.None);

        // Assert
        _nextMock.Verify(n => n(), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowValidationException_WhenValidationFails()
    {
        // Arrange
        var request = new OrderRequest();
        var failures = new List<ValidationFailure> {
            new("Property", "Error")
        }
        ;
        _validatorMock.Setup(v => v.Validate(It.IsAny<ValidationContext<OrderRequest>>()))
                      .Returns(new ValidationResult(failures));

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() => _pipeline.Handle(request, _nextMock.Object, CancellationToken.None));
        _nextMock.Verify(n => n(), Times.Never);
    }
}

