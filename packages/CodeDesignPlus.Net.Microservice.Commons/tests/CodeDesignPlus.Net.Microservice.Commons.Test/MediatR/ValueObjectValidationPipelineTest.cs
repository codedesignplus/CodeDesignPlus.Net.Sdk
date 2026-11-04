using CodeDesignPlus.Net.Core.Abstractions;
using CodeDesignPlus.Net.Microservice.Commons.MediatR;
using MediatR;
using Moq;

namespace CodeDesignPlus.Net.Microservice.Commons.Test.MediatR;

/// <summary>
/// The constructor of a value object no longer validates (Kappali pendings/737), so the pipeline is what rejects a request
/// carrying an invalid one, wherever it sits in the request.
/// </summary>
public class ValueObjectValidationPipelineTest
{
    private readonly Mock<RequestHandlerDelegate<Unit>> nextMock = new();

    private static Task<Unit> Handle<TRequest>(TRequest request, RequestHandlerDelegate<Unit> next) where TRequest : IBaseRequest
    {
        return new ValueObjectValidationPipeline<TRequest, Unit>().Handle(request, next, CancellationToken.None);
    }

    [Fact]
    public async Task Handle_ValidValueObjects_CallsNext()
    {
        // Arrange
        var request = new CreateThingCommand(Guid.NewGuid(), new Code("COP"), [new Line(new Code("USD"))], new Dictionary<string, Code> { ["x"] = new("EUR") });

        // Act
        await Handle(request, nextMock.Object);

        // Assert
        nextMock.Verify(n => n(CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task Handle_InvalidValueObjectProperty_ThrowsItsError()
    {
        // Arrange
        var request = new CreateThingCommand(Guid.NewGuid(), new Code("COPX"), [], []);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidCodeException>(() => Handle(request, nextMock.Object));
    }

    [Fact]
    public async Task Handle_InvalidValueObjectProperty_DoesNotCallNext()
    {
        // Arrange
        var request = new CreateThingCommand(Guid.NewGuid(), new Code("COPX"), [], []);

        // Act
        await Record.ExceptionAsync(() => Handle(request, nextMock.Object));

        // Assert
        nextMock.Verify(n => n(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_InvalidValueObjectInsideAListOfDtos_ThrowsItsError()
    {
        // Arrange
        var request = new CreateThingCommand(Guid.NewGuid(), new Code("COP"), [new Line(new Code("US"))], []);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidCodeException>(() => Handle(request, nextMock.Object));
    }

    [Fact]
    public async Task Handle_InvalidValueObjectInsideADictionary_ThrowsItsError()
    {
        // Arrange
        var request = new CreateThingCommand(Guid.NewGuid(), new Code("COP"), [], new Dictionary<string, Code> { ["x"] = new("EURO") });

        // Act & Assert
        await Assert.ThrowsAsync<InvalidCodeException>(() => Handle(request, nextMock.Object));
    }

    [Fact]
    public async Task Handle_NullValueObject_CallsNext()
    {
        // Arrange: whether a value object is required is the validator's call, not the pipeline's.
        var request = new CreateThingCommand(Guid.NewGuid(), null, [], []);

        // Act
        await Handle(request, nextMock.Object);

        // Assert
        nextMock.Verify(n => n(CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task Handle_CyclicGraph_CallsNext()
    {
        // Arrange
        var node = new Node { Code = new Code("COP") };
        node.Next = node;

        // Act
        await Handle(new NodeCommand(node), nextMock.Object);

        // Assert
        nextMock.Verify(n => n(CancellationToken.None), Times.Once);
    }

    [Fact]
    public void AddMediatR_RegistersTheValueObjectPipeline()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddMediatR<Startup>();

        // Assert
        Assert.Contains(services, s => s.ImplementationType == typeof(ValueObjectValidationPipeline<,>));
    }

    public sealed class InvalidCodeException(string message) : Exception(message);

    public sealed class Code(string value) : IValidatableValueObject
    {
        public string Value { get; } = value;

        public void EnsureValid()
        {
            if (Value.Length != 3)
            {
                throw new InvalidCodeException($"'{Value}' is not a three-letter code");
            }
        }
    }

    public sealed record Line(Code Currency);

    public sealed record CreateThingCommand(Guid Id, Code? Currency, List<Line> Lines, Dictionary<string, Code> ByName) : IRequest;

    public sealed class Node
    {
        public Code Code { get; set; } = null!;

        public Node? Next { get; set; }
    }

    public sealed record NodeCommand(Node Root) : IRequest;
}
