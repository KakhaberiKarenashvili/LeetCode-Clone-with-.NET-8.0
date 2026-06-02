using FluentValidation;

namespace MainApp.Application.Features.Submissions.Commands.CreateSubmission;

public class CreateSubmissionCommandValidator : AbstractValidator<CreateSubmissionCommand>
{
    public CreateSubmissionCommandValidator()
    {
        RuleFor(x => x.ProblemId)
            .GreaterThan(0);

        RuleFor(x => x.Language)
            .IsInEnum();

        RuleFor(x => x.Code)
            .NotEmpty();
    }
}
