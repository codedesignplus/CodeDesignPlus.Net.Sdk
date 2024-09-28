using FluentValidation;
using MediatR;
using CodeDesignPlus.Net.Microservice.Commons.Test.Helpers.Models;

namespace CodeDesignPlus.Net.Microservice.Commons.Test.Helpers.Application.Commands;

public record CreateOrderCommand(Guid Id, Client Client) : IRequest;

public class Validator : AbstractValidator<CreateOrderCommand>
{
    public Validator()
    {
        RuleFor(x => x.Id).NotEmpty().NotNull();
        RuleFor(x => x.Client)
            .NotNull()
            .DependentRules(() =>
            {
                RuleFor(x => x.Client.Id).NotEmpty().NotNull();
                RuleFor(x => x.Client.Name).NotEmpty().NotNull();
            });
    }
}
