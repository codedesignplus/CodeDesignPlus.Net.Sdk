using MediatR;

namespace CodeDesignPlus.Net.Microservice.Commons.Test.Helpers.Application.Commands;

public class CreateOrderCommandHandler() : IRequestHandler<CreateOrderCommand>
{
    public Task Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}

