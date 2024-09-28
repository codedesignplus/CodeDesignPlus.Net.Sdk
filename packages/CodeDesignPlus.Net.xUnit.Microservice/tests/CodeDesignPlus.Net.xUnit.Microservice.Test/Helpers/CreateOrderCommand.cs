using MediatR;

namespace CodeDesignPlus.Net.xUnit.Microservice.Test.Helpers;

public record CreateOrderCommand(Guid Id, string Status, ClientEntity Client) : IRequest;

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand>
{
    public Task Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
