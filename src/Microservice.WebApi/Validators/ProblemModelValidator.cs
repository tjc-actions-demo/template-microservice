using FluentValidation;
using Microservice.WebApi.Models;

namespace Microservice.WebApi.Validators;

public class ProblemModelValidator : AbstractValidator<ProblemModel>
{
    public ProblemModelValidator()
    {
        RuleFor(problem => problem.Description)
            .NotEmpty()
            .WithMessage("A problem must have a description.");

        RuleFor(problem => problem.Title)
            .NotEmpty()
            .WithMessage("A problem must have title.");
    }
}
