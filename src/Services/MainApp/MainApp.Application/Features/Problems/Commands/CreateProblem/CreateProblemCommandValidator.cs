using FluentValidation;

namespace MainApp.Application.Features.Problems.Commands.CreateProblem;

public class CreateProblemCommandValidator : AbstractValidator<CreateProblemCommand>
{
    public CreateProblemCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MinimumLength(2);

        RuleFor(x => x.ProblemText)
            .NotEmpty();

        RuleFor(x => x.Categories)
            .NotEmpty();

        RuleFor(x => x.Difficulty)
            .NotEmpty();

        RuleFor(x => x.RuntimeLimitMs)
            .GreaterThan(0);

        RuleFor(x => x.MemoryLimitMb)
            .GreaterThan(0);

        RuleFor(x => x.TestCases)
            .NotEmpty();
    }
}
