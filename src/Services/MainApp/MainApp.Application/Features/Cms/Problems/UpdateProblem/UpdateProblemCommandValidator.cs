using FluentValidation;

namespace MainApp.Application.Features.Cms.Problems.UpdateProblem;

public class UpdateProblemCommandValidator : AbstractValidator<UpdateProblemCommand>
{
    public UpdateProblemCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.Name).NotEmpty().MinimumLength(2);
        RuleFor(x => x.ProblemText).NotEmpty();
        RuleFor(x => x.Categories).NotEmpty();
        RuleFor(x => x.Difficulty).IsInEnum();
        RuleFor(x => x.RuntimeLimitMs).GreaterThan(0);
        RuleFor(x => x.MemoryLimitMb).GreaterThan(0);
        RuleFor(x => x.TestCases).NotEmpty();
    }
}
