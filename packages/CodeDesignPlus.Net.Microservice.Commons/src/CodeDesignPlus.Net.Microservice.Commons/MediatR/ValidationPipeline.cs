using FluentValidation;
using MediatR;

namespace CodeDesignPlus.Net.Microservice.Commons.MediatR;

/// <summary>
/// Validation pipeline for handling requests and responses.
/// </summary>
/// <typeparam name="TRequest">Type of the request.</typeparam>
/// <typeparam name="TResponse">Type of the response.</typeparam>
/// <param name="validators">Collection of validators for the request.</param>
public class ValidationPipeline<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse> where TRequest : IBaseRequest
{
    private readonly IEnumerable<IValidator<TRequest>> validators = validators;

    /// <summary>
    /// Handles the request and performs validation.
    /// </summary>
    /// <param name="request">Client request.</param>
    /// <param name="next">Delegate for the next action in the pipeline.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Server response.</returns>
    /// <exception cref="ValidationException">Thrown when there are validation errors.</exception>
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var context = new ValidationContext<TRequest>(request);

        var failures = validators
            .Select(x => x.Validate(context))
            .SelectMany(x => x.Errors)
            .Where(x => x != null)
            .ToList();

        if (failures.Count != 0)
            throw new ValidationException(failures);

        return await next(cancellationToken);
    }
}