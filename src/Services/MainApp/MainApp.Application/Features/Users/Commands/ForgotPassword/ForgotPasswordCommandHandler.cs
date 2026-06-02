using MainApp.Application.Common.Options;
using MainApp.Application.Common.Services.Email;
using MainApp.Domain.Entity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace MainApp.Application.Features.Users.Commands.ForgotPassword;

public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, Unit>
{
    private readonly UserManager<User> _userManager;
    private readonly IEmailService _emailService;
    private readonly EmailOptions _emailOptions;

    public ForgotPasswordCommandHandler(
        UserManager<User> userManager,
        IEmailService emailService,
        IOptions<EmailOptions> emailOptions)
    {
        _userManager = userManager;
        _emailService = emailService;
        _emailOptions = emailOptions.Value;
    }

    public async Task<Unit> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user == null)
            return Unit.Value;

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var baseRoute = $"{_emailOptions.FrontendBaseUrl}/{_emailOptions.ResetPasswordRoute}";

        var body = await EmailTemplateProvider.GetEmailBodyAsync(
            request.Email,
            EmailTemplateProvider.PasswordResetTemplate,
            token,
            baseRoute);

        await _emailService.SendEmailAsync(request.Email, EmailSubjects.ResetPassword, body, cancellationToken);

        return Unit.Value;
    }
}
