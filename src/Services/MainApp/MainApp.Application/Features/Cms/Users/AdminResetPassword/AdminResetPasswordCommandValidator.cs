using FluentValidation;

namespace MainApp.Application.Features.Cms.Users.AdminResetPassword;

public class AdminResetPasswordCommandValidator : AbstractValidator<AdminResetPasswordCommand>
{
    public AdminResetPasswordCommandValidator()
    {
        RuleFor(x => x.NewPassword).NotEmpty().MinimumLength(8).MaximumLength(32);
    }
}
