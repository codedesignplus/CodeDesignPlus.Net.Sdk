using MediatR;

namespace CodeDesignPlus.Net.Microservice.Commons.Test.Helpers.Application.Queries;

public class ProductQueryHandler : IRequestHandler<ProductRequest, ProductResponse>
{
    public Task<ProductResponse> Handle(ProductRequest request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new ProductResponse());
    }
}