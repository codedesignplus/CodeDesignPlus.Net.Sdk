using MediatR;

namespace CodeDesignPlus.Net.xUnit.Microservice.Test.Helpers;

public record GetOrderByIdQuery(Guid Id) : IRequest<OrderResponse>;

public class OrderResponse
{
}

public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, OrderResponse>
{
    public Task<OrderResponse> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new OrderResponse());
    }
}