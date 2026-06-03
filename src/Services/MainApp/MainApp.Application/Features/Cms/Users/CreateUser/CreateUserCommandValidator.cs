using FluentValidation;

namespace MainApp.Application.Features.Cms.Users.CreateUser;

public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.UserName).NotEmpty().MinimumLength(6).MaximumLength(20);
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
    }
}
