using FluentValidation;

namespace MainApp.Application.Features.Cms.Users.ChangeEmail;

public class ChangeEmailCommandValidator : AbstractValidator<ChangeEmailCommand>
{
    public ChangeEmailCommandValidator()
    {
        RuleFor(x => x.NewEmail).NotEmpty().EmailAddress();
    }
}
